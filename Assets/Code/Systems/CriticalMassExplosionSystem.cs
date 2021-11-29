using Code.EcsComponents;
using Code.Extensions;
using Code.SO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Systems
{
    public class CriticalMassExplosionSystem: IEcsRunSystem
    {
        [EcsShared] private Config _config;

        [EcsFilter(typeof(Mass))] private EcsFilter _masses;
        [EcsPool] private EcsPool<Mass> _mass;
        [EcsPool] private EcsPool<BallDestroyAction> _ballDestroy;
        [EcsPool] private EcsPool<BallInitAction> _ballInit;
        [EcsPool] private EcsPool<Position> _pos;

        public void Run(EcsSystems systems)
        {
            foreach (int entity in _masses)
            {
                ref Mass mass = ref _mass.Get(entity);
                if (mass.mass > _config.criticalMass)
                {
                    _ballDestroy.Add(entity);
                    
                    float diameter = mass.CalcBallDiameter(_config);

                    foreach (BallTypeConfig ballType in _config.ballTypes)
                    {
                        Vector2 offset = Random.insideUnitCircle * diameter;
                        _ballInit.Add(systems.GetWorld().NewEntity()) = new BallInitAction(
                            name: "fragment",
                            position: _pos.Get(entity).position + offset,
                            direction: offset.normalized,
                            speed: _config.criticalExplosionSpeed,
                            mass: mass.mass / _config.ballTypes.Length,
                            ballType
                        );
                    }
                }
            }
        }
    }
}