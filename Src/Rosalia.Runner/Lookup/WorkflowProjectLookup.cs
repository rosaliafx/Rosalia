namespace Rosalia.Runner.Lookup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.Context.Environment;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Watchers.Logging;

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

            buildWorkflow.Init(new WorkflowContext(
                new DefaultEnvironment(),
                new DefaultDirectory(options.RunningOptions.WorkDirectory)));

            // not a good thing -- storing messages in memory
            var messages = new List<Message>();
            buildWorkflow.WorkflowMessagePosted += (sender, args) => messages.Add(args.Message);

            var buildResult = buildWorkflow.Execute(options.RunningOptions);

            if (buildResult.ResultType == ResultType.Success)
            {
                var projectName = Path.GetFileNameWithoutExtension(options.RunningOptions.InputFile);
                var assemblyFile = Path.Combine(
                    Path.GetDirectoryName(options.RunningOptions.InputFile), "bin/Debug", projectName + ".dll");

                yield return Assembly.LoadFile(assemblyFile);
            }
            else
            {
                foreach (var message in messages)
                {
                    options.RunningOptions.LogRenderer.AppendMessage(0, message.Text, message.Level, MessageType.TaskMessage);
                }
            }
        }
    }
}