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
            for (int i = 0; i < config.initialSpawn.ballNumber; i++)
            {
                float dir = Random.Range(0, 360);
                Vector2 position = Random.insideUnitCircle * (config.initialSpawn.maxCenterDistance * 2);
                env.NewEntity(new BallInitAction(
                    position: position,
                    direction: new Vector2(Mathf.Cos(dir * Mathf.Deg2Rad), Mathf.Sin(dir * Mathf.Deg2Rad)),
                    speed: config.initialSpawn.ballSpeed,
                    mass: config.initialSpawn.ballMass,
                    config: config.ballTypes[Random.Range(0, config.ballTypes.Length)],
                    name: $"Ball {i:0000}"
                ));
            }

            command.DelEntity();

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
        }
    }
}