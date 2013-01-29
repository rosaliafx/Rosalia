namespace Rosalia.Build
{
    using Rosalia.Build.Tasks;
    using Rosalia.Core.FileSystem;

    public class BuildRosaliaContext
    {
        public IDirectory ProjectRootDirectory { get; set; }

        public IDirectory Src { get; set; }

        public IFile SolutionFile { get; set; }

        public IDirectory Artifacts { get; set; }

        public string Configuration { get; set; }

        public IDirectory BuildAssets
        {
            get
            {
                return Src
                    .GetDirectory(@"Rosalia.Build")
                    .GetDirectory("Assets");
            }
        }

        public IFile RosaliaPackageInstallScript
        {
            get
            {
                return BuildAssets
                    .GetFile("Install.ps1");
            }
        }

        public IDirectory RosaliaRunnerConsole
        {
            get { return Src.GetDirectory(@"Rosalia.Runner.Console"); }
        }

        public IDirectory RosaliaRunnerConsoleBin
        {
            get { return RosaliaRunnerConsole.GetDirectory(@"bin").GetDirectory(Configuration); }
        }

        public IFile RosaliaRunnerConsoleExe
        {
            get { return RosaliaRunnerConsoleBin.GetFile("Rosalia.exe"); }
        }

        public IFile NuSpecRosalia
        {
            get { return Artifacts.GetFile("Rosalia.nuspec"); }
        }

        public IFile NuSpecRosaliaCore
        {
            get { return Artifacts.GetFile("Rosalia.Core.nuspec"); }
        }

        public string Version { get; set; }

        public PrivateData PrivateData { get; set; }
    }
}