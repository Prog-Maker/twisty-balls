using System.Reflection;
using Kk.BusyEcs;

namespace Code.GenSupport
{
    public static class WorldHolder<T>
    {
        public static readonly string worldName = typeof(T).GetCustomAttribute<EcsWorldAttribute>()?.name;
    }
}