using Code.EcsComponents;
using Code.Phases;
using Kk.BusyEcs;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class BallDestroySystem
    {
        [LateUpdate]
        public void Act(Entity entity, BallDestroyAction _)
        {
            Object.Destroy(entity.Get<Go>().go);
            entity.DelEntity();
        }
    }
}