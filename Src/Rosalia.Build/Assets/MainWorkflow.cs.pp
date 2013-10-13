namespace $rootnamespace$
{
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;

    public class MainWorkflow : Workflow
    {
        public override void RegisterTasks()
        {
            Register(
                name: "Task 1",
                task: () => {
                    Result.AddInfo("Hello...")
                });

            Register(
                name: "Task 2",
                task: () => {
                    Result.AddInfo("...world")
                });
        }
    }
}