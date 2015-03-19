namespace Rosalia.Build
{
    using Rosalia.FileSystem;

    public class BuildData
    {
        public string Configuration { get; set; }

        public IDirectory ProjectRootDirectory { get; set; }

        public IDirectory Src { get; set; }

        public IFile SolutionFile { get; set; }

        public IDirectory Artifacts { get; set; }

        public IDirectory BuildAssets
        {
            get
            {
                return Src["Rosalia.Build"]["Assets"];
            }
        }

        public IFile RosaliaPackageInstallScript
        {
            get
            {
                return BuildAssets["Install.ps1"];
            }
        }

        public IDirectory RosaliaRunnerConsole
        {
            get { return Src["Rosalia.Runner.Console"]; }
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
    }
}