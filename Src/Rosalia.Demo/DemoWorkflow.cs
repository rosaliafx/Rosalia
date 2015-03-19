namespace Rosalia.Demo
{
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks.Results;

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
                    return new
                    {
                        Name = "John",
                        Address = new {
                            Country = "Foo",
                            City = "Bar"
                        }
                    }.AsTaskResult();
                });

            string x = greet2.FetchValue(null).Name;

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