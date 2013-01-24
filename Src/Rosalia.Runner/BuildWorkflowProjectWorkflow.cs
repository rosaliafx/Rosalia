namespace Rosalia.Runner
{
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.TaskLib.MsBuild;

    public class BuildWorkflowProjectWorkflow : Workflow<RunningOptions>
    {
        public override ITask<RunningOptions> RootTask
        {
            get
            {
                return new MsBuildTask<RunningOptions>()
                    .FillInput(c => new MsBuildInput()
                                        .WithProjectFile(c.Data.InputFile)
                                        .WithConfiguration(Configuration(c)));
            }
        }

        private static string Configuration(TaskContext<RunningOptions> taskContext)
        {
            if (!string.IsNullOrEmpty(taskContext.Data.WorkflowProjectBuildConfiguration))
            {
                return taskContext.Data.WorkflowProjectBuildConfiguration;
            }

            return "Debug";
        }
    }
}