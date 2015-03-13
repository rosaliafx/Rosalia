namespace Rosalia.Runner.Console.Startup.Lookup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Environment;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;

    public class WorkflowProjectLookup : AbstractAssemblyWorkflowLookup
    {
        public override bool CanHandle(LookupOptions options)
        {
            return options.RunningOptions.InputFile.Extension.Is("csproj");

//            var extension = Path.GetExtension(options.RunningOptions.InputFile);
//            return extension != null && extension.Equals(".csproj", StringComparison.InvariantCultureIgnoreCase);
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

            var buildResult = executerTask.Execute(taskContext);

//            buildWorkflow.Init(new WorkflowContext(
//                new DefaultEnvironment(),
//                new DefaultDirectory(options.RunningOptions.WorkDirectory)));

            // not a good thing -- storing messages in memory
//            var messages = new List<Message>();
//            buildWorkflow.WorkflowMessagePosted += (sender, args) => messages.Add(args.Message);
//
//            var buildResult = buildWorkflow.Execute(options.RunningOptions);

            if (buildResult.IsSuccess)
            {
                var projectName = options.RunningOptions.InputFile.NameWithoutExtension;
                IFile assemblyFile = options.RunningOptions.InputFile.Directory["bin"]["Debug"][projectName + ".dll"];
                    
//                var projectName = Path.GetFileNameWithoutExtension(options.RunningOptions.InputFile);
//                var assemblyFile = Path.Combine(
//                    Path.GetDirectoryName(options.RunningOptions.InputFile), "bin/Debug", projectName + ".dll");

                yield return Assembly.LoadFile(assemblyFile.AbsolutePath);
            }
//            else
//            {
//                foreach (var message in messages)
//                {
//                    options.RunningOptions.LogRenderer.AppendMessage(0, message.Text, message.Level, MessageType.TaskMessage);
//                }
//            }
        }
    }
}