namespace Rosalia.Demo
{
    using Rosalia.Core.Api;

    public class DemoWorkflow : Workflow
    {
        public string Message { get; set; }

        protected override void RegisterTasks()
        {
            Task(
                "Greet",
                context =>
                {
                    context.Log.Info("Hello, Rosalia!");
                    context.Log.Warning("Oh, no, {0}", Message);
                    context.Log.Error("Oh, no, {0}", 42);
                });
        }
    }
}