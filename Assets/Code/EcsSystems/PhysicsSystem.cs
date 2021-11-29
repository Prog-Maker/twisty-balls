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
        private EcsPool<BallType> _type;
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
            _type = world.GetPool<BallType>();
            _balls = world.Filter<Velocity>().Inc<Mass>().End();
            _collAppls = world.Filter<CollisionApplied>().End();
        }

        public void Act(IEntityStorage storage)
        {
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
                    // float a = config.gravity * (totalMass - _mass.Get(entity).mass) / (_pos.Get(entity)().position - center).sqrMagnitude;
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
                        if (!DoDistanceGrow(entity, another, dt))
                        {
                            if (_type.Get(another).config == _type.Get(entity).config)
                            {
                                Merge(entity, another);
                            }
                            else
                            {
                                Bounce(entity, another);
                            }

                            _collAppl.Add(another);
                            break;
                        }
                    }
                }

                foreach (int another in _collAppls)
                {
                    _collAppl.Del(another);
                }
            }
        }

        private void Merge(int b1, int b2)
        {
            Vector2 myImpulse = _velocity.Get(b1).velocity * _mass.Get(b1).mass;
            Vector2 anotherImpulse = _velocity.Get(b2).velocity * _mass.Get(b2).mass;

            float totalMass = _mass.Get(b1).mass + _mass.Get(b1).mass;

            _pos.Get(b1).position = Vector2.Lerp(
                _pos.Get(b1).position,
                _pos.Get(b2).position,
                Mathf.InverseLerp(
                    _mass.Get(b1).mass,
                    _mass.Get(b2).mass,
                    totalMass / 2
                )
            );
            _velocity.Get(b1).velocity = (myImpulse + anotherImpulse) / totalMass;
            _mass.Get(b1).mass = _mass.Get(b1).mass + _mass.Get(b2).mass;
            _push.Get(b1).requestCount++;

            _ballDestroy.Add(b2);
            _mass.Del(b2);
            _velocity.Del(b2);
        }

        private void Bounce(int b1, int b2)
        {
            _velocity.Get(b1).velocity = ElasticImpactSpeed(
                _pos.Get(b1).position, _velocity.Get(b1).velocity, _mass.Get(b1).mass,
                _pos.Get(b2).position, _velocity.Get(b2).velocity, _mass.Get(b2).mass
            );
            _velocity.Get(b2).velocity = ElasticImpactSpeed(
                _pos.Get(b2).position, _velocity.Get(b2).velocity, _mass.Get(b2).mass,
                _pos.Get(b1).position, _velocity.Get(b1).velocity, _mass.Get(b1).mass
            );
        }

        private bool DoDistanceGrow(int b1, int b2, float dt)
        {
            Vector2 p1 = _pos.Get(b1).position;
            Vector2 p2 = _pos.Get(b2).position;
            Vector2 v1 = _velocity.Get(b1).velocity;
            Vector2 v2 = _velocity.Get(b2).velocity;

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