using Code.EcsComponents;
using Code.MonoBehaviors;
using Code.Phases;
using Code.SO;
using Kk.BusyEcs;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class BallInitSystem
    {
        [Inject]
        public Config config;

        [EarlyUpdate]
        public void Act(Entity entity, BallInitAction initAction)
        {
            entity.Add<Position>().position = initAction.position;
            entity.Add<Velocity>().velocity = initAction.direction * initAction.speed;
            entity.Add<Mass>().mass = initAction.mass;
            entity.Add(new BallType(initAction.config));
            GameObject go = Object.Instantiate(config.ballPrefab);
            go.name = initAction.name;
            go.AddComponent<EntityLink>().entity = entity.AsRef();
            entity.Add(new Go(go));
            entity.Add<PushToScene>().requestCount++;

            entity.Del<BallInitAction>();
        }
    }
}