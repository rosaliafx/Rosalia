namespace Rosalia.Runner.Console.Startup.Lookup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Environment;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;

    public class WorkflowProjectLookup : AbstractAssemblyWorkflowLookup
    {
        public override bool CanHandle(LookupOptions options)
        {
            return options.RunningOptions.InputFile.Extension.Is("csproj");
        }

        protected override IEnumerable<Assembly> GetAssemblies(LookupOptions options)
        {
            var buildWorkflow = new BuildWorkflowProjectWorkflow(options.RunningOptions);
            var executerTask = new SubflowTask<Nothing>(buildWorkflow, Identities.Empty);

            var taskContext = new TaskContext(
                new SequenceExecutionStrategy(), 
                options.RunningOptions.LogRenderer, 
                options.RunningOptions.WorkDirectory,
                new DefaultEnvironment(), 
                interceptor: null);

            ITaskResult<Nothing> buildResult = executerTask.Execute(taskContext);
            
            if (buildResult.IsSuccess)
            {
                var projectName = options.RunningOptions.InputFile.NameWithoutExtension;
                IFile assemblyFile = options.RunningOptions.InputFile.Directory["bin"]["Debug"][projectName + ".dll"];

                yield return Assembly.LoadFile(assemblyFile.AbsolutePath);
            }
        }
    }
}