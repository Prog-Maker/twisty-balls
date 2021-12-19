using System;
using System.Linq;
using System.Collections.Generic;
using Kk.BusyEcs;
using Kk.BusyEcs.Internal;
using Leopotam.EcsLite;
using System.Reflection;
[UnityEngine.Scripting.Preserve]
public class GeneratedEcsContainer : Kk.BusyEcs.Internal.IConfigurableEcsContainer {
  private readonly Dictionary<Type, Action> _phaseExecutionByType = new Dictionary<Type, Action>();
  private EcsSystems worlds;
  private Dictionary<Type, object> injectables = new Dictionary<Type, object>();
  private EcsWorld[] allWorlds;
  private Code.EcsSystems.BallDestroySystem _BallDestroySystem;
  private Code.EcsSystems.BallInitSystem _BallInitSystem;
  private Code.EcsSystems.BallPushToSceneSystem _BallPushToSceneSystem;
  private Code.EcsSystems.CriticalMassExplosionSystem _CriticalMassExplosionSystem;
  private Code.EcsSystems.HUDSystem _HUDSystem;
  private Code.EcsSystems.PhysicsSystem _PhysicsSystem;
  private Code.EcsSystems.RestartGameSystem _RestartGameSystem;
  private Code.EcsSystems.StartGameSystem _StartGameSystem;
  private Code.EcsSystems.UserControlsSystem _UserControlsSystem;
  private EcsWorld defaultWorld;
  private EcsPool<Code.EcsComponents.Velocity> pool_defaultWorld_Velocity;
  private EcsPool<Code.EcsComponents.Position> pool_defaultWorld_Position;
  private EcsPool<Code.EcsComponents.BallType> pool_defaultWorld_BallType;
  private EcsPool<Code.EcsComponents.BallInitAction> pool_defaultWorld_BallInitAction;
  private EcsPool<Code.EcsComponents.StartGameCommand> pool_defaultWorld_StartGameCommand;
  private EcsPool<Code.EcsComponents.BallDestroyAction> pool_defaultWorld_BallDestroyAction;
  private EcsPool<Code.EcsComponents.Mass> pool_defaultWorld_Mass;
  private EcsPool<Code.EcsComponents.RestartGameCommand> pool_defaultWorld_RestartGameCommand;
  private EcsPool<Code.EcsComponents.PushToScene> pool_defaultWorld_PushToScene;
  private EcsPool<Code.EcsComponents.Go> pool_defaultWorld_Go;
  private EcsFilter filter_defaultWorld_BallType_Position_Velocity;
  private EcsFilter filter_defaultWorld_BallInitAction;
  private EcsFilter filter_defaultWorld_StartGameCommand;
  private EcsFilter filter_defaultWorld_BallDestroyAction;
  private EcsFilter filter_defaultWorld_Mass_Position;
  private EcsFilter filter_defaultWorld_RestartGameCommand;
  private EcsFilter filter_defaultWorld_Go_PushToScene;
  private EcsWorld events;
  private EcsPool<Code.EcsComponents.Velocity> pool_events_Velocity;
  private EcsPool<Code.EcsComponents.Position> pool_events_Position;
  private EcsPool<Code.EcsComponents.BallType> pool_events_BallType;
  private EcsPool<Code.EcsComponents.BallInitAction> pool_events_BallInitAction;
  private EcsPool<Code.EcsComponents.StartGameCommand> pool_events_StartGameCommand;
  private EcsPool<Code.EcsComponents.BallDestroyAction> pool_events_BallDestroyAction;
  private EcsPool<Code.EcsComponents.Mass> pool_events_Mass;
  private EcsPool<Code.EcsComponents.RestartGameCommand> pool_events_RestartGameCommand;
  private EcsPool<Code.EcsComponents.PushToScene> pool_events_PushToScene;
  private EcsPool<Code.EcsComponents.Go> pool_events_Go;
  private EcsFilter filter_events_BallType_Position_Velocity;
  private EcsFilter filter_events_BallInitAction;
  private EcsFilter filter_events_StartGameCommand;
  private EcsFilter filter_events_BallDestroyAction;
  private EcsFilter filter_events_Mass_Position;
  private EcsFilter filter_events_RestartGameCommand;
  private EcsFilter filter_events_Go_PushToScene;
  public GeneratedEcsContainer() {
    _phaseExecutionByType[typeof(Code.Phases.DrawGizmos)] = () => {
      foreach (var entity0 in filter_defaultWorld_BallType_Position_Velocity) {
        _PhysicsSystem.DrawGizmos(ref pool_defaultWorld_Velocity.Get(entity0), ref pool_defaultWorld_Position.Get(entity0), ref pool_defaultWorld_BallType.Get(entity0));
      }
      foreach (var entity1 in filter_events_BallType_Position_Velocity) {
        _PhysicsSystem.DrawGizmos(ref pool_events_Velocity.Get(entity1), ref pool_events_Position.Get(entity1), ref pool_events_BallType.Get(entity1));
      }
    };
    _phaseExecutionByType[typeof(Code.Phases.EarlyUpdate)] = () => {
      _UserControlsSystem.Act();
      foreach (var entity2 in filter_defaultWorld_StartGameCommand) {
        _StartGameSystem.Act(new Entity(defaultWorld, entity2), pool_defaultWorld_StartGameCommand.Get(entity2));
      }
      foreach (var entity3 in filter_events_StartGameCommand) {
        _StartGameSystem.Act(new Entity(events, entity3), pool_events_StartGameCommand.Get(entity3));
      }
      foreach (var entity4 in filter_defaultWorld_BallInitAction) {
        _BallInitSystem.Act(new Entity(defaultWorld, entity4), pool_defaultWorld_BallInitAction.Get(entity4));
      }
      foreach (var entity5 in filter_events_BallInitAction) {
        _BallInitSystem.Act(new Entity(events, entity5), pool_events_BallInitAction.Get(entity5));
      }
    };
    _phaseExecutionByType[typeof(Code.Phases.Init)] = () => {
      _PhysicsSystem.Init();
    };
    _phaseExecutionByType[typeof(Code.Phases.LateUpdate)] = () => {
      foreach (var entity6 in filter_defaultWorld_BallDestroyAction) {
        _BallDestroySystem.Act(new Entity(defaultWorld, entity6), pool_defaultWorld_BallDestroyAction.Get(entity6));
      }
      foreach (var entity7 in filter_events_BallDestroyAction) {
        _BallDestroySystem.Act(new Entity(events, entity7), pool_events_BallDestroyAction.Get(entity7));
      }
    };
    _phaseExecutionByType[typeof(Code.Phases.OnGUI)] = () => {
      _HUDSystem.Act();
    };
    _phaseExecutionByType[typeof(Code.Phases.Update)] = () => {
      foreach (var entity8 in filter_defaultWorld_RestartGameCommand) {
        _RestartGameSystem.Act(new Entity(defaultWorld, entity8), pool_defaultWorld_RestartGameCommand.Get(entity8));
      }
      foreach (var entity9 in filter_events_RestartGameCommand) {
        _RestartGameSystem.Act(new Entity(events, entity9), pool_events_RestartGameCommand.Get(entity9));
      }
      _PhysicsSystem.Act();
      foreach (var entity10 in filter_defaultWorld_Mass_Position) {
        _CriticalMassExplosionSystem.Act(new Entity(defaultWorld, entity10), ref pool_defaultWorld_Mass.Get(entity10), ref pool_defaultWorld_Position.Get(entity10));
      }
      foreach (var entity11 in filter_events_Mass_Position) {
        _CriticalMassExplosionSystem.Act(new Entity(events, entity11), ref pool_events_Mass.Get(entity11), ref pool_events_Position.Get(entity11));
      }
    };
    _phaseExecutionByType[typeof(Code.Phases.Visualize)] = () => {
      foreach (var entity12 in filter_defaultWorld_Go_PushToScene) {
        _BallPushToSceneSystem.Act(new Entity(defaultWorld, entity12), pool_defaultWorld_PushToScene.Get(entity12), pool_defaultWorld_Go.Get(entity12));
      }
      foreach (var entity13 in filter_events_Go_PushToScene) {
        _BallPushToSceneSystem.Act(new Entity(events, entity13), pool_events_PushToScene.Get(entity13), pool_events_Go.Get(entity13));
      }
    };
  }
  public void Init(EcsSystems worlds) {
    this.worlds = worlds;
    _BallDestroySystem = new Code.EcsSystems.BallDestroySystem ();
    _BallInitSystem = new Code.EcsSystems.BallInitSystem ();
    _BallPushToSceneSystem = new Code.EcsSystems.BallPushToSceneSystem ();
    _CriticalMassExplosionSystem = new Code.EcsSystems.CriticalMassExplosionSystem ();
    _HUDSystem = new Code.EcsSystems.HUDSystem ();
    _PhysicsSystem = new Code.EcsSystems.PhysicsSystem ();
    _RestartGameSystem = new Code.EcsSystems.RestartGameSystem ();
    _StartGameSystem = new Code.EcsSystems.StartGameSystem ();
    _UserControlsSystem = new Code.EcsSystems.UserControlsSystem ();
    allWorlds = new EcsWorld[2];
    defaultWorld = worlds.GetWorld();
    allWorlds[0] = defaultWorld;
    pool_defaultWorld_Velocity = defaultWorld.GetPool<Code.EcsComponents.Velocity>();
    pool_defaultWorld_Position = defaultWorld.GetPool<Code.EcsComponents.Position>();
    pool_defaultWorld_BallType = defaultWorld.GetPool<Code.EcsComponents.BallType>();
    pool_defaultWorld_BallInitAction = defaultWorld.GetPool<Code.EcsComponents.BallInitAction>();
    pool_defaultWorld_StartGameCommand = defaultWorld.GetPool<Code.EcsComponents.StartGameCommand>();
    pool_defaultWorld_BallDestroyAction = defaultWorld.GetPool<Code.EcsComponents.BallDestroyAction>();
    pool_defaultWorld_Mass = defaultWorld.GetPool<Code.EcsComponents.Mass>();
    pool_defaultWorld_RestartGameCommand = defaultWorld.GetPool<Code.EcsComponents.RestartGameCommand>();
    pool_defaultWorld_PushToScene = defaultWorld.GetPool<Code.EcsComponents.PushToScene>();
    pool_defaultWorld_Go = defaultWorld.GetPool<Code.EcsComponents.Go>();
    filter_defaultWorld_BallType_Position_Velocity = defaultWorld.Filter<Code.EcsComponents.BallType>().Inc<Code.EcsComponents.Position>().Inc<Code.EcsComponents.Velocity>().End();
    filter_defaultWorld_BallInitAction = defaultWorld.Filter<Code.EcsComponents.BallInitAction>().End();
    filter_defaultWorld_StartGameCommand = defaultWorld.Filter<Code.EcsComponents.StartGameCommand>().End();
    filter_defaultWorld_BallDestroyAction = defaultWorld.Filter<Code.EcsComponents.BallDestroyAction>().End();
    filter_defaultWorld_Mass_Position = defaultWorld.Filter<Code.EcsComponents.Mass>().Inc<Code.EcsComponents.Position>().End();
    filter_defaultWorld_RestartGameCommand = defaultWorld.Filter<Code.EcsComponents.RestartGameCommand>().End();
    filter_defaultWorld_Go_PushToScene = defaultWorld.Filter<Code.EcsComponents.Go>().Inc<Code.EcsComponents.PushToScene>().End();
    if (worlds.GetWorld("events") == null) { worlds.AddWorld(new EcsWorld(), "events"); }
    events = worlds.GetWorld("events");
    allWorlds[1] = events;
    pool_events_Velocity = events.GetPool<Code.EcsComponents.Velocity>();
    pool_events_Position = events.GetPool<Code.EcsComponents.Position>();
    pool_events_BallType = events.GetPool<Code.EcsComponents.BallType>();
    pool_events_BallInitAction = events.GetPool<Code.EcsComponents.BallInitAction>();
    pool_events_StartGameCommand = events.GetPool<Code.EcsComponents.StartGameCommand>();
    pool_events_BallDestroyAction = events.GetPool<Code.EcsComponents.BallDestroyAction>();
    pool_events_Mass = events.GetPool<Code.EcsComponents.Mass>();
    pool_events_RestartGameCommand = events.GetPool<Code.EcsComponents.RestartGameCommand>();
    pool_events_PushToScene = events.GetPool<Code.EcsComponents.PushToScene>();
    pool_events_Go = events.GetPool<Code.EcsComponents.Go>();
    filter_events_BallType_Position_Velocity = events.Filter<Code.EcsComponents.BallType>().Inc<Code.EcsComponents.Position>().Inc<Code.EcsComponents.Velocity>().End();
    filter_events_BallInitAction = events.Filter<Code.EcsComponents.BallInitAction>().End();
    filter_events_StartGameCommand = events.Filter<Code.EcsComponents.StartGameCommand>().End();
    filter_events_BallDestroyAction = events.Filter<Code.EcsComponents.BallDestroyAction>().End();
    filter_events_Mass_Position = events.Filter<Code.EcsComponents.Mass>().Inc<Code.EcsComponents.Position>().End();
    filter_events_RestartGameCommand = events.Filter<Code.EcsComponents.RestartGameCommand>().End();
    filter_events_Go_PushToScene = events.Filter<Code.EcsComponents.Go>().Inc<Code.EcsComponents.PushToScene>().End();
    AddInjectable(this, typeof(IEnv));
    typeof(Entity).GetField("env", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, this);
    WorldsKeeper.worlds = Enumerable.Empty<EcsWorld>().Append(worlds.GetWorld()).Concat(worlds.GetAllNamedWorlds().Values).ToArray();
    _BallInitSystem.config = (Code.SO.Config) ResolveInjectable<Code.SO.Config>();
    _BallPushToSceneSystem.config = (Code.SO.Config) ResolveInjectable<Code.SO.Config>();
    _CriticalMassExplosionSystem.config = (Code.SO.Config) ResolveInjectable<Code.SO.Config>();
    _CriticalMassExplosionSystem.env = (Kk.BusyEcs.IEnv) ResolveInjectable<Kk.BusyEcs.IEnv>();
    _HUDSystem.env = (Kk.BusyEcs.IEnv) ResolveInjectable<Kk.BusyEcs.IEnv>();
    _HUDSystem.config = (Code.SO.Config) ResolveInjectable<Code.SO.Config>();
    _PhysicsSystem.config = (Code.SO.Config) ResolveInjectable<Code.SO.Config>();
    _PhysicsSystem.env = (Kk.BusyEcs.IEnv) ResolveInjectable<Kk.BusyEcs.IEnv>();
    _RestartGameSystem.env = (Kk.BusyEcs.IEnv) ResolveInjectable<Kk.BusyEcs.IEnv>();
    _StartGameSystem.config = (Code.SO.Config) ResolveInjectable<Code.SO.Config>();
    _StartGameSystem.env = (Kk.BusyEcs.IEnv) ResolveInjectable<Kk.BusyEcs.IEnv>();
    _UserControlsSystem.env = (Kk.BusyEcs.IEnv) ResolveInjectable<Kk.BusyEcs.IEnv>();
  }
  public void Execute<T>() where T : Attribute {
    _phaseExecutionByType[typeof(T)]();
  }
  public void AddInjectable(object injectable, Type overrideType = null)
  {
    injectables[overrideType ?? injectable.GetType()] = injectable;
  }
  private object ResolveInjectable<T>()
  {
    if (!injectables.TryGetValue(typeof(T), out var injectable))
    {
        throw new Exception("failed to resolve injection of " + typeof(T).FullName);
    }
    
    return injectable;
  }
  public Entity NewEntity<T1>(in T1 c1) 
    where T1 : struct 
  {
    EcsWorld w = worlds.GetWorld(WorldName<T1>.worldName);
    var id = w.NewEntity();
    w.GetPool<T1>().Add(id) = c1;
    return new Entity(w, id);
  }
  public Entity NewEntity<T1, T2>(in T1 c1, in T2 c2) 
    where T1 : struct 
    where T2 : struct 
  {
    EcsWorld w = worlds.GetWorld(WorldName<T1, T2>.worldName);
    var id = w.NewEntity();
    w.GetPool<T1>().Add(id) = c1;
    w.GetPool<T2>().Add(id) = c2;
    return new Entity(w, id);
  }
  public Entity NewEntity<T1, T2, T3>(in T1 c1, in T2 c2, in T3 c3) 
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    EcsWorld w = worlds.GetWorld(WorldName<T1, T2, T3>.worldName);
    var id = w.NewEntity();
    w.GetPool<T1>().Add(id) = c1;
    w.GetPool<T2>().Add(id) = c2;
    w.GetPool<T3>().Add(id) = c3;
    return new Entity(w, id);
  }
  public Entity NewEntity<T1, T2, T3, T4>(in T1 c1, in T2 c2, in T3 c3, in T4 c4) 
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    EcsWorld w = worlds.GetWorld(WorldName<T1, T2, T3, T4>.worldName);
    var id = w.NewEntity();
    w.GetPool<T1>().Add(id) = c1;
    w.GetPool<T2>().Add(id) = c2;
    w.GetPool<T3>().Add(id) = c3;
    w.GetPool<T4>().Add(id) = c4;
    return new Entity(w, id);
  }
  public Entity NewEntity<T1, T2, T3, T4, T5>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5) 
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
    where T5 : struct 
  {
    EcsWorld w = worlds.GetWorld(WorldName<T1, T2, T3, T4, T5>.worldName);
    var id = w.NewEntity();
    w.GetPool<T1>().Add(id) = c1;
    w.GetPool<T2>().Add(id) = c2;
    w.GetPool<T3>().Add(id) = c3;
    w.GetPool<T4>().Add(id) = c4;
    w.GetPool<T5>().Add(id) = c5;
    return new Entity(w, id);
  }
  public Entity NewEntity<T1, T2, T3, T4, T5, T6>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6) 
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
    where T5 : struct 
    where T6 : struct 
  {
    EcsWorld w = worlds.GetWorld(WorldName<T1, T2, T3, T4, T5, T6>.worldName);
    var id = w.NewEntity();
    w.GetPool<T1>().Add(id) = c1;
    w.GetPool<T2>().Add(id) = c2;
    w.GetPool<T3>().Add(id) = c3;
    w.GetPool<T4>().Add(id) = c4;
    w.GetPool<T5>().Add(id) = c5;
    w.GetPool<T6>().Add(id) = c6;
    return new Entity(w, id);
  }
  public Entity NewEntity<T1, T2, T3, T4, T5, T6, T7>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7) 
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
    where T5 : struct 
    where T6 : struct 
    where T7 : struct 
  {
    EcsWorld w = worlds.GetWorld(WorldName<T1, T2, T3, T4, T5, T6, T7>.worldName);
    var id = w.NewEntity();
    w.GetPool<T1>().Add(id) = c1;
    w.GetPool<T2>().Add(id) = c2;
    w.GetPool<T3>().Add(id) = c3;
    w.GetPool<T4>().Add(id) = c4;
    w.GetPool<T5>().Add(id) = c5;
    w.GetPool<T6>().Add(id) = c6;
    w.GetPool<T7>().Add(id) = c7;
    return new Entity(w, id);
  }
  public Entity NewEntity<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8) 
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
    where T5 : struct 
    where T6 : struct 
    where T7 : struct 
    where T8 : struct 
  {
    EcsWorld w = worlds.GetWorld(WorldName<T1, T2, T3, T4, T5, T6, T7, T8>.worldName);
    var id = w.NewEntity();
    w.GetPool<T1>().Add(id) = c1;
    w.GetPool<T2>().Add(id) = c2;
    w.GetPool<T3>().Add(id) = c3;
    w.GetPool<T4>().Add(id) = c4;
    w.GetPool<T5>().Add(id) = c5;
    w.GetPool<T6>().Add(id) = c6;
    w.GetPool<T7>().Add(id) = c7;
    w.GetPool<T8>().Add(id) = c8;
    return new Entity(w, id);
  }
  public void Query<T1>(SimpleCallback0<T1> callback)
    where T1 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().End();
      foreach (var id in filter) {
        callback(PoolKeeper<T1>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1>(EntityCallback0<T1> callback)
    where T1 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), PoolKeeper<T1>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1>(SimpleCallback1<T1> callback)
    where T1 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1>(EntityCallback1<T1> callback)
    where T1 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2>(SimpleCallback0<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().End();
      foreach (var id in filter) {
        callback(PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2>(EntityCallback0<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2>(SimpleCallback1<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2>(EntityCallback1<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2>(SimpleCallback2<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2>(EntityCallback2<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3>(SimpleCallback0<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().End();
      foreach (var id in filter) {
        callback(PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3>(EntityCallback0<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3>(SimpleCallback1<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3>(EntityCallback1<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3>(SimpleCallback2<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3>(EntityCallback2<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3>(SimpleCallback3<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), ref PoolKeeper<T3>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3>(EntityCallback3<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), ref PoolKeeper<T3>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(SimpleCallback0<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id), PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(EntityCallback0<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id), PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(SimpleCallback1<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id), PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(EntityCallback1<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id), PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(SimpleCallback2<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id), PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(EntityCallback2<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), PoolKeeper<T3>.byWorld[wi].Get(id), PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(SimpleCallback3<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), ref PoolKeeper<T3>.byWorld[wi].Get(id), PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(EntityCallback3<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), ref PoolKeeper<T3>.byWorld[wi].Get(id), PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(SimpleCallback4<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), ref PoolKeeper<T3>.byWorld[wi].Get(id), ref PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public void Query<T1, T2, T3, T4>(EntityCallback4<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    for (int wi = 0; wi < allWorlds.Length; wi++) {
      EcsWorld w = allWorlds[wi];
      EcsFilter filter = w.Filter<T1>().Inc<T2>().Inc<T3>().Inc<T4>().End();
      foreach (var id in filter) {
        callback(new Entity(w, id), ref PoolKeeper<T1>.byWorld[wi].Get(id), ref PoolKeeper<T2>.byWorld[wi].Get(id), ref PoolKeeper<T3>.byWorld[wi].Get(id), ref PoolKeeper<T4>.byWorld[wi].Get(id));
      }
    }
  }
  public bool Match<T1>(Entity entity, SimpleCallback0<T1> callback)
    where T1 : struct 
  {
    if (!entity.Has<T1>()) return false;
    callback(entity.Get<T1>());
    return true;
  }
  public bool Match<T1>(Entity entity, EntityCallback0<T1> callback)
    where T1 : struct 
  {
    if (!entity.Has<T1>()) return false;
    callback(entity, entity.Get<T1>());
    return true;
  }
  public bool Match<T1>(Entity entity, SimpleCallback1<T1> callback)
    where T1 : struct 
  {
    if (!entity.Has<T1>()) return false;
    callback(ref entity.Get<T1>());
    return true;
  }
  public bool Match<T1>(Entity entity, EntityCallback1<T1> callback)
    where T1 : struct 
  {
    if (!entity.Has<T1>()) return false;
    callback(entity, ref entity.Get<T1>());
    return true;
  }
  public bool Match<T1, T2>(Entity entity, SimpleCallback0<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    callback(entity.Get<T1>(), entity.Get<T2>());
    return true;
  }
  public bool Match<T1, T2>(Entity entity, EntityCallback0<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    callback(entity, entity.Get<T1>(), entity.Get<T2>());
    return true;
  }
  public bool Match<T1, T2>(Entity entity, SimpleCallback1<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    callback(ref entity.Get<T1>(), entity.Get<T2>());
    return true;
  }
  public bool Match<T1, T2>(Entity entity, EntityCallback1<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    callback(entity, ref entity.Get<T1>(), entity.Get<T2>());
    return true;
  }
  public bool Match<T1, T2>(Entity entity, SimpleCallback2<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    callback(ref entity.Get<T1>(), ref entity.Get<T2>());
    return true;
  }
  public bool Match<T1, T2>(Entity entity, EntityCallback2<T1, T2> callback)
    where T1 : struct 
    where T2 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    callback(entity, ref entity.Get<T1>(), ref entity.Get<T2>());
    return true;
  }
  public bool Match<T1, T2, T3>(Entity entity, SimpleCallback0<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    callback(entity.Get<T1>(), entity.Get<T2>(), entity.Get<T3>());
    return true;
  }
  public bool Match<T1, T2, T3>(Entity entity, EntityCallback0<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    callback(entity, entity.Get<T1>(), entity.Get<T2>(), entity.Get<T3>());
    return true;
  }
  public bool Match<T1, T2, T3>(Entity entity, SimpleCallback1<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    callback(ref entity.Get<T1>(), entity.Get<T2>(), entity.Get<T3>());
    return true;
  }
  public bool Match<T1, T2, T3>(Entity entity, EntityCallback1<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    callback(entity, ref entity.Get<T1>(), entity.Get<T2>(), entity.Get<T3>());
    return true;
  }
  public bool Match<T1, T2, T3>(Entity entity, SimpleCallback2<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    callback(ref entity.Get<T1>(), ref entity.Get<T2>(), entity.Get<T3>());
    return true;
  }
  public bool Match<T1, T2, T3>(Entity entity, EntityCallback2<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    callback(entity, ref entity.Get<T1>(), ref entity.Get<T2>(), entity.Get<T3>());
    return true;
  }
  public bool Match<T1, T2, T3>(Entity entity, SimpleCallback3<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    callback(ref entity.Get<T1>(), ref entity.Get<T2>(), ref entity.Get<T3>());
    return true;
  }
  public bool Match<T1, T2, T3>(Entity entity, EntityCallback3<T1, T2, T3> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    callback(entity, ref entity.Get<T1>(), ref entity.Get<T2>(), ref entity.Get<T3>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback0<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(entity.Get<T1>(), entity.Get<T2>(), entity.Get<T3>(), entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback0<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(entity, entity.Get<T1>(), entity.Get<T2>(), entity.Get<T3>(), entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback1<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(ref entity.Get<T1>(), entity.Get<T2>(), entity.Get<T3>(), entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback1<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(entity, ref entity.Get<T1>(), entity.Get<T2>(), entity.Get<T3>(), entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback2<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(ref entity.Get<T1>(), ref entity.Get<T2>(), entity.Get<T3>(), entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback2<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(entity, ref entity.Get<T1>(), ref entity.Get<T2>(), entity.Get<T3>(), entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback3<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(ref entity.Get<T1>(), ref entity.Get<T2>(), ref entity.Get<T3>(), entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback3<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(entity, ref entity.Get<T1>(), ref entity.Get<T2>(), ref entity.Get<T3>(), entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback4<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(ref entity.Get<T1>(), ref entity.Get<T2>(), ref entity.Get<T3>(), ref entity.Get<T4>());
    return true;
  }
  public bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback4<T1, T2, T3, T4> callback)
    where T1 : struct 
    where T2 : struct 
    where T3 : struct 
    where T4 : struct 
  {
    if (!entity.Has<T1>()) return false;
    if (!entity.Has<T2>()) return false;
    if (!entity.Has<T3>()) return false;
    if (!entity.Has<T4>()) return false;
    callback(entity, ref entity.Get<T1>(), ref entity.Get<T2>(), ref entity.Get<T3>(), ref entity.Get<T4>());
    return true;
  }
  private static class WorldName<T1> {
    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(typeof(T1));
  }
  private static class WorldName<T1, T2> {
    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(typeof(T1), typeof(T2));
  }
  private static class WorldName<T1, T2, T3> {
    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(typeof(T1), typeof(T2), typeof(T3));
  }
  private static class WorldName<T1, T2, T3, T4> {
    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
  }
  private static class WorldName<T1, T2, T3, T4, T5> {
    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
  }
  private static class WorldName<T1, T2, T3, T4, T5, T6> {
    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
  }
  private static class WorldName<T1, T2, T3, T4, T5, T6, T7> {
    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
  }
  private static class WorldName<T1, T2, T3, T4, T5, T6, T7, T8> {
    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
  }
}
