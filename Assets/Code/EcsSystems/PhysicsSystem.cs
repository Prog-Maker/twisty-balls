using System;
using Code.EcsComponents;
using Code.Extensions;
using Code.MonoBehaviors;
using Code.SO;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class PhysicsSystem : ISystem
    {
        [Serializable]
        private struct CollisionApplied { }

        [Inject]
        public Config config;

        private static Collider2D[] _results = new Collider2D[1024];

        public void Act(IEntityStorage storage)
        {
            int stepCount = config.movementStepCount;
            float dt = Time.deltaTime / stepCount;
            for (int i = 0; i < stepCount; i++)
            {
                int count = 0;
                Vector2 center = Vector2.zero;
                float totalMass = 0;
                foreach (var entity in storage.Query<Mass>())
                {
                    totalMass += entity.Get1().mass;
                    center += entity.Get<Position>().position;
                    count++;
                }

                center /= count;

                foreach (var entity in storage.Query<Velocity, Mass>())
                {
                    // float a = config.gravity * (totalMass - entity.Get2().mass) / (entity.Get<Position>().position - center).sqrMagnitude;
                    float a = config.gravity;
                    entity.Get1().velocity -= entity.Get<Position>().position.normalized * (a * dt);
                }

                foreach (var entity in storage.Query<Velocity, Mass>())
                {
                    entity.Get<Position>().position += entity.Get1().velocity * dt;
                }

                foreach (var entity in storage.Query<Velocity, Mass>())
                {
                    if (!entity.Has1() || !entity.Has2())
                    {
                        // because this loop can delete these components by reference from this loop
                        continue;
                    }

                    if (entity.Has<CollisionApplied>())
                    {
                        continue;
                    }

                    ref Position position = ref entity.Get<Position>();

                    int collisionCount = Physics2D.OverlapCircleNonAlloc(position.position, entity.Get2().CalcBallDiameter(config) / 2, _results);
                    for (int j = 0; j < collisionCount; j++)
                    {
                        if (_results[j].TryGetComponent(out EntityLink link)
                            && link.entity.TryGet(out Entity<Velocity, Mass> another)
                            && another != entity
                            && !another.Has<BallDestroyAction>()
                            && !another.Has<CollisionApplied>()
                            && !DoDistanceGrow(entity, another, dt)
                        )
                        {
                            if (another.Get<BallType>().config == entity.Get<BallType>().config)
                            {
                                Merge(entity, another);
                            }
                            else
                            {
                                Bounce(entity, another);
                            }

                            another.Add<CollisionApplied>();
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