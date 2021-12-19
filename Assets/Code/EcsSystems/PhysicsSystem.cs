using System;
using System.Collections.Generic;
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
        private Entity[] _candidates = new Entity[64];
        private RegularGrid<Entity> _collisionGrid;


        // that works a bit bad with hot reload - one frame information is lost, but we do not care
        private HashSet<SymmetricEntityPair> _collidedPrevFrame = new HashSet<SymmetricEntityPair>();
        private HashSet<SymmetricEntityPair> _collidedThisFrame = new HashSet<SymmetricEntityPair>();

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
            
            DrawCollisionGrid();
        }

        private void DrawCollisionGrid()
        {
            Gizmos.color = Color.white;
            Vector2Int gridSize = _collisionGrid.gridSize;
            Vector2 offset = _collisionGrid.offset;
            Vector2 size = _collisionGrid.size;
            float step = _collisionGrid.step;
            for (int y = 0; y <= gridSize.y; y++)
            {
                Gizmos.DrawLine(
                    offset + new Vector2(0, y * step),
                    offset + new Vector2(gridSize.x * step, y * step)
                );
            }
            for (int x = 0; x <= gridSize.x; x++)
            {
                Gizmos.DrawLine(
                    offset + new Vector2(x * step, 0),
                    offset + new Vector2(x * step, gridSize.y * step)
                );
            }
        }

        [Init]
        public void Init()
        {
            float size = Camera.main.orthographicSize;
            float aspect = Camera.main.aspect;
            Vector2 gridSize = new Vector2(aspect * size, size);
            float step = new Mass { mass = config.Platform().criticalMass }.CalcBallDiameter(config) * 2;
            Debug.Log($"regular grid with step {step} is used");
            _collisionGrid = new RegularGrid<Entity>(-gridSize / 2, gridSize, step, 5000);
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
                    if (entity.Has<BallDestroyAction>())
                    {
                        return;
                    }

                    if (collisionStrategy == Config.CollisionStrategy.Unity2D)
                    {
                        int collisionCount = Physics2D.OverlapCircleNonAlloc(position.position, mass.CalcBallDiameter(config) / 2, _results);
                        for (int j = 0; j < collisionCount; j++)
                        {
                            if (!_results[j].TryGetComponent(out EntityLink link)) continue;
                            if (!link.entity.Deref(out Entity another)) continue;
                            if (another == entity)
                            {
                                continue;
                            }

                            if (another.Has<BallDestroyAction>())
                            {
                                continue;
                            }

                            another.Match((ref Position anotherPos, ref Mass anotherMass) =>
                            {
                                float distance = (anotherPos.position - position.position).magnitude;
                                float minimalDistance = (anotherMass.CalcBallDiameter(config) + mass.CalcBallDiameter(config)) / 2;
                                if (minimalDistance < distance)
                                {
                                    return;
                                }

                                if (another.Get<BallType>().config == entity.Get<BallType>().config)
                                {
                                    Merge(entity, another);
                                }
                                else
                                {
                                    SymmetricEntityPair pair = new SymmetricEntityPair(entity, another);
                                    bool processedFromOtherSide = !_collidedThisFrame.Add(pair);
                                    bool stuckInside = _collidedPrevFrame.Contains(pair);

                                    if (stuckInside || !processedFromOtherSide)
                                    {
                                        PushAway(entity, another);
                                    }

                                    if (!processedFromOtherSide && !stuckInside)
                                    {
                                        Bounce(entity, another);
                                    }
                                }
                            });
                        }
                    }

                    if (collisionStrategy == Config.CollisionStrategy.CustomRegularGrid)
                    {
                        int candidateCount = _collisionGrid.SearchNonAlloc(position.position, _candidates);

                        for (int j = 0; j < candidateCount; j++)
                        {
                            _candidates[j].Match((Entity another, ref Position anotherPos, ref Mass anotherMass) =>
                            {
                                if (another == entity)
                                {
                                    return;
                                }

                                if (another.Has<BallDestroyAction>())
                                {
                                    return;
                                }

                                float distance = (anotherPos.position - position.position).magnitude;
                                float minimalDistance = (anotherMass.CalcBallDiameter(config) + mass.CalcBallDiameter(config)) / 2;
                                if (minimalDistance < distance)
                                {
                                    return;
                                }

                                if (another.Get<BallType>().config == entity.Get<BallType>().config)
                                {
                                    Merge(entity, another);
                                }
                                else
                                {
                                    SymmetricEntityPair pair = new SymmetricEntityPair(entity, another);
                                    bool processedFromOtherSide = !_collidedThisFrame.Add(pair);
                                    bool stuckInside = _collidedPrevFrame.Contains(pair);

                                    if (stuckInside || !processedFromOtherSide)
                                    {
                                        PushAway(entity, another);
                                    }

                                    if (!processedFromOtherSide && !stuckInside)
                                    {
                                        Bounce(entity, another);
                                    }
                                }
                            });
                        }
                    }
                });

                if (collisionStrategy == Config.CollisionStrategy.CustomRegularGrid)
                {
                    _collisionGrid.Clear();
                }

                (_collidedPrevFrame, _collidedThisFrame) = (_collidedThisFrame, _collidedPrevFrame);
                _collidedThisFrame.Clear();
            }
        }

        private void PushAway(Entity b1, Entity b2)
        {
            Vector2 push1 = (b1.Get<Position>().position - b2.Get<Position>().position).normalized;
            b1.Get<Velocity>().velocity += push1 * config.pushAwaySpeed;
            b2.Get<Velocity>().velocity -= push1 * config.pushAwaySpeed;
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