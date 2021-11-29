using Code.EcsComponents;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Systems
{
    public class BallDestroySystem: IEcsRunSystem
    {
        [EcsFilter(typeof(BallDestroyAction))] private EcsFilter _ballDestroys;
        [EcsPool]private EcsPool<Go> _go;

        public void Run(EcsSystems systems)
        {
            foreach (int entity in _ballDestroys)
            {
                
                Object.Destroy(_go.Get(entity).go);
                systems.GetWorld().DelEntity(entity);
            }
        }
    }
}