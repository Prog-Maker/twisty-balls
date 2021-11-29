using Code.EcsComponents;
using Code.MonoBehaviors;
using Code.SO;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class StartGameSystem : ISystem
    {
        [Inject]
        public Config config;

        public void Act(IEntityStorage storage)
        {
            Random.InitState(123);
            foreach (Entity<StartGameCommand> command in storage.Query<StartGameCommand>())
            {
                for (int i = 0; i < config.initialSpawn.ballNumber; i++)
                {
                    float dir = Random.Range(0, 360);
                    Vector2 position = Random.insideUnitCircle * (config.initialSpawn.maxCenterDistance * 2);
                    storage.NewEntity().Add(new BallInitAction(
                        position: position,
                        direction: new Vector2(Mathf.Cos(dir * Mathf.Deg2Rad), Mathf.Sin(dir * Mathf.Deg2Rad)),
                        speed: config.initialSpawn.ballSpeed,
                        mass: config.initialSpawn.ballMass,
                        config: config.ballTypes[Random.Range(0, config.ballTypes.Length)],
                        name: $"Ball {i:0000}"
                    ));
                }

                command.Destroy();

                foreach (BallSpawn ballSpawn in Object.FindObjectsOfType<BallSpawn>())
                {
                    ballSpawn.GetComponent<Renderer>().enabled = false;
                    storage.NewEntity().Add(new BallInitAction(
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
}