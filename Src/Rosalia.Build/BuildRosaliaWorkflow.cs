namespace Rosalia.Build
{
    using System.Collections.Generic;
    using Rosalia.Core;
    using Rosalia.Core.FileSystem;
    using Rosalia.TaskLib.MsBuild;
    using Rosalia.TaskLib.NuGet;

    public class BuildRosaliaWorkflow : Workflow<BuildRosaliaContext>
    {
        public SequenceTask<BuildRosaliaContext> MainSequence
        {
            get
            {
                return Sequence
                    .WithSubtask(new MsBuildTask<BuildRosaliaContext>()
                        .FillInput(c => new MsBuildInput()
                            .WithProjectFile(c.Data.SolutionFile)))
                    .WithSubtask(GenerateNuGetSpec)
                    .WithSubtask(new GeneratePackageTask<BuildRosaliaContext>(
                        GetConsoleRunnerSpecFile));
            }
        }

        private static ITask<BuildRosaliaContext> GenerateNuGetSpec
        {
            get
            {
                return new GenerateNuGetSpecTask<BuildRosaliaContext>()
                    .FillInput(c =>
                        new SpecInput()
                            .Id("Rosalia")
                            .Version("0.1.0.0")
                            .Authors("Eugene Guryanov")
                            .Description("Simple workflow execution framework/tool that could be used for build scripts")
                            .WithFile(string.Format(@"bin\{0}\Rosalia.exe", c.Data.Configuration), "tools")
                            .WithFiles(GetRunnerDllFiles(c), "tools")
                            .ToFile(GetConsoleRunnerSpecFile(c)));
            }
        }

        private static IEnumerable<IFile> GetRunnerDllFiles(ExecutionContext<BuildRosaliaContext> c)
        {
            return
                c.FileSystem.GetFilesRecursively(c.Data.Src.GetDirectory(string.Format(@"Rosalia.Runner.Console\bin\{0}", c.Data.Configuration)))
                    .Filter(fileName => fileName.EndsWith(".dll"))
                    .Exclude(fileName => fileName.Contains(".TaskLib."))
                    .All;
        }

        private static IFile GetConsoleRunnerSpecFile(ExecutionContext<BuildRosaliaContext> c)
        {
            return c.Data.Src.GetFile("Rosalia.Runner.Console\\Rosalia.Runner.Console.nuspec");
        }

        protected override void OnBeforeExecute(ExecutionContext<BuildRosaliaContext> context)
        {
            base.OnBeforeExecute(context);

            var data = context.Data;

            data.Configuration = "Debug";
            data.ProjectRootDirectory = context.WorkDirectory.Parent.Parent;
            data.Src = data.ProjectRootDirectory.GetDirectory("Src");
            data.SolutionFile = data.Src.GetFile("Rosalia.sln");
            data.Artifacts = data.ProjectRootDirectory.GetDirectory("Artifacts");
        }

        public override ITask<BuildRosaliaContext> CreateRootTask()
        {
            return MainSequence;
        }
    }
}