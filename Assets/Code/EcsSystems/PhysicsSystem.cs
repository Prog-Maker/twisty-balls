using System;
using Code.EcsComponents;
using Code.Extensions;
using Code.MonoBehaviors;
using Code.Oop;
using Code.Phases;
using Code.SO;
using Kk.BusyEcs;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class PhysicsSystem
    {
        private static Entity[] _candidates = new Entity[64];
        private static RegularGrid<Entity> _collisionGrid = new RegularGrid<Entity>(new Vector2(-3000, -1600), new Vector2(6000, 3200), 100, 5000);

        private struct Del
        {
            public int component;
            public Entity entity;
        }

        private static Del[] _delQueue = new Del[1024];
        private static int _delCount;

        [Serializable]
        public struct CollisionApplied { }

        [Inject]
        public Config config;

        [Inject]
        public IEnv env;

        private static Collider2D[] _results = new Collider2D[1024];

        [DrawGizmos]
        public void DrawGizmos(ref Velocity v, ref Position p, ref BallType type)
        {
            Gizmos.color = type.config.color;
            Gizmos.DrawLine(p.position, p.position + v.velocity * Constants.FixedDt);
        }

        [Update]
        public void Act()
        {
            float fullDt = Constants.FixedDt;

            float maxSegments = 1;

            env.Query((ref Velocity velocity, ref Mass mass) =>
            {
                Vector2 diff = velocity.velocity * fullDt;
                float segmentsFloat = diff.magnitude / mass.CalcBallDiameter(config);
                maxSegments = Mathf.Max(maxSegments, segmentsFloat);
            });

            int stepCount =
                config.Platform().movementStepCount;
            // Mathf.CeilToInt(maxSegments * config.stepFactor);

            float dt = fullDt / stepCount;

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
                    // float a = config.gravity * (totalMass - mass.mass) / (position.position - center).sqrMagnitude;
                    float a = config.Platform().gravity;
                    velocity.velocity -= position.position.normalized * (a * dt);
                });
                Config.CollisionStrategy collisionStrategy = config.Platform().collisionStrategy;
                
                env.Query((Entity entity, ref Velocity velocity, ref Mass mass, ref Position position) =>
                {
                    position.position += velocity.velocity * dt;
                    if (collisionStrategy == Config.CollisionStrategy.CustomRegularGrid)
                    {
                        _collisionGrid.Add(position.position, entity);
                    }
                });

                env.Query((Entity entity, ref Velocity velocity, Position position, Mass mass) =>
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
                    
                    if (entity.Has<BallDestroyAction>())
                    {
                        return;
                    }

                    if (collisionStrategy == Config.CollisionStrategy.CustomRegularGrid)
                    {
                        int candidateCount = _collisionGrid.SearchNonAlloc(position.position, _candidates);

                        for (int j = 0; j < candidateCount; j++)
                        {
                            bool @break = false;
                            _candidates[j].Match((Entity another, ref Position anotherPos, ref Mass anotherMass) =>
                            {
                                if (another == entity)
                                {
                                    return;
                                }

                                if ((anotherMass.CalcBallDiameter(config) + mass.CalcBallDiameter(config)) / 2 <
                                    (anotherPos.position - position.position).magnitude)
                                {
                                    return;
                                }

                                if (another.Has<BallDestroyAction>()) return;
                                if (another.Has<CollisionApplied>()) return;
                                another.Add<CollisionApplied>();
                                if (DoDistanceGrow(entity, another)) return;
                                if (another.Get<BallType>().config == entity.Get<BallType>().config)
                                {
                                    Merge(entity, another);
                                }
                                else
                                {
                                    Bounce(entity, another);
                                }

                                @break = true;
                            });
                            if (@break) break;
                        }
                    }

                    if (collisionStrategy == Config.CollisionStrategy.Unity2D)
                    {
                        int collisionCount = Physics2D.OverlapCircleNonAlloc(position.position, mass.CalcBallDiameter(config) / 2, _results);
                        for (int j = 0; j < collisionCount; j++)
                        {
                            if (!_results[j].TryGetComponent(out EntityLink link)) continue;
                            if (!link.entity.Deref(out Entity another)) continue;
                            if (!another.Has<Mass>() || !another.Has<Velocity>()) continue;
                            if (another == entity) continue;
                            if (another.Has<BallDestroyAction>()) continue;
                            if (another.Has<CollisionApplied>()) continue;
                            another.Add<CollisionApplied>();
                            if (DoDistanceGrow(entity, another)) continue;
                            if (another.Get<BallType>().config == entity.Get<BallType>().config)
                            {
                                Merge(entity, another);
                            }
                            else
                            {
                                Bounce(entity, another);
                            }

                            break;
                        }
                    }
                });

                if (collisionStrategy == Config.CollisionStrategy.CustomRegularGrid)
                {
                    _collisionGrid.Clear();
                }

                env.Query((Entity another, ref CollisionApplied _) => another.Del<CollisionApplied>());
                
                while (_delCount > 0)
                {
                    ref Del del = ref _delQueue[_delCount - 1];
                    del.entity.GetRaw(out EcsWorld world, out var entity);
                    world.GetPoolById(del.component).Del(entity);
                    _delCount--;
                }
            }
        }

        private static void Merge(Entity b1, Entity b2)
        {
            Stats.Instance.merges++;
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
        }

        private void Bounce(Entity b1, Entity b2)
        {
            Stats.Instance.bounces++;
            Vector2 prevB1Velocity = b1.Get<Velocity>().velocity;
            b1.Get<Velocity>().velocity = ElasticImpactSpeed(
                b1.Get<Position>().position, prevB1Velocity, b1.Get<Mass>().mass,
                b2.Get<Position>().position, b2.Get<Velocity>().velocity, b2.Get<Mass>().mass
            );
            b2.Get<Velocity>().velocity = ElasticImpactSpeed(
                b2.Get<Position>().position, b2.Get<Velocity>().velocity, b2.Get<Mass>().mass,
                b1.Get<Position>().position, prevB1Velocity, b1.Get<Mass>().mass
            );
        }

        private static bool DoDistanceGrow(Entity b1, Entity b2)
        {
            Vector2 p1 = b1.Get<Position>().position;
            Vector2 p2 = b2.Get<Position>().position;
            Vector2 v1 = b1.Get<Velocity>().velocity;
            Vector2 v2 = b2.Get<Velocity>().velocity;

            // float epsilon = Mathf.Epsilon;
            float epsilon = 0.001f;
            Vector2 p1Next = p1 + v1 * epsilon;
            Vector2 p2Next = p2 + v2 * epsilon;
            return (p1 - p2).sqrMagnitude < (p1Next - p2Next).sqrMagnitude;
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