#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Code.CodegenTools;

namespace Kk.BusyEcs
{
    public static class RuntimeEcsContainerGenerator
    {
        public static Type GenerateEcsContainer(IEnumerable<Assembly> userAssemblies)
        {
            EcsContainerSourcesGenerator.Result result = EcsContainerSourcesGenerator.GenerateEcsContainer(userAssemblies);

            List<Assembly> references = new List<Assembly>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "Newtonsoft.Json")
                {
                    // error CS0009: Metadata file `C:\dev\unity\twisty-balls\Library\PackageCache\com.unity.nuget.newtonsoft-json@2.0.0\Runtime\Newtonsoft.Json.dll' does not contain valid metadata
                    continue;
                }

                references.Add(assembly);
            }

            Assembly compiled = AssemblyCompiler.CompileAssembly(CodeGenConstants.ClassName + "_Assembly", new[] { result.source }, references);

            return compiled.GetType(CodeGenConstants.ClassName);
        }
    }
}
#endif