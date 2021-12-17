using Code.EcsComponents;
using Code.Phases;
using Kk.BusyEcs;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class UserControlsSystem
    {
        [Inject]
        public IEnv env;

        [EarlyUpdate]
        public void Act()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                env.NewEntity(new RestartGameCommand());
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}