using System;
using Code.EcsComponents;
using Code.Extensions;
using Code.MonoBehaviors;
using Code.SO;
using Kk.LeoQuery;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.EcsSystems
{
    public class PhysicsSystem : ISystem
    {
        [Serializable]
        private struct CollisionApplied { }

        [Inject]
        public Config config;

        private EcsPool<Position> _pos;
        private EcsPool<Mass> _mass;
        private EcsPool<Velocity> _velocity;
        private EcsPool<BallDestroyAction> _ballDestroy;
        private EcsPool<CollisionApplied> _collAppl;
        private EcsPool<PushToScene> _push;
        private EcsFilter _balls;
        private EcsFilter _collAppls;

        private static Collider2D[] _results = new Collider2D[1024];

        public PhysicsSystem(EcsWorld world)
        {
            _pos = world.GetPool<Position>();
            _mass = world.GetPool<Mass>();
            _velocity = world.GetPool<Velocity>();
            _ballDestroy = world.GetPool<BallDestroyAction>();
            _collAppl = world.GetPool<CollisionApplied>();
            _push = world.GetPool<PushToScene>();
            _balls = world.Filter<Velocity>().Inc<Mass>().End();
            _collAppls = world.Filter<CollisionApplied>().End();
        }

        public void Act(IEntityStorage storage)
        {
            bool statsPresent = storage.TrySingle(out Entity<CollisionStats> stats);
            
            int stepCount = config.movementStepCount;
            float dt = Time.deltaTime / stepCount;
            for (int i = 0; i < stepCount; i++)
            {
                int count = 0;
                Vector2 center = Vector2.zero;
                float totalMass = 0;
                foreach (int entity in _balls)
                {
                    totalMass += _mass.Get(entity).mass;
                    center += _pos.Get(entity).position;
                    count++;
                }

                center /= count;

                foreach (int entity in _balls)
                {
                    // float a = config.gravity * (totalMass - entity.Get2().mass) / (entity.Get<Position>().position - center).sqrMagnitude;
                    float a = config.gravity;
                    _velocity.Get(entity).velocity -= _pos.Get(entity).position.normalized * (a * dt);
                }

                foreach (int entity in _balls)
                {
                    _pos.Get(entity).position += _velocity.Get(entity).velocity * dt;
                }

                foreach (int entity in _balls)
                {
                    if (!_velocity.Has(entity) || !_mass.Has(entity))
                    {
                        // because this loop can delete these components by reference from this loop
                        continue;
                    }

                    if (_collAppl.Has(entity))
                    {
                        continue;
                    }

                    ref Position position = ref _pos.Get(entity);

                    int collisionCount = Physics2D.OverlapCircleNonAlloc(position.position, _mass.Get(entity).CalcBallDiameter(config) / 2, _results);
                    for (int j = 0; j < collisionCount; j++)
                    {
                        if (!_results[j].TryGetComponent(out EntityLink link))
                            continue;

                        if (!link.entity.raw.Unpack(out var world, out int another))
                            continue;

                        if (!_velocity.Has(another))
                            continue;

                        if (!_mass.Has(another))
                            continue;
                        
                        if (another == entity) 
                            continue; 
                        
                        if (_ballDestroy.Has(another))
                            continue;
                        if (_collAppl.Has(another)) 
                            continue;
                        Entity<Velocity,Mass> entity_ = new Entity<Velocity, Mass>(world.PackEntityWithWorld(entity));
                        Entity<Velocity,Mass> another_ = new Entity<Velocity, Mass>(world.PackEntityWithWorld(another));
                        if (!DoDistanceGrow(entity_, another_, dt))
                        {
                            if (statsPresent)
                            {
                                stats.Get<CollisionStats>().collisions++;
                            }
                            
                            if (another_.Get<BallType>().config == entity_.Get<BallType>().config)
                            {
                                Merge(entity_, another_);
                            }
                            else
                            {
                                Bounce(entity_, another_);
                            }

                            another_.Add<CollisionApplied>();
                            break;
                        }
                    }
                }

                foreach (Entity<CollisionApplied> another in storage.Query<CollisionApplied>())
                {
                    another.Del1();
                }
            }
        }

        private static void Merge(Entity<Velocity, Mass> b1, Entity<Velocity, Mass> b2)
        {
            Vector2 myImpulse = b1.Get1().velocity * b1.Get2().mass;
            Vector2 anotherImpulse = b2.Get1().velocity * b2.Get2().mass;

            float totalMass = b1.Get2().mass + b2.Get2().mass;

            b1.Get<Position>().position = Vector2.Lerp(
                b1.Get<Position>().position,
                b2.Get<Position>().position,
                Mathf.InverseLerp(
                    b1.Get2().mass,
                    b2.Get2().mass,
                    totalMass / 2
                )
            );
            b1.Get1().velocity = (myImpulse + anotherImpulse) / totalMass;
            b1.Get2().mass = b1.Get2().mass + b2.Get2().mass;
            b1.Get<PushToScene>().requestCount++;

            b2.Add<BallDestroyAction>();
            b2.Del<Mass>();
            b2.Del<Velocity>();
        }

        private void Bounce(Entity<Velocity, Mass> b1, Entity<Velocity, Mass> b2)
        {
            b1.Get1().velocity = ElasticImpactSpeed(
                b1.Get<Position>().position, b1.Get1().velocity, b1.Get2().mass,
                b2.Get<Position>().position, b2.Get1().velocity, b2.Get2().mass
            );
            b2.Get1().velocity = ElasticImpactSpeed(
                b2.Get<Position>().position, b2.Get1().velocity, b2.Get2().mass,
                b1.Get<Position>().position, b1.Get1().velocity, b1.Get2().mass
            );
        }

        private static bool DoDistanceGrow(Entity<Velocity, Mass> b1, Entity<Velocity, Mass> b2, float dt)
        {
            Vector2 p1 = b1.Get<Position>().position;
            Vector2 p2 = b2.Get<Position>().position;
            Vector2 v1 = b1.Get1().velocity;
            Vector2 v2 = b2.Get1().velocity;

            return (p1 - p2).sqrMagnitude > (p1 - v1 * dt - (p2 - v2 * dt)).sqrMagnitude;
        }

        // private Vector2 ElasticImpactSpeed(Vector3 v1, float m1, Vector3 v2, float m2)
        // {
        //     return ((m1 - m2) * v1 + 2 * m2 * v2) / (m1 + m2);
        // }

        private Vector2 ElasticImpactSpeed(
            Vector2 x1,
            Vector2 v1,
            float m1,
            Vector2 x2,
            Vector2 v2,
            float m2
        ) => v1 - ((2 * m2) / (m1 + m2)) * (Vector2.Dot(v1 - v2, x1 - x2) / (x1 - x2).sqrMagnitude) * (x1 - x2);
    }
}