using System;
using System.Reflection;
using Kk.BusyEcs;

namespace Kk.BusyEcs
{
    // used by generated code
    public class WorldResolve
    {
        public static string ResolveWorldName(params Type[] componentTypes)
        {
            string worldName = null;
            foreach (Type componentType in componentTypes)
            {
                string w = componentType.GetCustomAttribute<EcsWorldAttribute>()?.name;
                if (worldName != null && worldName != w)
                {
                    throw new Exception($"world resolution conflict: {worldName} and {w}");
                }

                if (w != "") // attribute value cannot be null
                {
                    worldName = w;
                }
            }

            return worldName;
        }
    }
}