// using System;
// using System.IO;
// using System.Text;
// using UnityEditor;
// using UnityEditor.Build;
// using UnityEditor.Build.Reporting;
//
// namespace Code.Editor
// {
//     public class IL2CPPIntegrationBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
//     {
//         private const string WarmupGenericsClassFile = "Assets/Code/buildtime_codegen.cs";
//         
//         [MenuItem("My Tools/PreprocessBuild")]
//         public static void Run()
//         {
//             new IL2CPPIntegrationBuildProcessor().OnPreprocessBuild(null);
//         }
//
//         public int callbackOrder { get; }
//         public void OnPostprocessBuild(BuildReport report)
//         {
//             File.WriteAllText(WarmupGenericsClassFile, "// do not modify this file. it's used for build-time code generation");
//         }
//
//         public void OnPreprocessBuild(BuildReport report)
//         {
//             WarmUpGenerics();
//         }
//
//         private void WarmUpGenerics()
//         {
//             StringBuilder s = new StringBuilder();
//             s.Append("using Leopotam.EcsLite;" +
//                      "using UnityEngine.Scripting;" +
//                      "public class buildtime_codegen {" +
//                      "  [Preserve]" +
//                      "  public static void Warmup(EcsWorld world, EcsFilter.Mask filter) { " +
//                      "       "
//             );
//             foreach (Type type in typeof(Startup).Assembly.GetTypes())
//             {
//                 if (type.IsValueType)
//                 {
//                     string name = type.FullName.Replace("+", ".");
//                     s.Append($"      world.GetPool<{name}>();");
//                     s.Append($"      world.Filter<{name}>();");
//                     s.Append($"      filter.Inc<{name}>();");
//                     s.Append($"      filter.Exc<{name}>();");
//                 }
//             }
//             s.Append("   }" +
//                      "}"
//             );
//             s.Append()
//             File.WriteAllText(WarmupGenericsClassFile, s.ToString());
//         }
//     }
// }