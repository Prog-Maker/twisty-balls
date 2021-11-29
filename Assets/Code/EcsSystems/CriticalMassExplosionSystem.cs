using Code.EcsComponents;
using Code.Extensions;
using Code.SO;
using Kk.LeoQuery;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.EcsSystems
{
    public class CriticalMassExplosionSystem: ISystem
    {
        [Inject]
        public Config config;

        private readonly EcsFilter _masses;
        private readonly EcsPool<Mass> _mass;
        private readonly EcsPool<BallDestroyAction> _ballDestroy;
        private readonly EcsPool<Position> _pos;

        public CriticalMassExplosionSystem(EcsWorld world)
        {
            _masses = world.Filter<Mass>().End();
            _mass = world.GetPool<Mass>();
            _ballDestroy = world.GetPool<BallDestroyAction>();
            _pos = world.GetPool<Position>();
        }

        public void Act(IEntityStorage storage)
        {
            foreach (int entity in _masses)
            {
                ref Mass mass = ref _mass.Get(entity);
                if (mass.mass > config.criticalMass)
                {
                    _ballDestroy.Add(entity);
                    
                    float diameter = mass.CalcBallDiameter(config);

                    foreach (BallTypeConfig ballType in config.ballTypes)
                    {
                        Vector2 offset = Random.insideUnitCircle * diameter;
                        storage.NewEntity().Add(new BallInitAction(
                            name: "fragment",
                            position: _pos.Get(entity).position + offset,
                            direction: offset.normalized,
                            speed: config.criticalExplosionSpeed,
                            mass: mass.mass / config.ballTypes.Length,
                            ballType
                        ));
                    }
                }
            }
        }
    }
}