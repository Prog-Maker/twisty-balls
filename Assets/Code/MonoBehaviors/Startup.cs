using System;
using System.Collections.Generic;
using Code.EcsComponents;
using Code.Phases;
using Code.SO;
using Kk.BusyEcs;
using Kk.LeoHot;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code.MonoBehaviors
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private SerializableEcsUniverse stash;

        public Config config;

        private Leopotam.EcsLite.EcsSystems _ecsSystems;

        // private IEcsContainer _ecs;
        private IEcsContainer _ecs;
        
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void ConfigureBusyEcs()
        {
            BusyEcs.SkipIterationCheck = true;
            BusyEcs.SetUserAssemblies(typeof(Startup).Assembly);
            BusyEcs.SystemOrderDumpFile = "Assets/Code/EcsSystems/order.lock.yaml";
            BusyEcs.SystemsOrder = systems =>
            {
                List<string> canonicalOrder = new List<string>
                {
                    "UserControlsSystem",
                    "StartGameSystem",
                    "BallInitSystem",
                    "RestartGameSystem",
                    "PhysicsSystem",
                    "CriticalMassExplosionSystem",
                    "BallDestroySystem",
                    "BallPushToSceneSystem",
                };

                Array.Sort(systems, (a, b) =>
                    canonicalOrder.IndexOf(a.DeclaringType.Name)
                    - canonicalOrder.IndexOf(b.DeclaringType.Name));
            };
        }

        private void OnEnable()
        {
            _ecsSystems = new Leopotam.EcsLite.EcsSystems(new EcsWorld());
            _ecsSystems.AddWorld(new EcsWorld(), "events");

            _ecs = new EcsContainerBuilder(EcsMetadata.ScanProject())
                .AddInjectable(config)
                .Integrate(_ecsSystems)
                .Create();

            InitEcsDebugger();
            _ecsSystems.Init();

            stash ??= new SerializableEcsUniverse();

            stash.AddConverter<EntityRef, int>(
                (runtime, ctx) =>
                {
                    if (!runtime.Deref(out Entity entity))
                    {
                        return 0;
                    }

                    entity.GetRaw(out EcsWorld world, out var id);

                    TempEntityKey tempEntityKey = new TempEntityKey(ctx.worldToName[world], id);
                    return ctx.entityToPackedId[tempEntityKey];
                },
                (persistent, ctx) =>
                {
                    if (persistent == 0)
                    {
                        return default;
                    }

                    TempEntityKey tempEntity = ctx.entityByPackedId[persistent];
                    return new Entity(ctx.worldByName[tempEntity.world ?? ""], tempEntity.entity).AsRef();
                }
            );

            stash.AddIncomingLink<EntityLink, EntityRef>(
                link => link.entity,
                (link, entity) => link.entity = entity
            );

            stash.UnpackState(_ecsSystems);
        }

        private void InitEcsDebugger()
        {
#if UNITY_EDITOR
            _ecsSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            foreach (var pair in _ecsSystems.GetAllNamedWorlds())
            {
                _ecsSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(pair.Key));
            }
#endif
        }

        private void Start()
        {
            _ecs?.NewEntity(new StartGameCommand());
        }

        private void Update()
        {
            _ecs?.Execute<EarlyUpdate>();
            _ecs?.Execute<Update>();
            _ecs?.Execute<LateUpdate>();
            _ecs?.Execute<Visualize>();
            // _ecsSystems.Run();
        }

        private void OnDrawGizmos()
        {
            _ecs?.Execute<DrawGizmos>();
        }

        private void OnGUI()
        {
            _ecs?.Execute<OnGUI>();
        }

        private void OnDisable()
        {
            stash.PackState(_ecsSystems);
            _ecsSystems.Destroy();
        }
    }
}