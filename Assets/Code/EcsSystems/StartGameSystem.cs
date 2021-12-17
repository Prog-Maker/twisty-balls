using Code.EcsComponents;
using Code.MonoBehaviors;
using Code.Phases;
using Code.SO;
using Kk.BusyEcs;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class StartGameSystem
    {
        [Inject]
        public Config config;


        [Inject]
        public IEnv env;

        [EarlyUpdate]
        public void Act(Entity command, StartGameCommand _)
        {
            Random.InitState(123);
            for (int i = 0; i < config.Platform().initialSpawn.ballNumber; i++)
            {
                float dir = Random.Range(0, 360);
                Vector2 position = Random.insideUnitCircle * (config.Platform().initialSpawn.maxCenterDistance * 2);
                env.NewEntity(new BallInitAction(
                    position: position,
                    direction: new Vector2(Mathf.Cos(dir * Mathf.Deg2Rad), Mathf.Sin(dir * Mathf.Deg2Rad)),
                    speed: config.Platform().initialSpawn.ballSpeed,
                    mass: config.Platform().initialSpawn.ballMass * config.Platform().initialSpawn.ballMassDistribution.Evaluate(Random.value),
                    config: config.ballTypes[Random.Range(0, config.Platform().initialSpawn.typesCount)],
                    name: $"Ball {i:0000}"
                ));
            }

            foreach (BallSpawn ballSpawn in Object.FindObjectsOfType<BallSpawn>())
            {
                ballSpawn.GetComponent<Renderer>().enabled = false;
                env.NewEntity(new BallInitAction(
                    position: ballSpawn.transform.localPosition,
                    direction: ballSpawn.direction.normalized,
                    speed: ballSpawn.speed,
                    mass: ballSpawn.mass,
                    config: ballSpawn.type,
                    name: ballSpawn.name
                ));
            }

            command.DelEntity();

            Stats.Instance = default;
        }
    }
}