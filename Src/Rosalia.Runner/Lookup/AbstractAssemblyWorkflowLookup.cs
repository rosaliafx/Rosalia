namespace Rosalia.Runner.Lookup
{
    using System;
    using System.Collections.Generic;
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
                        yield return CreateWorkflowInfo(exportedType, workflowType);
                    }
                }
            }
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