namespace Rosalia.Runner
{
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.TaskLib.MsBuild;

    public class BuildWorkflowProjectWorkflow : Workflow<RunningOptions>
    {
        public override void RegisterTasks()
        {
            Register(
                task: new MsBuildTask<RunningOptions>(),
                beforeExecute: (context, task) => task
                    .WithProjectFile(context.Data.InputFile)
                    .WithConfiguration(Configuration(context)));
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