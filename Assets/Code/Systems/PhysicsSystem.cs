using System.Collections.Generic;
using Code.EcsComponents;
using Code.Extensions;
using Code.MonoBehaviors;
using Code.Oop;
using Code.SO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Oop
{ }

namespace Code.Systems
{
    public class PhysicsSystem : IEcsRunSystem, IEcsInitSystem
    {
        private int[] _candidates = new int[64];
        private RegularGrid<int> _collisionGrid;

        // that works a bit bad with hot reload - one frame information is lost, but we do not care
        private HashSet<SymmetricEntityPair> _collidedPrevFrame = new HashSet<SymmetricEntityPair>();
        private HashSet<SymmetricEntityPair> _collidedThisFrame = new HashSet<SymmetricEntityPair>();

        [EcsShared] private Config _config;

        [EcsWorld] private EcsWorld _world;
        [EcsPool] private EcsPool<Position> _pos;
        [EcsPool] private EcsPool<BallType> _type;
        [EcsPool] private EcsPool<Mass> _mass;
        [EcsPool] private EcsPool<Velocity> _velocity;
        [EcsPool] private EcsPool<BallDestroyAction> _ballDestroy;
        [EcsPool] private EcsPool<PushToScene> _push;

        [EcsFilter(typeof(Velocity), typeof(Mass))]
        private EcsFilter _balls;

        private static Collider2D[] _results = new Collider2D[1024];

        public void Init(EcsSystems systems)
        {
            float size = Camera.main.orthographicSize;
            float aspect = Camera.main.aspect;
            Vector2 gridSize = new Vector2(aspect * size, size);
            float step = new Mass { mass = _config.Platform().criticalMass }.CalcBallDiameter(_config) * 2;
            Debug.Log($"regular grid with step {step} is used");
            _collisionGrid = new RegularGrid<int>(-gridSize / 2, gridSize, step, 5000);
        }

        public void Run(EcsSystems systems)
        {
            int stepCount = _config.Platform().movementStepCount;
            float dt = Constants.FixedDt;
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
                    float a = _config.Platform().gravity;
                    _velocity.Get(entity).velocity -= _pos.Get(entity).position.normalized * (a * dt);
                }

                Config.CollisionStrategy collisionStrategy = _config.Platform().collisionStrategy;

                foreach (int entity in _balls)
                {
                    _pos.Get(entity).position += _velocity.Get(entity).velocity * dt;
                    if (collisionStrategy == Config.CollisionStrategy.CustomRegularGrid)
                    {
                        Vector2 position = _pos.Get(entity).position;
                        _collisionGrid.Add(position, entity);
                    }
                }

                foreach (int entity in _balls)
                {
                    if (_ballDestroy.Has(entity))
                    {
                        continue;
                    }

                    ref Position position = ref _pos.Get(entity);

                    if (collisionStrategy == Config.CollisionStrategy.Unity2D)
                    {
                        int collisionCount =
                            Physics2D.OverlapCircleNonAlloc(position.position, _mass.Get(entity).CalcBallDiameter(_config) / 2, _results);
                        for (int j = 0; j < collisionCount; j++)
                        {
                            if (!_results[j].TryGetComponent(out EntityLink link))
                                continue;

                            if (!link.entity.Unpack(out EcsWorld _, out int another))
                                continue;

                            if (another == entity)
                                continue;

                            if (!_velocity.Has(another))
                                continue;

                            if (!_mass.Has(another))
                                continue;

                            if (_ballDestroy.Has(another))
                            {
                                continue;
                            }

                            float distance = (_pos.Get(another).position - position.position).magnitude;
                            float minimalDistance = (_mass.Get(another).CalcBallDiameter(_config) + _mass.Get(entity).CalcBallDiameter(_config)) / 2;
                            if (distance > minimalDistance)
                            {
                                continue;
                            }

                            if (_type.Get(another).config == _type.Get(entity).config)
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
                        }
                    }

                    if (collisionStrategy == Config.CollisionStrategy.CustomRegularGrid)
                    {
                        int candidateCount = _collisionGrid.SearchNonAlloc(position.position, _candidates);

                        for (int j = 0; j < candidateCount; j++)
                        {
                            int another = _candidates[j];

                            if (another == entity)
                            {
                                continue;
                            }

                            if (_ballDestroy.Has(another))
                            {
                                continue;
                            }

                            float distance = (_pos.Get(another).position - position.position).magnitude;
                            float minimalDistance = (_mass.Get(another).CalcBallDiameter(_config) + _mass.Get(entity).CalcBallDiameter(_config)) / 2;
                            if (distance > minimalDistance)
                            {
                                continue;
                            }

                            if (_type.Get(another).config == _type.Get(entity).config)
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
                        }
                    }
                }

                if (collisionStrategy == Config.CollisionStrategy.CustomRegularGrid)
                {
                    _collisionGrid.Clear();
                }

                (_collidedPrevFrame, _collidedThisFrame) = (_collidedThisFrame, _collidedPrevFrame);
                _collidedThisFrame.Clear();
            }
        }

        private void PushAway(int b1, int b2)
        {
            Vector2 push1 = (_pos.Get(b1).position - _pos.Get(b2).position).normalized;
            _velocity.Get(b1).velocity += push1 * _config.pushAwaySpeed;
            _velocity.Get(b2).velocity -= push1 * _config.pushAwaySpeed;
        }

        private void Merge(int b1, int b2)
        {
            Stats.Instance.merges++;
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
        }

        private void Bounce(int b1, int b2)
        {
            Stats.Instance.bounces++;
            Vector2 prevB1Velocity = _velocity.Get(b1).velocity;
            _velocity.Get(b1).velocity = ElasticImpactSpeed(
                _pos.Get(b1).position, prevB1Velocity, _mass.Get(b1).mass,
                _pos.Get(b2).position, _velocity.Get(b2).velocity, _mass.Get(b2).mass
            );
            _velocity.Get(b2).velocity = ElasticImpactSpeed(
                _pos.Get(b2).position, _velocity.Get(b2).velocity, _mass.Get(b2).mass,
                _pos.Get(b1).position, prevB1Velocity, _mass.Get(b1).mass
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