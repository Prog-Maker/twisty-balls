using System.Linq;
using Code.EcsComponents;
using Code.EcsSystems;
using Code.MonoBehaviors;
using Code.Phases;
using Code.SO;
using Kk.BusyEcs;
using Kk.LeoHot;
using Leopotam.EcsLite;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [SerializeField] private SerializableEcsUniverse stash;

    public Config config;

    // private LeoLiteStorage _storage;
    // private ISystem _update;
    // private ISystem _gui;

    private EcsSystems _ecsSystems;
    private IEcsContainer _ecs;

    private void OnEnable()
    {
        // _storage = new LeoLiteStorage();

        _ecsSystems = new EcsSystems(new EcsWorld());
        _ecsSystems.AddWorld(new EcsWorld(), "events");
        
        _ecs = new EcsContainerBuilder()
            .Scan(typeof(Startup).Assembly)
            // .ScanTypes(Resources.Load<ManualAssembly>("Scripts").scripts.Select(it => it.GetClass()))
            .Integrate(_ecsSystems)
            .Injectable(config)
            .End();

        InitEcsDebugger();
        _ecsSystems.Init();

        // _update = new MulticastSystem()
        //         // input
        //         .Add(new UserControlsSystem())
        //         // update
        //         .Add(new StartGameSystem())
        //         .Add(new BallInitSystem())
        //         .Add(new RestartGameSystem())
        //         .Add(new PhysicsSystem())
        //         .Add(new CriticalMassExplosionSystem())
        //         .Add(new BallDestroySystem())
        //         // visualize
        //         .Add(new BallPushToSceneSystem())
                // .ForEach(injector.InjectInto)
            // ;

        // _gui = new MulticastSystem()
        //         .Add(new HUDSystem())
        //         .ForEach(injector.InjectInto)
            ;

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

        // debugger has all worlds
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
        _ecs.NewEntity(new StartGameCommand());
    }

    private void Update()
    {
        _ecs.Execute<EarlyUpdate>();
        _ecs.Execute<Update>();
        _ecs.Execute<LateUpdate>();
        _ecs.Execute<Visualize>();
        // _ecsSystems.Run();
    }

    private void OnGUI()
    {
        _ecs.Execute<OnGUI>();
    }

    private void OnDisable()
    {
        stash.PackState(_ecsSystems);
        _ecsSystems.Destroy();
    }
}