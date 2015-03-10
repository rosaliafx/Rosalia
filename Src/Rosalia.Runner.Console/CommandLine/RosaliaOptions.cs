namespace Rosalia.Runner.Console.CommandLine
{
    using System.Collections.Generic;
    using Rosalia.Core;

    public class RosaliaOptions
    {
        public RosaliaOptions()
        {
            OutputFiles = new List<string>();
            Properties = new Dictionary<string, string>();
            Tasks = Identities.Empty;
        }

        public bool ShowHelp { get; set; }

        public bool NoLogo { get; set; }

        public bool Hold { get; set; }

        public string InputFile { get; set; }

        public string WorkflowProjectBuildConfiguration { get; set; }

        public string WorkflowBuildOutputPath { get; set; }

        public string WorkDirectory { get; set; }

        public string Workflow { get; set; }

        public Identities Tasks { get; set; }

        public IList<string> OutputFiles { get; private set; }

        public IDictionary<string, string> Properties { get; private set; }
    }
}