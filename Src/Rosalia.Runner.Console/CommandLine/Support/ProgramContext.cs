namespace Rosalia.Runner.Console.CommandLine.Support
{
    using Rosalia.Core.Api;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.FileSystem;

    public class ProgramContext
    {
        public string[] Args { get; set; }
        public OptionsConfig OptionsConfig { get; set; }
        public RosaliaOptions Options { get; set; }
        public ILogRenderer LogRenderer { get; set; }
        public LogHelper Log { get; set; }
        public IDirectory WorkDirectory { get; set; }
        public IFile InputFile { get; set; }
        public IWorkflow Workflow { get; set; }
    }
}