namespace Rosalia.Runner.Console
{
    using System.Collections.Generic;

    public class ProgramOptions
    {
        public ProgramOptions()
        {
            VisualisationFilesPath = new List<string>();
            LogFilesPath = new List<string>();
            Properties = new Dictionary<string, string>();
        }

        public bool ShowHelp { get; set; }

        public bool NoLogo { get; set; }

        public bool Hold { get; set; }

        public string InputFile { get; set; }

        public string DefaultWorkflow { get; set; }

        public string WorkflowProjectBuildConfiguration { get; set; }

        public string WorkflowBuildOutputPath { get; set; }

        public string WorkDirectory { get; set; }

        public IList<string> VisualisationFilesPath { get; private set; }

        public IList<string> LogFilesPath { get; private set; }

        public IDictionary<string, string> Properties { get; private set; }
    }
}