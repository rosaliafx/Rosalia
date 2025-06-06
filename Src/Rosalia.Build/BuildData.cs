namespace Rosalia.Build
{
    using Rosalia.FileSystem;

    public class BuildData
    {
        public static readonly string LibsTargetFramework = "netstandard2.0";

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
                return BuildAssets["init.ps1"];
            }
        }

        public IDirectory RosaliaRunnerConsole
        {
            get { return Src["Rosalia.Runner.Console"]; }
        }

        public IDirectory RosaliaRunnerConsoleBin
        {
            get { return RosaliaRunnerConsole.GetDirectory(@"bin").GetDirectory(Configuration).GetDirectory("net7.0"); }
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