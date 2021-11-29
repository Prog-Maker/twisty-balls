using Code.EcsComponents;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class UserControlsSystem: ISystem
    {
        public void Act(IEntityStorage storage)
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                storage.NewEntity().Add<RestartGameCommand>();
            }
        }
    }
}