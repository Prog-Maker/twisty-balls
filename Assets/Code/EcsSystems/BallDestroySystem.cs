using Code.EcsComponents;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class BallDestroySystem: ISystem
    {
        public void Act(IEntityStorage storage)
        {
            foreach (Entity<BallDestroyAction> entity in storage.Query<BallDestroyAction>())
            {
                Object.Destroy(entity.Get<Go>().go);
                entity.Destroy();
            }
        }
    }
}