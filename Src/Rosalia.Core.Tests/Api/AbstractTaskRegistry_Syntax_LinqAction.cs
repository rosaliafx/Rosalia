namespace Rosalia.Core.Tests.Api
{
    using NUnit.Framework;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class AbstractTaskRegistry_Syntax_LinqAction
    {
        [Test]
        public void Register_LinqSelect()
        {
            QuickWorkflow
                .Define(w =>
                {
                    var fooTask = w.Task("fooTask", Task.Const("foo"));

                    ITaskFuture<Nothing> result = w.Task(
                        "result",
                        from foo in fooTask
                        select context =>
                        {
                            context.Log.Info("Log info!");
                        });
                })
                .GetRegisteredTasks()
                .DefinitionById("result")
                    .AssertDefined()
                    .AssertDependsOn("fooTask");
        }
 
        [Test]
        public void Register_LinqSelectMany()
        {
            QuickWorkflow
                .Define(w =>
                {
                    var fooTask = w.Task("fooTask", Task.Const("foo"));
                    var barTask = w.Task("barTask", Task.Const("bar"));

                    ITaskFuture<Nothing> result = w.Task(
                        "result",
                        from foo in fooTask
                        from bar in barTask
                        select context =>
                        {
                            context.Log.Info("Log info!");
                        });
                })
                .GetRegisteredTasks()
                .DefinitionById("result")
                    .AssertDefined()
                    .AssertDependsOn("fooTask")
                    .AssertDependsOn("barTask");
        }
 
        [Test]
        public void Register_LinqSelectManyWithIntermediate()
        {
            QuickWorkflow
                .Define(w =>
                {
                    var fooTask = w.Task("fooTask", Task.Const("foo"));
                    var barTask = w.Task("barTask", Task.Const("bar"));
                    var bazTask = w.Task("bazTask", Task.Const("baz"));

                    ITaskFuture<Nothing> result = w.Task(
                        "result",
                        from foo in fooTask
                        from bar in barTask
                        from baz in bazTask
                        select context =>
                        {
                            context.Log.Info("Log info!");
                        });
                })
                .GetRegisteredTasks()
                .DefinitionById("result")
                    .AssertDefined()
                    .AssertDependsOn("fooTask")
                    .AssertDependsOn("barTask")
                    .AssertDependsOn("bazTask");
        }
    }
}