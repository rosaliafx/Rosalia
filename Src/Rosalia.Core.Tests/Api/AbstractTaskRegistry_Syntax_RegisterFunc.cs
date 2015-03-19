namespace Rosalia.Core.Tests.Api
{
    using NUnit.Framework;
    using Rosalia.Core.Api;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class AbstractTaskRegistry_Syntax_RegisterFunc
    {
        [Test]
        public void Register_FuncContextToResult_ShouldRegisterTask()
        {
            QuickWorkflow
                .Define(w => w.Task(
                    "fooTask",
                    context =>
                    {
                        context.Log.Info("Log message!");
                        return "foo".AsTaskResult();
                    }))
                .GetRegisteredTasks()
                .DefinitionById("fooTask")
                    .AssertDefined()
                    .AssertHasNoDependencies();
        }

        [Test]
        public void Register_FuncVoidToResult_ShouldRegisterTask()
        {
            QuickWorkflow
                .Define(w => w.Task(
                    "fooTask", 
                    () => "foo".AsTaskResult()))
                .GetRegisteredTasks()
                .DefinitionById("fooTask")
                    .AssertDefined()
                    .AssertHasNoDependencies();
        }

        [Test]
        public void Register_FuncContextToTask_ShouldRegisterTask()
        {
            QuickWorkflow
                .Define(w => w.Task(
                    "fooTask",
                    context =>
                    {
                        context.Log.Info("Hello!");
                        return Task.Const("foo");
                    }))
                .GetRegisteredTasks()
                .DefinitionById("fooTask")
                    .AssertDefined()
                    .AssertHasNoDependencies();
        }
    }
}