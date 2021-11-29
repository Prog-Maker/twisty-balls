using Code.EcsComponents;
using Code.Extensions;
using Code.SO;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class BallPushToSceneSystem : ISystem
    {
        [Inject]
        public Config config;

        public void Act(IEntityStorage storage)
        {
            foreach (Entity<PushToScene> entity in storage.Query<PushToScene>())
            {
                GameObject ball = entity.Get<Go>().go;
                if (entity.Get1().requestCount > 0)
                {
                    ball.transform.localScale = Vector3.one * entity.Get<Mass>().CalcBallDiameter(config);
                    ball.GetComponent<Renderer>().material.color = entity.Get<BallType>().config.color;
                    entity.Get1().requestCount = 0;
                }

                ball.transform.localPosition = entity.Get<Position>().position;
            }
        }
    }
}