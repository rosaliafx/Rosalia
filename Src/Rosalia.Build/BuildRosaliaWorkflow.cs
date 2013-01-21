namespace Rosalia.Build
{
    using Rosalia.Core;
    using Rosalia.TaskLib.MsBuild;

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
                    .WithSubtask((builder, context) =>
                    {
                        var directory = context.Data.Src.GetDirectory("Rosalia.Runner.Console").GetDirectory("bin").GetDirectory(context.Data.Configuration);
                        context.FileSystem.CopyFilesToDirectory(
                            context.FileSystem.GetFilesRecursively(directory).Exclude(file => file.AbsolutePath.EndsWith("pdb")), 
                            context.Data.Artifacts);
                    });
            }
        }

        public override void OnBeforeExecute(ExecutionContext<BuildRosaliaContext> context)
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