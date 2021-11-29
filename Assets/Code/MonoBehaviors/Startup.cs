using Code.EcsComponents;
using Code.SO;
using Code.Systems;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif

namespace Code.MonoBehaviors
{
    public class Startup : MonoBehaviour
    {
        public Config config;

        private EcsWorld _ecsWorld;
        private EcsSystems _update;
        private EcsSystems _gui;

        private void Start()
        {
            _ecsWorld = new EcsWorld();

            _update = new EcsSystems(_ecsWorld, config)
                    // input
                    .Add(new UserControlsSystem())
                    // update
                    .Add(new StartGameSystem())
                    .Add(new BallInitSystem())
                    .Add(new RestartGameSystem())
                    .Add(new PhysicsSystem())
                    .Add(new CriticalMassExplosionSystem())
                    .Add(new BallDestroySystem())
                    // visualize
                    .Add(new BallPushToSceneSystem())
#if UNITY_EDITOR
                    .Add(new EcsWorldDebugSystem())
#endif
                    .Inject(config)
                ;
            _update.Init();

            _gui = new EcsSystems(_ecsWorld, config)
                    .Add(new HUDSystem())
#if UNITY_EDITOR
                    .Add(new EcsWorldDebugSystem())
#endif
                    .Inject()
                ;
            _gui.Init();
            
            _ecsWorld.GetPool<StartGameCommand>().Add(_ecsWorld.NewEntity());
        }

        private void Update()
        {
            _update.Run();
        }

        private void OnGUI()
        {
            _gui.Run();
        }

        private void OnDestroy()
        {
            _update.Destroy();
            _gui.Destroy();
        }
    }
}