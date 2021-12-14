using System;
using Code.EcsComponents;
using Code.Extensions;
using Code.MonoBehaviors;
using Code.SO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Systems
{
    public class PhysicsSystem : IEcsRunSystem
    {
        [Serializable]
        private struct CollisionApplied { }

        [EcsShared] private Config _config;

        [EcsPool] private EcsPool<Position> _pos;
        [EcsPool] private EcsPool<BallType> _type;
        [EcsPool] private EcsPool<Mass> _mass;
        [EcsPool] private EcsPool<Velocity> _velocity;
        [EcsPool] private EcsPool<BallDestroyAction> _ballDestroy;
        [EcsPool] private EcsPool<CollisionApplied> _collAppl;
        [EcsPool] private EcsPool<PushToScene> _push;
        [EcsFilter(typeof(Velocity), typeof(Mass))] private EcsFilter _balls;
        [EcsFilter(typeof(CollisionApplied))]private EcsFilter _collAppls;

        private static Collider2D[] _results = new Collider2D[1024];

        public void Run(EcsSystems systems)
        {
            int stepCount = _config.movementStepCount;
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
                    float a = _config.gravity;
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

                    int collisionCount = Physics2D.OverlapCircleNonAlloc(position.position, _mass.Get(entity).CalcBallDiameter(_config) / 2, _results);
                    for (int j = 0; j < collisionCount; j++)
                    {
                        if (!_results[j].TryGetComponent(out EntityLink link))
                            continue;

                        if (!link.entity.Unpack(out EcsWorld _, out int another))
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