namespace $rootnamespace$
{
    using Rosalia.Core.Api;

    public class MainWorkflow : Workflow
    {
        protected override void RegisterTasks()
        {
            var task1 = Task(
                "Task1",
                context => {
                    context.Log.Info("Hello...");
                });

            Task(
                "Task2",
                context => {
                    context.Log.Info("...world");
                },
				DependsOn(task1));
        }
    }
}