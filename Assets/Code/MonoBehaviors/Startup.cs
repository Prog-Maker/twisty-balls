using System;
using Code.EcsComponents;
using Code.EcsSystems;
using Code.MonoBehaviors;
using Code.SO;
using Kk.LeoHot;
using Kk.LeoQuery;
using Leopotam.EcsLite;
using Leopotam.EcsLite.UnityEditor;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [SerializeField] private SerializableEcsUniverse stash;

    public Config config;

    private LeoLiteStorage _storage;
    private ISystem _update;
    private ISystem _gui;

    private EcsSystems _debugger;

    private void OnEnable()
    {
        _storage = new LeoLiteStorage();

        Injector injector = new Injector()
            .Add(config);

        InitEcsDebugger(_storage);

        _update = new MulticastSystem()
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
                .ForEach(injector.InjectInto)
            ;

        _gui = new MulticastSystem()
                .Add(new HUDSystem())
                .ForEach(injector.InjectInto)
            ;

        stash ??= new SerializableEcsUniverse();

        stash.AddConverter<Entity, int>(
            (runtime, ctx) =>
            {
                if (!runtime.raw.Unpack(out EcsWorld world, out int entity))
                {
                    return 0;
                }

                TempEntityKey tempEntityKey = new TempEntityKey(ctx.worldToName[world], entity);
                return ctx.entityToPackedId[tempEntityKey];
            },
            (persistent, ctx) =>
            {
                if (persistent == 0)
                {
                    return default;
                }

                TempEntityKey tempEntity = ctx.entityByPackedId[persistent];
                return new Entity(ctx.worldByName[tempEntity.world ?? ""].PackEntityWithWorld(tempEntity.entity));
            }
        );

        stash.AddIncomingLink<EntityLink, Entity>(
            link => link.entity,
            (link, entity) => link.entity = entity
        );

        // debugger has all worlds
        stash.UnpackState(_debugger);
    }

    private void InitEcsDebugger(LeoLiteStorage storage)
    {
        EcsWorld[] rawWorlds = null;
        storage.GetWorlds(ref rawWorlds);

        _debugger = new EcsSystems(rawWorlds[0]);
        _debugger.Add(new EcsWorldDebugSystem());
        for (var i = 1; i < rawWorlds.Length; i++)
        {
            var worldName = i.ToString();
            _debugger.AddWorld(rawWorlds[i], worldName);
            _debugger.Add(new EcsWorldDebugSystem(worldName));
        }

        _debugger.Init();
    }

    private void Start()
    {
        _storage.NewEntity().Add<StartGameCommand>();
    }

    private void Update()
    {
        _debugger.Run();
        _update.Act(_storage);
    }

    private void OnGUI()
    {
        _gui.Act(_storage);
    }

    private void OnDisable()
    {
        stash.PackState(_debugger);
        _debugger.Destroy();
    }
}