namespace Rosalia.Runner
{
    using System.Collections.Generic;
    using Rosalia.Core.Watchers.Logging;

    public class RunningOptions
    {
        public string WorkDirectory { get; set; }

        public string InputFile { get; set; }

        public string DefaultWorkflowType { get; set; }

        public string WorkflowProjectBuildConfiguration { get; set; }

        public string WorkflowBuildOutputPath { get; set; }

        public ILogRenderer LogRenderer { get; set; }

        public IList<string> VisualisationFilesPath { get; set; }
    }
}