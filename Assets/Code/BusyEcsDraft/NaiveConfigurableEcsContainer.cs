using System;
using Kk.BusyEcs;

namespace Kk.BusyEcs
{
    public class NaiveConfigurableEcsContainer : IConfigurableEcsContainer
    {
        private IEcsContainer _ecs;
        private readonly EcsContainerBuilder _builder;

        public NaiveConfigurableEcsContainer()
        {
            _builder = new EcsContainerBuilder();
        }

        public void AddInjectable(object injectable, Type overrideType = null)
        {
            _builder.Injectable(injectable, overrideType);
        }

        public void Init(Leopotam.EcsLite.EcsSystems systems)
        {
            _ecs = _builder
                .Scan(typeof(Startup).Assembly)
                .Integrate(systems)
                .End();
        }

        public bool Match<T1>(Entity entity, SimpleCallback<T1> callback) where T1 : struct
        {
            return _ecs.Match(entity, callback);
        }

        public bool Match<T1>(Entity entity, EntityCallback<T1> callback) where T1 : struct
        {
            return _ecs.Match(entity, callback);
        }

        public void Query<T1>(SimpleCallback<T1> callback) where T1 : struct
        {
            _ecs.Query(callback);
        }

        public void Query<T1>(EntityCallback<T1> callback) where T1 : struct
        {
            _ecs.Query(callback);
        }

        public bool Match<T1, T2>(Entity entity, SimpleCallback<T1, T2> callback) where T1 : struct where T2 : struct
        {
            return _ecs.Match(entity, callback);
        }

        public bool Match<T1, T2>(Entity entity, EntityCallback<T1, T2> callback) where T1 : struct where T2 : struct
        {
            return _ecs.Match(entity, callback);
        }

        public void Query<T1, T2>(SimpleCallback<T1, T2> callback) where T1 : struct where T2 : struct
        {
            _ecs.Query(callback);
        }

        public void Query<T1, T2>(EntityCallback<T1, T2> callback) where T1 : struct where T2 : struct
        {
            _ecs.Query(callback);
        }

        public bool Match<T1, T2, T3>(Entity entity, SimpleCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct
        {
            return _ecs.Match(entity, callback);
        }

        public bool Match<T1, T2, T3>(Entity entity, EntityCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct
        {
            return _ecs.Match(entity, callback);
        }

        public void Query<T1, T2, T3>(SimpleCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct
        {
            _ecs.Query(callback);
        }

        public void Query<T1, T2, T3>(EntityCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct
        {
            _ecs.Query(callback);
        }

        public bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct
        {
            return _ecs.Match(entity, callback);
        }

        public bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct
        {
            return _ecs.Match(entity, callback);
        }

        public void Query<T1, T2, T3, T4>(SimpleCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct
        {
            _ecs.Query(callback);
        }

        public void Query<T1, T2, T3, T4>(EntityCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct
        {
            _ecs.Query(callback);
        }

        public Entity NewEntity<T1>(in T1 c1) where T1 : struct
        {
            return _ecs.NewEntity(in c1);
        }

        public Entity NewEntity<T1, T2>(in T1 c1, in T2 c2) where T1 : struct where T2 : struct
        {
            return _ecs.NewEntity(in c1, in c2);
        }

        public Entity NewEntity<T1, T2, T3>(in T1 c1, in T2 c2, in T3 c3) where T1 : struct where T2 : struct where T3 : struct
        {
            return _ecs.NewEntity(in c1, in c2, in c3);
        }

        public Entity NewEntity<T1, T2, T3, T4>(in T1 c1, in T2 c2, in T3 c3, in T4 c4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct
        {
            return _ecs.NewEntity(in c1, in c2, in c3, in c4);
        }

        public Entity NewEntity<T1, T2, T3, T4, T5>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct
        {
            return _ecs.NewEntity(in c1, in c2, in c3, in c4, in c5);
        }

        public Entity NewEntity<T1, T2, T3, T4, T5, T6>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct
        {
            return _ecs.NewEntity(in c1, in c2, in c3, in c4, in c5, in c6);
        }

        public Entity NewEntity<T1, T2, T3, T4, T5, T6, T7>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct
        {
            return _ecs.NewEntity(in c1, in c2, in c3, in c4, in c5, in c6, in c7);
        }

        public Entity NewEntity<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct where T8 : struct
        {
            return _ecs.NewEntity(in c1, in c2, in c3, in c4, in c5, in c6, in c7, in c8);
        }

        public void Execute<T>() where T : Attribute
        {
            _ecs.Execute<T>();
        }
    }
}