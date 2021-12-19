using Code.EcsComponents;
using Code.MonoBehaviors;
using Code.SO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Systems
{
    public class StartGameSystem : IEcsRunSystem
    {
        [EcsShared] private Config _config;
        [EcsFilter(typeof(StartGameCommand))] EcsFilter _startGames;
        [EcsPool] private EcsPool<BallInitAction> _ballInit;

        public void Run(EcsSystems systems)
        {
            foreach (int command in _startGames)
            {
                Random.InitState(123);
                for (int i = 0; i < _config.Platform().initialSpawn.ballNumber; i++)
                {
                    float dir = Random.Range(0, 360);
                    Vector2 position = Random.insideUnitCircle * (_config.Platform().initialSpawn.maxCenterDistance * 2);
                    _ballInit.Add(systems.GetWorld().NewEntity()) = new BallInitAction(
                        position: position,
                        direction: new Vector2(Mathf.Cos(dir * Mathf.Deg2Rad), Mathf.Sin(dir * Mathf.Deg2Rad)),
                        speed: _config.Platform().initialSpawn.ballSpeed,
                        mass: _config.Platform().initialSpawn.ballMass * _config.Platform().initialSpawn.ballMassDistribution.Evaluate(Random.value),
                        config: _config.ballTypes[Random.Range(0, _config.Platform().initialSpawn.typesCount)],
                        name: $"Ball {i:0000}"
                    );
                }


                foreach (BallSpawn ballSpawn in Object.FindObjectsOfType<BallSpawn>())
                {
                    ballSpawn.GetComponent<Renderer>().enabled = false;
                    _ballInit.Add(systems.GetWorld().NewEntity()) = new BallInitAction(
                        position: ballSpawn.transform.localPosition,
                        direction: ballSpawn.direction.normalized,
                        speed: ballSpawn.speed,
                        mass: ballSpawn.mass,
                        config: ballSpawn.type,
                        name: ballSpawn.name
                    );
                }

                systems.GetWorld().DelEntity(command);
                Stats.Instance = default;
            }
        }
    }
}