using Code.EcsComponents;
using Code.Extensions;
using Code.Phases;
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
        
        [Update]
        public void Act(Entity entity, ref Mass mass, ref Position position)
        {
            if (mass.mass > config.Platform().criticalMass)
            {
                entity.Add<BallDestroyAction>();
                
                float diameter = mass.CalcBallDiameter(config);

                for (var i = 0; i < config.Platform().initialSpawn.typesCount; i++)
                {
                    BallTypeConfig ballType = config.ballTypes[i];
                    Vector2 offset = Random.insideUnitCircle * diameter;
                    env.NewEntity(new BallInitAction(
                        name: "fragment",
                        position: position.position + offset,
                        direction: offset.normalized,
                        speed: config.Platform().criticalExplosionSpeed,
                        mass: mass.mass / config.Platform().initialSpawn.typesCount,
                        ballType
                    ));
                }

                Stats.Instance.explosions++;
            }
        }
    }
}