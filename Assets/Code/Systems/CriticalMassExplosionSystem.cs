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
                if (mass.mass > _config.Platform().criticalMass)
                {
                    if (!_ballDestroy.Has(entity))
                    {
                        _ballDestroy.Add(entity);
                    }
                    
                    float diameter = mass.CalcBallDiameter(_config);

                    for (var i = 0; i < _config.Platform().initialSpawn.typesCount; i++)
                    {
                        BallTypeConfig ballType = _config.ballTypes[i];
                        Vector2 offset = Random.insideUnitCircle * diameter;
                        _ballInit.Add(systems.GetWorld().NewEntity()) = new BallInitAction(
                            name: "fragment",
                            position: _pos.Get(entity).position + offset,
                            direction: offset.normalized,
                            speed: _config.Platform().criticalExplosionSpeed,
                            mass: mass.mass / _config.Platform().initialSpawn.typesCount,
                            ballType
                        );
                    }

                    Stats.Instance.explosions++;
                }
            }
        }
    }
}