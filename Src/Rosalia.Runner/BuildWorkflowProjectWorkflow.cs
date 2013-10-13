namespace Rosalia.Runner
{
    using Rosalia.Core;
    using Rosalia.TaskLib.MsBuild;

    public class BuildWorkflowProjectWorkflow : GenericWorkflow<RunningOptions>
    {
        public override void RegisterTasks()
        {
            Register(
                task: new MsBuildTask(),
                beforeExecute: task => task
                    .WithProjectFile(Data.InputFile)
                    .WithConfiguration(Configuration()));
        }

        private string Configuration()
        {
            if (!string.IsNullOrEmpty(Data.WorkflowProjectBuildConfiguration))
            {
                return Data.WorkflowProjectBuildConfiguration;
            }

            return "Debug";
        }
    }
}