using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using AutoMapper;

namespace SFA.DAS.AssessorService.AutoMapperExtensions
{
    public static class MapCallAnalyzer
    {
        public static void FindAndPrintMapCalls(string assemblyPath, IConfigurationProvider mapperConfig)
        {
            try
            {
                var mainAssembly = AssemblyDefinition.ReadAssembly(assemblyPath);
                var referencedAssemblies = GetReferencedAssemblies(assemblyPath);

                Console.WriteLine($"Analyzing assembly: {Path.GetFileName(assemblyPath)}");

                // Analyze the main assembly
                AnalyzeAssemblyForMapCalls(mainAssembly, mainAssembly.MainModule, Path.GetFileName(assemblyPath), mapperConfig);

                // Analyze referenced assemblies
                foreach (var referencedAssembly in referencedAssemblies)
                {
                    Console.WriteLine($"Analyzing referenced assembly: {referencedAssembly.MainModule.FileName}");
                    AnalyzeReferencedAssembly(mainAssembly, referencedAssembly, mapperConfig);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing assembly {Path.GetFileName(assemblyPath)}: {ex.Message}");
            }
        }

        private static void AnalyzeReferencedAssembly(AssemblyDefinition mainAssembly, AssemblyDefinition referencedAssembly, IConfigurationProvider mapperConfig)
        {
            try
            {
                foreach (var module in referencedAssembly.Modules)
                {
                    foreach (var type in module.Types)
                    {
                        AnalyzeTypeForMapCalls(type, referencedAssembly.MainModule.FileName, mapperConfig);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Error analyzing referenced assembly {referencedAssembly.Name}: {ex.Message}");
            }
        }

        private static void AnalyzeAssemblyForMapCalls(AssemblyDefinition assembly, ModuleDefinition module, string assemblyName, IConfigurationProvider mapperConfig)
        {
            foreach (var type in module.Types)
            {
                AnalyzeTypeForMapCalls(type, assemblyName, mapperConfig);
            }
        }

        private static void AnalyzeTypeForMapCalls(TypeDefinition type, string assemblyName, IConfigurationProvider mapperConfig)
        {
            foreach (var method in type.Methods)
            {
                AnalyzeMethodForMapCalls(method, type.FullName, assemblyName, mapperConfig);
            }

            foreach (var nestedType in type.NestedTypes)
            {
                AnalyzeTypeForMapCalls(nestedType, assemblyName, mapperConfig);
            }
        }

        private static void AnalyzeMethodForMapCalls(MethodDefinition method, string declaringTypeName, string assemblyName, IConfigurationProvider mapperConfig)
        {
            if (!method.HasBody) return;

            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Callvirt || instruction.OpCode == OpCodes.Call)
                {
                    var methodReference = instruction.Operand as GenericInstanceMethod;
                    if (methodReference != null && methodReference.Name == "Map")
                    {
                        var destinationTypes = methodReference.GenericArguments
                            .Select(arg => ExtractBaseType(arg.FullName))
                            .ToList();

                        var sourceObjectType = ExtractBaseType(FindSourceObjectType(instruction, method));

                        if (string.IsNullOrEmpty(sourceObjectType))
                        {
                            System.Diagnostics.Trace.WriteLine($"Skipping mapping with invalid source type: {sourceObjectType}");
                            continue;
                        }

                        System.Diagnostics.Trace.WriteLine($"--- Found .Map<T> Call ---");
                        System.Diagnostics.Trace.WriteLine($"    Location: {declaringTypeName}.{method.Name} (Assembly: {assemblyName})");
                        System.Diagnostics.Trace.WriteLine($"    Mapping: {sourceObjectType} -> {string.Join(", ", destinationTypes)}");

                        // Validate the mapping
                        ValidateMapping(destinationTypes, sourceObjectType, mapperConfig);

                        System.Diagnostics.Trace.WriteLine($"---------------------------");
                    }
                }
            }
        }

        private static void ValidateMapping(List<string> destinationTypes, string sourceType, IConfigurationProvider mapperConfig)
        {
            foreach (var destinationType in destinationTypes)
            {
                var resolvedSourceType = ResolveType(sourceType);
                var resolvedDestinationType = ResolveType(destinationType);

                if (resolvedSourceType == null || resolvedDestinationType == null)
                {
                    System.Diagnostics.Trace.WriteLine($"Error: Could not resolve source type '{sourceType}' or destination type '{destinationType}'.");
                    continue;
                }

                if (resolvedSourceType.AssemblyQualifiedName == resolvedDestinationType.AssemblyQualifiedName)
                {
                    System.Diagnostics.Trace.WriteLine($"Warning: Source and destination types are identical ({resolvedSourceType.AssemblyQualifiedName}). This may indicate redundant mapping.");
                    continue;
                }

                try
                {
                    var executionPlan = mapperConfig.BuildExecutionPlan(resolvedSourceType, resolvedDestinationType);
                    if (executionPlan != null)
                    {
                        System.Diagnostics.Trace.WriteLine($"Valid Mapping: {resolvedSourceType.AssemblyQualifiedName} -> {resolvedDestinationType.AssemblyQualifiedName}");
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"ERROR: No mapping found for {resolvedSourceType.AssemblyQualifiedName} -> {resolvedDestinationType.AssemblyQualifiedName}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine($"Error validating mapping for {resolvedSourceType.AssemblyQualifiedName} -> {resolvedDestinationType.AssemblyQualifiedName}: {ex.Message}");
                }
            }
        }

        private static string FindSourceObjectType(Instruction instruction, MethodDefinition method)
        {
            var previousInstruction = instruction.Previous;
            System.Diagnostics.Trace.WriteLine($"Analyzing instructions for source type resolution...");

            while (previousInstruction != null)
            {
                if (previousInstruction.OpCode == OpCodes.Ldarg)
                {
                    if (previousInstruction.Operand is ParameterDefinition parameter)
                    {
                        return parameter.ParameterType.FullName;
                    }
                }

                if (previousInstruction.OpCode == OpCodes.Ldfld)
                {
                    if (previousInstruction.Operand is FieldReference fieldReference)
                    {
                        return fieldReference.FieldType.FullName;
                    }
                }

                if (previousInstruction.OpCode == OpCodes.Newobj)
                {
                    if (previousInstruction.Operand is MethodReference methodReference)
                    {
                        return methodReference.DeclaringType.FullName;
                    }
                }

                previousInstruction = previousInstruction.Previous;
            }

            return "Unknown";
        }

        private static string ExtractBaseType(string typeName)
        {
            if (typeName.Contains("<") && typeName.Contains(">"))
            {
                var startIndex = typeName.IndexOf('<') + 1;
                var endIndex = typeName.LastIndexOf('>');
                return typeName.Substring(startIndex, endIndex - startIndex);
            }
            return typeName;
        }

        private static Type ResolveType(string typeName)
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == typeName || t.AssemblyQualifiedName == typeName);
            }
            catch
            {
                return null;
            }
        }

        private static List<AssemblyDefinition> GetReferencedAssemblies(string assemblyPath)
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));

            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { AssemblyResolver = resolver });
            var assemblies = new List<AssemblyDefinition> { assembly };

            foreach (var reference in assembly.MainModule.AssemblyReferences)
            {
                try
                {
                    var referencedAssembly = resolver.Resolve(reference);
                    assemblies.Add(referencedAssembly);
                }
                catch
                {
                    // Skip unresolved assemblies
                }
            }

            return assemblies;
        }
    }
}
