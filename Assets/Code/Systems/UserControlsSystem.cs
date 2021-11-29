using Code.EcsComponents;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.Systems
{
    public class UserControlsSystem: IEcsRunSystem, IEcsInitSystem
    {
        private EcsPool<RestartGameCommand> _restart;

        public void Init(EcsSystems systems)
        {
            _restart = systems.GetWorld().GetPool<RestartGameCommand>();
        }

        public void Run(EcsSystems systems)
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                int entity = systems.GetWorld().NewEntity();
                _restart.Add(entity);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}