using Code.EcsComponents;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Code.Systems
{
    public class RestartGameSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(RestartGameCommand))] EcsFilter _restartGames;
        [EcsFilter(typeof(BallType))] private EcsFilter _ballTypes;
        [EcsPool] private EcsPool<StartGameCommand> _startGame;
        [EcsPool] private EcsPool<BallDestroyAction> _ballDestroy;
        
        public void Run(EcsSystems systems)
        {
            foreach (int command in _restartGames)
            {
                foreach (int entity in _ballTypes)
                {
                    _ballDestroy.Add(entity);
                }
                
                systems.GetWorld().DelEntity(command);

                _startGame.Add(systems.GetWorld().NewEntity());
            }
        }
    }
}