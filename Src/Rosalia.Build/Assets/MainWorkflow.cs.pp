namespace $rootnamespace$
{
    using Rosalia.Core.Api;

    public class MainWorkflow : Workflow
    {
        public override void RegisterTasks()
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