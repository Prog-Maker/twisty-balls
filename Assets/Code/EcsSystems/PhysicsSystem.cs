using System;
using Code.EcsComponents;
using Code.Extensions;
using Code.MonoBehaviors;
using Code.Phases;
using Code.SO;
using Kk.BusyEcs;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class PhysicsSystem
    {
        [Serializable]
        public struct CollisionApplied { }

        [Inject]
        public Config config;

        [Inject]
        public IEnv env;

        private static Collider2D[] _results = new Collider2D[1024];

        [Update]
        public void Act()
        {
            int stepCount = config.movementStepCount;
            float dt = Constants.FixedDt / stepCount;
            for (int i = 0; i < stepCount; i++)
            {
                int count = 0;
                Vector2 center = Vector2.zero;
                float totalMass = 0;
                env.Query((ref Mass mass, ref Position position) =>
                {
                    totalMass += mass.mass;
                    center += position.position;
                    count++;
                });

                center /= count;

                env.Query((ref Velocity velocity, ref Mass mass, ref Position position) =>
                {
                    // float a = config.gravity * (totalMass - entity.Get2().mass) / (entity.Get<Position>().position - center).sqrMagnitude;
                    float a = config.gravity;
                    velocity.velocity -= position.position.normalized * (a * dt);
                });

                env.Query((ref Velocity velocity, ref Mass mass, ref Position position) =>
                {
                    position.position += velocity.velocity * dt;
                });

                env.Query((Entity entity, ref Velocity velocity, ref Mass mass, ref Position position) =>
                {
                    // if (!entity.Has1() || !entity.Has2())
                    // {
                    //     // because this loop can delete these components by reference from this loop
                    //     continue;
                    // }

                    if (entity.Has<CollisionApplied>())
                    {
                        return;
                    }

                    int collisionCount = Physics2D.OverlapCircleNonAlloc(position.position, mass.CalcBallDiameter(config) / 2, _results);
                    for (int j = 0; j < collisionCount; j++)
                    {
                        if (!_results[j].TryGetComponent(out EntityLink link)) continue;
                        if (!link.entity.Deref(out Entity another)) continue;
                        if (!another.Has<Mass>() || !another.Has<Velocity>()) continue;
                        if (another == entity) continue;
                        if (another.Has<BallDestroyAction>()) continue;
                        if (another.Has<CollisionApplied>()) continue;
                        if (DoDistanceGrow(entity, another, dt)) continue;
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
                });

                env.Query((Entity another, ref CollisionApplied _) => another.Del<CollisionApplied>());
            }
        }

        private static void Merge(Entity b1, Entity b2)
        {
            Vector2 myImpulse = b1.Get<Velocity>().velocity * b1.Get<Mass>().mass;
            Vector2 anotherImpulse = b2.Get<Velocity>().velocity * b2.Get<Mass>().mass;

            float totalMass = b1.Get<Mass>().mass + b2.Get<Mass>().mass;

            b1.Get<Position>().position = Vector2.Lerp(
                b1.Get<Position>().position,
                b2.Get<Position>().position,
                Mathf.InverseLerp(
                    b1.Get<Mass>().mass,
                    b2.Get<Mass>().mass,
                    totalMass / 2
                )
            );
            b1.Get<Velocity>().velocity = (myImpulse + anotherImpulse) / totalMass;
            b1.Get<Mass>().mass = b1.Get<Mass>().mass + b2.Get<Mass>().mass;
            b1.Get<PushToScene>().requestCount++;

            b2.Add<BallDestroyAction>();
            b2.Del<Mass>();
            b2.Del<Velocity>();
        }

        private void Bounce(Entity b1, Entity b2)
        {
            b1.Get<Velocity>().velocity = ElasticImpactSpeed(
                b1.Get<Position>().position, b1.Get<Velocity>().velocity, b1.Get<Mass>().mass,
                b2.Get<Position>().position, b2.Get<Velocity>().velocity, b2.Get<Mass>().mass
            );
            b2.Get<Velocity>().velocity = ElasticImpactSpeed(
                b2.Get<Position>().position, b2.Get<Velocity>().velocity, b2.Get<Mass>().mass,
                b1.Get<Position>().position, b1.Get<Velocity>().velocity, b1.Get<Mass>().mass
            );
        }

        private static bool DoDistanceGrow(Entity b1, Entity b2, float dt)
        {
            Vector2 p1 = b1.Get<Position>().position;
            Vector2 p2 = b2.Get<Position>().position;
            Vector2 v1 = b1.Get<Velocity>().velocity;
            Vector2 v2 = b2.Get<Velocity>().velocity;
            
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