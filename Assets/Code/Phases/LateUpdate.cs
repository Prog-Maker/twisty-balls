using System;
using Kk.BusyEcs;

namespace Code.Phases
{
    [EcsPhase]
    [AttributeUsage(AttributeTargets.Method)]
    public class LateUpdate : Attribute { }
}