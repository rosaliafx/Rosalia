namespace Rosalia.Build
{
    using Rosalia.Core;
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
                    .WithSubtask(GenerateNuGetSpec);
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
                            .Description("Simple workflow execution framework/tool that could be used for build scripts")
                            .WithFile(string.Format(@"bin\{0}\Rosalia.exe", c.Data.Configuration), "tools")
                            .ToFile(c.Data.Src.GetFile("Rosalia.Runner.Console\\Rosalia.Runner.Console.nuspec")));
            }
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