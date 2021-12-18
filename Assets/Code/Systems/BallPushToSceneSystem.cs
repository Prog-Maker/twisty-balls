using Code.EcsComponents;
using Code.Extensions;
using Code.SO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Systems
{
    public class BallPushToSceneSystem : IEcsRunSystem
    {
        [EcsShared] private Config _config;

        [EcsPool] private EcsPool<Mass> _mass;
        [EcsPool] private EcsPool<Go> _go;
        [EcsPool] private EcsPool<PushToScene> _push;
        [EcsPool] private EcsPool<BallType> _ballType;
        [EcsPool] private EcsPool<Position> _pos;
        [EcsFilter(typeof(PushToScene))] private EcsFilter _pushes;

        public void Run(EcsSystems systems)
        {
            foreach (int entity in _pushes)
            {
                GameObject ball = _go.Get(entity).go;
                if (_push.Get(entity).requestCount > 0)
                {
                    ball.transform.localScale = Vector3.one * _mass.Get(entity).CalcBallDiameter(_config) * _config.Platform().visualRadiusMultiplier;
                    ball.GetComponent<Renderer>().material.color = _ballType.Get(entity).config.color;
                    _push.Get(entity).requestCount = 0;
                }

                ball.transform.localPosition = _pos.Get(entity).position;
            }
        }
    }
}