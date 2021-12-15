using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using UnityEngine;

namespace Code.CodegenTools
{
    public static class AssemblyCompiler
    {
        public static Assembly CompileAssembly(string assemblyName, IEnumerable<string> sources, IEnumerable<Assembly> referredAssemblies)
        {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            foreach (string source in sources)
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions().WithLanguageVersion(LanguageVersion.CSharp7_2));
                syntaxTrees.Add(syntaxTree);
            }

            List<MetadataReference> references = new List<MetadataReference>();
            foreach (Assembly assembly in referredAssemblies)
            {
                if (assembly.Location != "")
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            MemoryStream buffer = new MemoryStream();
            EmitResult result = compilation.Emit(buffer);
            
            foreach (Diagnostic diagnostic in result.Diagnostics)
            {
                switch (diagnostic.Severity)
                {
                    case DiagnosticSeverity.Error:
                        Debug.LogError(diagnostic);
                        break;
                    case DiagnosticSeverity.Hidden:
                        Debug.Log(diagnostic);
                        break;
                    case DiagnosticSeverity.Info:
                        Debug.Log(diagnostic);
                        break;
                    case DiagnosticSeverity.Warning:
                        Debug.LogWarning(diagnostic);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (!result.Success)
            {
                return null;
            }
            return Assembly.Load(buffer.GetBuffer());
        }
    }
}