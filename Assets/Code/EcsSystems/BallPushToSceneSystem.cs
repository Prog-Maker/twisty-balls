using Code.EcsComponents;
using Code.Extensions;
using Code.Phases;
using Code.SO;
using Kk.BusyEcs;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class BallPushToSceneSystem
    {
        [Inject]
        public Config config;

        [Visualize]
        public void Act(Entity entity, PushToScene pushToScene, Go go)
        {
            {
                GameObject ball = go.go;
                if (pushToScene.requestCount > 0)
                {
                    ball.transform.localScale = Vector3.one * entity.Get<Mass>().CalcBallDiameter(config);
                    ball.GetComponent<Renderer>().material.color = entity.Get<BallType>().config.color;
                    pushToScene.requestCount = 0;
                }

                ball.transform.localPosition = entity.Get<Position>().position;
            }
        }
    }
}