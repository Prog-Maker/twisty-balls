using Code.EcsComponents;
using Code.Extensions;
using Code.SO;
using Kk.BusyEcs;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class CriticalMassExplosionSystem
    {

        [Inject]
        public Config config;

        [Inject]
        public IEnv env;
        
        public void Act(Entity entity, Mass mass)
        {
            {
                if (mass.mass > config.criticalMass)
                {
                    entity.Add<BallDestroyAction>();
                    
                    float diameter = mass.CalcBallDiameter(config);

                    foreach (BallTypeConfig ballType in config.ballTypes)
                    {
                        Vector2 offset = Random.insideUnitCircle * diameter;
                        env.NewEntity(new BallInitAction(
                            name: "fragment",
                            position: entity.Get<Position>().position + offset,
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