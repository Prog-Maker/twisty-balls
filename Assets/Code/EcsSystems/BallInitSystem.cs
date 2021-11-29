using Code.EcsComponents;
using Code.MonoBehaviors;
using Code.SO;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class BallInitSystem : ISystem
    {
        [Inject]
        public Config config;

        public void Act(IEntityStorage storage)
        {
            foreach (Entity<BallInitAction> entity in storage.Query<BallInitAction>())
            {
                BallInitAction initAction = entity.Get1();
                
                ref BallBody body = ref entity.Add<BallBody>();
                body.position = initAction.position;
                body.velocity = initAction.direction * initAction.speed;
                body.mass = initAction.mass;
                body.config = initAction.config;
                
                GameObject go = Object.Instantiate(config.ballPrefab);
                go.name = initAction.name;
                go.AddComponent<EntityLink>().entity = entity;
                entity.Add(new Go(go));
                entity.Add<PushToScene>().requestCount++;
                
                entity.Del<BallInitAction>();
            }
        }
    }
}