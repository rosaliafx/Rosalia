namespace Rosalia.Runner.Lookup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;

    public abstract class AbstractAssemblyWorkflowLookup : IWorkflowLookup
    {
        public abstract bool CanHandle(LookupOptions options);

        public IEnumerable<WorkflowInfo> Find(LookupOptions options)
        {
            foreach (var assembly in GetAssemblies(options))
            {
                foreach (var exportedType in assembly.GetExportedTypes())
                {
                    var workflowType = GetWorkflowTypeIfPossible(exportedType);
                    if (workflowType != null)
                    {
                        LoadDllsFromAssemblyDirectory(assembly, options);
                        yield return CreateWorkflowInfo(exportedType, workflowType);
                    }
                }
            }
        }

        private void LoadDllsFromAssemblyDirectory(Assembly assembly, LookupOptions options)
        {
            var directory = Path.GetDirectoryName(assembly.Location);
            // todo Check it!!!
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var assemblyFileName = string.Format("{0}.dll", args.Name.Substring(0,  args.Name.IndexOf(",", StringComparison.InvariantCultureIgnoreCase)));
                var assemblyFullPath = Path.Combine(directory, assemblyFileName);
                if (File.Exists(assemblyFullPath))
                {
                    return Assembly.LoadFrom(assemblyFullPath);
                }

                return null;
            };
        }

        protected virtual WorkflowInfo CreateWorkflowInfo(Type type, Type workflowBaseType)
        {
            return new WorkflowInfo
            {
                ContextType = GetContextType(workflowBaseType),
                WorkflowType = type
            };
        }

        protected virtual Type GetContextType(Type workflowBaseType)
        {
            return workflowBaseType.GetGenericArguments()[0];
        }

        protected virtual Type GetBaseWorkflowType(Type[] interfaces)
        {
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType && typeof(IGenericWorkflow<>).IsAssignableFrom(@interface.GetGenericTypeDefinition()))
                {
                    return @interface;
                }
            }

            return null;
        }

        protected virtual Type GetWorkflowTypeIfPossible(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                return null;
            }

            return GetBaseWorkflowType(type.GetInterfaces());
        }

        protected abstract IEnumerable<Assembly> GetAssemblies(LookupOptions options);
    }
}