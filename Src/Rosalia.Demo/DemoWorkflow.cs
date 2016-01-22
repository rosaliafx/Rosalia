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
                    return new
                    {
                        Name = "John",
                        Address = new {
                            Country = "Foo",
                            City = "Bar"
                        }
                    }.AsTaskResult();
                });

            Task(
                "Greet3",
                context =>
                {
                    context.Log.Error("Oh, no, {0}", 42);
                },
                DependsOn(greet2));

            Task(
                "",
                from v in greet2
                select new {}.AsTaskResult());
        }
    }
}