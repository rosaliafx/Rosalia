namespace Rosalia.Runner.Console.Startup
{
    using System.Collections.Generic;
    using Rosalia.Core.Logging;
    using Rosalia.FileSystem;

    public class RunningOptions
    {
        public IDirectory WorkDirectory { get; set; }

        public IFile InputFile { get; set; }

        public string DefaultWorkflowType { get; set; }

        public string WorkflowProjectBuildConfiguration { get; set; }

        public string WorkflowBuildOutputPath { get; set; }

        public ILogRenderer LogRenderer { get; set; }

        public IDictionary<string, string> Properties { get; set; }

    }
}