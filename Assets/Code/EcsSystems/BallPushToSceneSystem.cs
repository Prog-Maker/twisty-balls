using Code.EcsComponents;
using Code.Extensions;
using Code.SO;
using Kk.LeoQuery;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.EcsSystems
{
    public class BallPushToSceneSystem : ISystem
    {
        [Inject]
        public Config config;

        private EcsPool<Mass> _mass;
        private EcsPool<Go> _go;
        private EcsPool<PushToScene> _push;
        private EcsFilter _pushes;
        private EcsPool<BallType> _ballType;
        private EcsPool<Position> _pos;

        public BallPushToSceneSystem(EcsWorld world)
        {
            _pushes = world.Filter<PushToScene>().End();
            _push = world.GetPool<PushToScene>();
            _mass = world.GetPool<Mass>();
            _go = world.GetPool<Go>();
            _ballType = world.GetPool<BallType>();
            _pos = world.GetPool<Position>();
        }

        public void Act(IEntityStorage storage)
        {
            foreach (int entity in _pushes)
            {
                
                GameObject ball = _go.Get(entity).go;
                if (_push.Get(entity).requestCount > 0)
                {
                    ball.transform.localScale = Vector3.one * _mass.Get(entity).CalcBallDiameter(config);
                    ball.GetComponent<Renderer>().material.color = _ballType.Get(entity).config.color;
                }

                ball.transform.localPosition = _pos.Get(entity).position;
            }
        }
    }
}