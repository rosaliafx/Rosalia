namespace Rosalia.Runner.Lookup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.FileSystem;

    public class WorkflowProjectLookup : AbstractAssemblyWorkflowLookup
    {
        public override bool CanHandle(LookupOptions options)
        {
            var extension = Path.GetExtension(options.RunningOptions.InputFile);
            return extension != null && extension.Equals(".csproj", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override IEnumerable<Assembly> GetAssemblies(LookupOptions options)
        {
            var buildWorkflow = new BuildWorkflowProjectWorkflow();
            buildWorkflow.WorkDirectory = new DefaultDirectory(options.RunningOptions.WorkDirectory);
            buildWorkflow.Init();

            var buildResult = buildWorkflow.Execute(options.RunningOptions);

            if (buildResult.ResultType == ResultType.Success)
            {
                var projectName = Path.GetFileNameWithoutExtension(options.RunningOptions.InputFile);
                var assemblyFile = Path.Combine(
                    Path.GetDirectoryName(options.RunningOptions.InputFile), "bin/Debug", projectName + ".dll");

                yield return Assembly.LoadFile(assemblyFile);
            }
        }
    }
}