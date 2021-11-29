using Code.EcsComponents;
using Code.MonoBehaviors;
using Code.SO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Systems
{
    public class BallInitSystem : IEcsRunSystem
    {
        [EcsShared] private Config _config;
        [EcsFilter(typeof(BallInitAction))] private EcsFilter _ballInitActions;
        [EcsPool] private EcsPool<BallInitAction> _ballInitAction;
        [EcsPool] private EcsPool<Position> _position;
        [EcsPool] private EcsPool<Velocity> _velocity;
        [EcsPool] private EcsPool<Mass> _mass;
        [EcsPool] private EcsPool<BallType> _ballType;
        [EcsPool] private EcsPool<Go> _go;
        [EcsPool] private EcsPool<PushToScene> _pushToScene;


        public void Run(EcsSystems systems)
        {
            foreach (int entity in _ballInitActions)
            {
                BallInitAction initAction = _ballInitAction.Get(entity);
                _position.Add(entity).position = initAction.position;
                _velocity.Add(entity).velocity = initAction.direction * initAction.speed;
                _mass.Add(entity).mass = initAction.mass;
                _ballType.Add(entity) = new BallType(initAction.config);
                GameObject go = Object.Instantiate(_config.ballPrefab);
                go.name = initAction.name;
                go.AddComponent<EntityLink>().entity = systems.GetWorld().PackEntityWithWorld(entity);
                _go.Add(entity) = new Go(go);
                _pushToScene.Add(entity).requestCount++;
                
                _ballInitAction.Del(entity);
            }
        }
    }
}