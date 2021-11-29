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
                entity.Add<Position>().position = initAction.position;
                entity.Add<Velocity>().velocity = initAction.direction * initAction.speed;
                entity.Add<Mass>().mass = initAction.mass;
                entity.Add(new BallType(initAction.config));
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