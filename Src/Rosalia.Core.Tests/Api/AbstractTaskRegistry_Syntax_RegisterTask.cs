namespace Rosalia.Core.Tests.Api
{
    using NUnit.Framework;
    using Rosalia.Core.Api;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class AbstractTaskRegistry_Syntax_RegisterTask : TaskRegistryApiHelper
    {
        [Test]
        public void Test_SingleTaskWorkflow()
        {
            var task = Task.Const("foo");

            QuickWorkflow
                .Define(w => w.Task("fooTask", task))
                .GetRegisteredTasks()
                .DefinitionByExecutable(task)
                    .AssertDefined()
                    .AssertHasNoDependencies();
        }

        [Test]
        public void Test_MultipleAnonymousTaskWorkflow()
        {
            var task1 = Task.Const("foo");
            var task2 = Task.Const("bar");

            QuickWorkflow
                .Define(w =>
                {
                    w.Task(null, task1);
                    w.Task(null, task2);
                })
                .GetRegisteredTasks()
                    .DefinitionByExecutable(task1)
                        .AssertDefined()
                        .AssertHasNoDependencies().And()
                    .DefinitionByExecutable(task2)
                        .AssertDefined()
                        .AssertHasNoDependencies().And();
        }

        [Test]
        public void Test_MultipleNamedTaskWorkflow()
        {
            var task1 = Task.Const("foo");
            var task2 = Task.Const("bar");

            QuickWorkflow
                .Define(w =>
                {
                    w.Task("fooTask", task1);
                    w.Task("barTask", task2);
                })
                .GetRegisteredTasks()
                    .DefinitionById("fooTask")
                        .AssertDefined()
                        .AssertHasNoDependencies().And()
                    .DefinitionById("barTask")
                        .AssertDefined()
                        .AssertHasNoDependencies().And();
        }

        [Test]
        public void Test_ManualDependency()
        {
            QuickWorkflow
                .Define(workflow =>
                {
                    var parent = workflow.Task("parent", Task.Const("foo"));
                    workflow.Task("child", Task.Const("foo"), DependsOn(parent));
                })
                .GetRegisteredTasks()
                .DefinitionById("child").AssertDependsOn("parent");
        }
    }
}