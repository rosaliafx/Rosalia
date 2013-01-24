namespace Rosalia.Build
{
    using Rosalia.Core.FileSystem;

    public class BuildRosaliaContext
    {
        public IDirectory ProjectRootDirectory { get; set; }

        public IDirectory Src { get; set; }

        public IFile SolutionFile { get; set; }

        public IDirectory Artifacts { get; set; }

        public string Configuration { get; set; }

        public IDirectory RosaliaRunnerConsoleBin
        {
            get { return Src.GetDirectory(@"Rosalia.Runner.Console\bin").GetDirectory(Configuration); }
        }

        public string Version { get; set; }
    }
}