using System;
using Kk.BusyEcs;

namespace Kk.BusyEcs
{
    public interface IConfigurableEcsContainer : IEcsContainer
    {
        void AddInjectable(object injectable, Type overrideType = null);
        void Init(Leopotam.EcsLite.EcsSystems systems);
    }
}