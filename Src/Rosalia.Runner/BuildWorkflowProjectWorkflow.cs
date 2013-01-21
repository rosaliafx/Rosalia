namespace Rosalia.Runner
{
    using Rosalia.Core;
    using Rosalia.TaskLib.MsBuild;

    public class BuildWorkflowProjectWorkflow : Workflow<RunningOptions>
    {
        public override ITask<RunningOptions> CreateRootTask()
        {
            return new MsBuildTask<RunningOptions>()
                .FillInput(c => new MsBuildInput()
                    .WithProjectFile(c.Data.InputFile)
                    .WithConfiguration(Configuration(c)));
        }

        private static string Configuration(ExecutionContext<RunningOptions> executionContext)
        {
            if (!string.IsNullOrEmpty(executionContext.Data.WorkflowProjectBuildConfiguration))
            {
                return executionContext.Data.WorkflowProjectBuildConfiguration;
            }

            return "Debug";
        }
    }
}