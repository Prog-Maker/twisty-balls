using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kk.BusyEcs
{
    public static class EcsContainerFactory
    {
        public static IConfigurableEcsContainer NewInstance(IEnumerable<Assembly> userAssemblies)
        {
            Type ecsContainerType = userAssemblies.Select(it => it.GetType(CodeGenConstants.ClassName)).SingleOrDefault();
#if UNITY_EDITOR
            if (ecsContainerType == null)
            {
                ecsContainerType = RuntimeEcsContainerGenerator.GenerateEcsContainer(userAssemblies);
            }
#endif
            if (ecsContainerType == null)
            {
                throw new Exception("invalid BusyECS configuration");
            }
            return (IConfigurableEcsContainer)Activator.CreateInstance(ecsContainerType);
        }
    }
}