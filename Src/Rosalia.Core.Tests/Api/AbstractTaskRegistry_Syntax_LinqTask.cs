namespace Rosalia.Core.Tests.Api
{
    using System;
    using NUnit.Framework;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;

    [TestFixture]
    public class AbstractTaskRegistry_Syntax_LinqTask
    {
        [Test]
        public void Register_LinqSelect()
        {
            QuickWorkflow
                .Define(w =>
                {
                    var fooTask = w.Task("fooTask", Task.Const("foo"));

                    ITaskFuture<string> result = w.Task(
                        "result",
                        from foo in fooTask
                        select Task.Const("result"));
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

                    ITaskFuture<string> result = w.Task(
                        "result",
                        from foo in fooTask
                        from bar in barTask
                        select Task.Const("result"));
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

                    ITaskFuture<string> result = w.Task(
                        "result",
                        from foo in fooTask
                        from bar in barTask
                        from baz in bazTask
                        select Task.Const("result"));
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