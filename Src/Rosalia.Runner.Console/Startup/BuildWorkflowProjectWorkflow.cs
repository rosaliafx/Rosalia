namespace Rosalia.Runner.Console.Startup
{
    using Rosalia.Core.Api;
    using Rosalia.TaskLib.MsBuild;

    public class BuildWorkflowProjectWorkflow : Workflow
    {
        private readonly RunningOptions _runningOptions;

        public BuildWorkflowProjectWorkflow(RunningOptions runningOptions)
        {
            _runningOptions = runningOptions;
        }

        protected override void RegisterTasks()
        {
            Task(
                "Build workflow project",
                new MsBuildTask
                {
                    ProjectFile = _runningOptions.InputFile,
                    Switches =
                    {
                        MsBuildSwitch.Configuration(Configuration())
                    }
                });

//            Register(
//                task: new MsBuildTask(),
//                beforeExecute: task => task
//                    .WithProjectFile(Data.InputFile)
//                    .WithConfiguration(Configuration()));
        }

        private string Configuration()
        {
            if (!string.IsNullOrEmpty(_runningOptions.WorkflowProjectBuildConfiguration))
            {
                return _runningOptions.WorkflowProjectBuildConfiguration;
            }

            return "Debug";  // todo: move to msbuild constants
        }
    }
}