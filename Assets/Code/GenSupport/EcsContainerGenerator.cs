using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Kk.BusyEcs;
using UnityEditor;

namespace Code.GenSupport
{
    public static class EcsContainerGenerator
    {
        const string ClassName = "GeneratedEcsContainer";
        const int NewEntityMaxComponentCount = 8;
        const int QueryMaxComponentCount = 4;

        [MenuItem("My Tools/Generate ECS container")]
        public static void TestGenerateSystemsGlue()
        {
            File.WriteAllText($"Assets/Code/{ClassName}.gen.cs", GenerateEcsContainer());
        }

        public static string GenerateEcsContainer()
        {
            Context context = new Context();
            Scan(context);

            return GenerateBody(context);
        }

        public class Injection
        {
            public Type system;
            public Type type;
            public string field;
        }

        public class Context
        {
            public List<Injection> injections = new List<Injection>();
            public Dictionary<Type, List<MethodInfo>> systemsByPhase = new Dictionary<Type, List<MethodInfo>>();
            public List<Type> systemClasses = new List<Type>();
            public HashSet<Type> components = new HashSet<Type>();
            public List<List<Type>> filters = new List<List<Type>>();
            public List<string> worlds = new List<string>();
        }

        public static string GenerateBody(Context ctx)
        {
            string s = "";
            s += "using System;";
            s += "using System.Collections.Generic;";
            s += "using Kk.BusyEcs;";
            s += "using Leopotam.EcsLite;";
            s += "using System.Reflection;";
            s += "[UnityEngine.Scripting.Preserve]";
            s += "public class " + ClassName + " : Code.GenSupport.IConfigurableEcsContainer {";
            s += GenerateFields(ctx);
            s += "  public " + ClassName + "() {";
            s += GenerateConstructor(ctx);
            s += "  }";
            s += "  public void Init(EcsSystems worlds) {";
            s += GenerateInit(ctx);
            s += "  }";
            s += "  public void Execute<T>() where T : Attribute {";
            s += "    _phaseExecutionByType[typeof(T)]();";
            s += "  }";
            s += GenerateMethods(ctx);
            s += GenerateNestedClasses(ctx);
            s += "}";
            return s;
        }

        private static string GenerateFields(Context ctx)
        {
            string s = "";

            s += "private readonly Dictionary<Type, Action> _phaseExecutionByType = new Dictionary<Type, Action>();";
            s += "private EcsSystems worlds;";
            s += "private Dictionary<Type, object> injectables = new Dictionary<Type, object>();";
            s += "private List<EcsWorld> allWorlds = new List<EcsWorld>();";

            foreach (Type systemClass in ctx.systemClasses)
            {
                s += "private " + systemClass.FullName + " " + SystemInstanceVar(systemClass) + ";";
            }

            foreach (string world in ctx.worlds)
            {
                s += "private EcsWorld " + WorldVar(world) + ";";
                foreach (Type componentType in ctx.components)
                {
                    s += "private EcsPool<" + componentType.FullName + "> " + PoolVar(world, componentType) + ";";
                }

                foreach (List<Type> pair in ctx.filters)
                {
                    s += "private EcsFilter " + FilterName(world, pair) + ";";
                }
            }

            return s;
        }

        private static string SystemInstanceVar(Type type)
        {
            return "_" + type.Name;
        }

        private static string GenerateInit(Context ctx)
        {
            string s = "";
            s += "this.worlds = worlds;";
            foreach (Type systemClass in ctx.systemClasses)
            {
                s += SystemInstanceVar(systemClass) + " = new " + systemClass.FullName + " ();";
            }

            foreach (string world in ctx.worlds)
            {
                if (world == "")
                {
                    s += WorldVar(world) + " = worlds.GetWorld();";
                }
                else
                {
                    s += "if (worlds.GetWorld(\"" + world + "\") == null) { worlds.AddWorld(new EcsWorld(), \"" + world + "\");}";
                    s += WorldVar(world) + " = worlds.GetWorld(\"" + world + "\");";
                }

                s += "allWorlds.Add(" + WorldVar(world) + ");";

                foreach (Type componentType in ctx.components)
                {
                    s += PoolVar(world, componentType) + " = " + WorldVar(world) + ".GetPool<" + componentType.FullName + ">();";
                }

                foreach (List<Type> pair in ctx.filters)
                {
                    s += FilterName(world, pair) + " = " + WorldVar(world) + ".Filter<" + pair[0].FullName + ">()";
                    for (var i = 1; i < pair.Count; i++)
                    {
                        s += ".Inc<" + pair[i] + ">()";
                    }

                    s += ".End();";
                }
            }

            foreach (Injection injection in ctx.injections)
            {
                s += SystemInstanceVar(injection.system) + "." + injection.field + " = (" + injection.type.FullName + ") ResolveInjectable<" +
                     injection.type.FullName + ">();";
            }

            return s;
        }

        private static string WorldVar(string world)
        {
            if (world == "")
            {
                return "defaultWorld";
            }

            return world;
        }

        private static string FilterName(string world, List<Type> components)
        {
            return "filter_" + WorldVar(world) + "_" + string.Join("_", components.Select(it => it.Name).OrderBy(x => x));
        }

        private static string PoolVar(string world, Type type)
        {
            return "pool_" + WorldVar(world) + "_" + type.Name;
        }

        private static string GenerateConstructor(Context ctx)
        {
            long nextVarId = 0;
            string s = "";
            s += "AddInjectable(this, typeof(IEnv));";
            s += "typeof(Entity).GetField(\"env\", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, this);";
            foreach (KeyValuePair<Type, List<MethodInfo>> pair in ctx.systemsByPhase)
            {
                s += "_phaseExecutionByType[typeof(" + pair.Key.FullName + ")] = () => {";
                foreach (MethodInfo method in pair.Value)
                {
                    if (method.GetParameters().Length <= 0)
                    {
                        s += SystemInstanceVar(method.DeclaringType) + "." + method.Name + "();";
                    }
                    else
                    {
                        bool supplyEntity = false;
                        List<Type> components = new List<Type>();
                        foreach (ParameterInfo parameter in method.GetParameters())
                        {
                            if (parameter.ParameterType == typeof(Entity))
                            {
                                supplyEntity = true;
                            }
                            else
                            {
                                if (parameter.ParameterType.IsByRef)
                                {
                                    components.Add(parameter.ParameterType.GetElementType());
                                }
                                else
                                {
                                    components.Add(parameter.ParameterType);
                                }
                            }
                        }

                        foreach (string world in ctx.worlds)
                        {
                            string entityVar = "entity" + nextVarId++;
                            s += "foreach (var " + entityVar + " in " + FilterName(world, components) + ") {";
                            s += SystemInstanceVar(method.DeclaringType) + "." + method.Name + "(";
                            if (supplyEntity) { }

                            bool first = true;
                            foreach (ParameterInfo parameter in method.GetParameters())
                            {
                                if (first)
                                {
                                    first = false;
                                }
                                else
                                {
                                    s += ", ";
                                }

                                if (parameter.ParameterType == typeof(Entity))
                                {
                                    s += "new Entity(" + WorldVar(world) + ", " + entityVar + ")";
                                }
                                else
                                {
                                    Type componentType;
                                    if (parameter.ParameterType.IsByRef)
                                    {
                                        s += "ref ";
                                        componentType = parameter.ParameterType.GetElementType();
                                    }
                                    else
                                    {
                                        componentType = parameter.ParameterType;
                                    }

                                    s += PoolVar(world, componentType) + ".Get(" + entityVar + ")";
                                }
                            }

                            s += ");";
                            s += "}";
                        }
                    }
                }

                s += "};";
            }

            return s;
        }


        private static string GenerateMethods(Context ctx)
        {
            string s = "";
            s += "  public void AddInjectable(object injectable, Type overrideType = null)";
            s += "  {";
            s += "    injectables[overrideType ?? injectable.GetType()] = injectable;";
            s += "  }";
            s += "  private object ResolveInjectable<T>()";
            s += "  {";
            s += "    if (!injectables.TryGetValue(typeof(T), out var injectable))";
            s += "    {";
            s += "        throw new Exception(\"failed to resolve injection of \" + typeof(T).FullName);";
            s += "    }";
            s += "    ";
            s += "    return injectable;";
            s += "  }";

            for (int componentCount = 1; componentCount <= NewEntityMaxComponentCount; componentCount++)
            {
                string gsig = "";

                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) gsig += ", ";

                    gsig += "T" + i;
                }

                s += "public Entity NewEntity<" + gsig + ">(";

                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ", ";
                    s += "in T" + i + " c" + i;
                }

                s += ") ";

                for (int i = 1; i <= componentCount; i++)
                {
                    s += " where T" + i + " : struct ";
                }


                s += " {";
                s += "EcsWorld w = worlds.GetWorld(WorldName<" + gsig + ">.worldName);";
                s += "var id = w.NewEntity();";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += "w.GetPool<T" + i + ">().Add(id) = c" + i + ";";
                }

                s += "return new Entity(w, id);";
                s += "}";
            }

            for (int componentCount = 1; componentCount <= QueryMaxComponentCount; componentCount++)
            {
                string gsig = "";
                string where = "";

                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) gsig += ", ";

                    gsig += "T" + i;
                }

                for (int i = 1; i <= componentCount; i++)
                {
                    where += " where T" + i + " : struct ";
                }

                s += "public void Query<" + gsig + ">(SimpleCallback<" + gsig + "> callback) " + where + " {";
                s += "foreach (EcsWorld w in allWorlds) {";
                
                s += "EcsFilter filter = w.Filter<T1>()";
                for (int i = 2; i <= componentCount; i++)
                {
                    s += ".Inc<T" + i + ">()";
                }

                s += ".End();";
                s += "foreach (var id in filter) {";
                s += "callback(";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ",";
                    s += "ref w.GetPool<T"+i+">().Get(id)";
                }
                s += ");";
                s += "}";

                s += "}";
                s += "}";
                s += "public void Query<" + gsig + ">(EntityCallback<" + gsig + "> callback) " + where + "{";
                s += "}";
            }

            for (int componentCount = 1; componentCount <= QueryMaxComponentCount; componentCount++)
            {
                string gsig = "";
                string where = "";

                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) gsig += ", ";

                    gsig += "T" + i;
                }

                for (int i = 1; i <= componentCount; i++)
                {
                    where += " where T" + i + " : struct ";
                }

                s += "public bool Match<"+gsig+">(Entity entity, SimpleCallback<"+gsig+"> callback) " + where +" {";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"if (!entity.Has<T{i}>()) return false;";
                }

                s += "callback(";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ",";
                    s += "ref entity.Get<T"+i+">()";
                }
                s += ");";
                s += "return true;";

                s += "}";

                s += "public bool Match<"+gsig+">(Entity entity, EntityCallback<"+gsig+"> callback) " + where +" {";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"if (!entity.Has<T{i}>()) return false;";
                }

                s += "callback(entity, ";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ",";
                    s += "ref entity.Get<T"+i+">()";
                }
                s += ");";
                s += "return true;";

                s += "}";
            }
            

            return s;
        }

        private static string GenerateNestedClasses(Context ctx)
        {
            var s = "";

            for (int componentCount = 1; componentCount <= NewEntityMaxComponentCount; componentCount++)
            {
                s += "private static class WorldName<";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ", ";

                    s += "T" + i;
                }

                s += "> {";
                s += "internal static readonly string worldName = Code.GenSupport.WorldResolve.ResolveWorldName(";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ", ";

                    s += "typeof(T" + i + ")";
                }

                s += ");";
                s += "}";
            }

            return s;
        }

        private static void Scan(Context ctx)
        {
            ctx.systemsByPhase = new Dictionary<Type, List<MethodInfo>>();
            ctx.systemClasses = new List<Type>();

            HashSet<string> worlds = new HashSet<string>();
            worlds.Add("");
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<EcsPhaseAttribute>() != null)
                    {
                        ctx.systemsByPhase[type] = new List<MethodInfo>();
                    }

                    if (type.GetCustomAttribute<EcsSystemAttribute>() != null)
                    {
                        ctx.systemClasses.Add(type);

                        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            if (field.GetCustomAttribute<InjectAttribute>() != null)
                            {
                                ctx.injections.Add(new Injection
                                {
                                    field = field.Name,
                                    system = type,
                                    type = field.FieldType
                                });
                            }
                        }
                    }

                    EcsWorldAttribute ecsWorldAttribute = type.GetCustomAttribute<EcsWorldAttribute>();
                    if (ecsWorldAttribute != null)
                    {
                        worlds.Add(ecsWorldAttribute.name);
                    }
                }
            }

            ctx.worlds.AddRange(worlds.OrderBy(it => it));

            foreach (Type systemClass in ctx.systemClasses)
            {
                foreach (MethodInfo method in systemClass.GetMethods())
                {
                    foreach (Attribute attribute in method.GetCustomAttributes())
                    {
                        if (ctx.systemsByPhase.TryGetValue(attribute.GetType(), out var systemsForPhase))
                        {
                            systemsForPhase.Add(method);
                        }
                    }
                }
            }


            foreach (List<MethodInfo> methods in ctx.systemsByPhase.Values)
            {
                foreach (MethodInfo method in methods)
                {
                    List<Type> filter = new List<Type>();
                    foreach (ParameterInfo parameter in method.GetParameters())
                    {
                        if (parameter.ParameterType == typeof(Entity))
                        {
                            continue;
                        }

                        Type componentType = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
                        ctx.components.Add(componentType);
                        filter.Add(componentType);
                    }

                    if (filter.Any())
                    {
                        ctx.filters.Add(filter.OrderBy(x => x.Name).ToList());
                    }
                }
            }
        }
    }
}