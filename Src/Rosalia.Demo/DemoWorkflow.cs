namespace Rosalia.Demo
{
    using Rosalia.Core.Api;

    public class DemoWorkflow : Workflow
    {
        public string Message { get; set; }

        protected override void RegisterTasks()
        {
            Task(
                "Greet1",
                context =>
                {
                    context.Log.Info("Hello, Rosalia!");
                });

            var greet2 = Task(
                "Greet2",
                context =>
                {
                    context.Log.Warning("Oh, no, {0}", Message);
                });

            Task(
                "Greet3",
                context =>
                {
                    context.Log.Error("Oh, no, {0}", 42);
                },
                DependsOn(greet2));
        }
    }
}