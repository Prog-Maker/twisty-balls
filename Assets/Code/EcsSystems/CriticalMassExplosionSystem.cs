using Code.EcsComponents;
using Code.Extensions;
using Code.SO;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class CriticalMassExplosionSystem: ISystem
    {

        [Inject]
        public Config config;
        
        public void Act(IEntityStorage storage)
        {
            foreach (Entity<BallBody> entity in storage.Query<BallBody>())
            {
                if (entity.Get1().mass > config.criticalMass)
                {
                    entity.Add<BallDestroyAction>();
                    
                    float diameter = entity.Get1().CalcBallDiameter(config);

                    foreach (BallTypeConfig ballType in config.ballTypes)
                    {
                        Vector2 offset = Random.insideUnitCircle * diameter;
                        storage.NewEntity().Add(new BallInitAction(
                            name: "fragment",
                            position: entity.Get<BallBody>().position + offset,
                            direction: offset.normalized,
                            speed: config.criticalExplosionSpeed,
                            mass: entity.Get1().mass / config.ballTypes.Length,
                            ballType
                        ));
                    }
                }
            }
        }
    }
}