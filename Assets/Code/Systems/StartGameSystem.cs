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
            Random.InitState(123);
            foreach (int command in _startGames)
            {
                for (int i = 0; i < _config.initialSpawn.ballNumber; i++)
                {
                    float dir = Random.Range(0, 360);
                    Vector2 position = Random.insideUnitCircle * (_config.initialSpawn.maxCenterDistance * 2);
                    _ballInit.Add(systems.GetWorld().NewEntity()) = new BallInitAction(
                        position: position,
                        direction: new Vector2(Mathf.Cos(dir * Mathf.Deg2Rad), Mathf.Sin(dir * Mathf.Deg2Rad)),
                        speed: _config.initialSpawn.ballSpeed,
                        mass: _config.initialSpawn.ballMass,
                        config: _config.ballTypes[Random.Range(0, _config.ballTypes.Length)],
                        name: $"Ball {i:0000}"
                    );
                }

                systems.GetWorld().DelEntity(command);

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
            }
        }
    }
}