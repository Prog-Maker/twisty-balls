using System;
using Code.EcsComponents;
using Kk.BusyEcs;

namespace Code.GenSupport
{
    public interface IConfigurableEcsContainer : IEcsContainer
    {
        void AddInjectable(object injectable, Type overrideType = null);
        void Init(Leopotam.EcsLite.EcsSystems systems);
        void Execute<T>() where T : Attribute;
    }
}