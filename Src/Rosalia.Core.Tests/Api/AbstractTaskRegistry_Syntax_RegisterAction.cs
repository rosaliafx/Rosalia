namespace Rosalia.Core.Tests.Api
{
    using NUnit.Framework;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class AbstractTaskRegistry_Syntax_RegisterAction
    {
        [Test]
        public void Register_ActionWithContext_ShouldRegisterTask()
        {
            QuickWorkflow
                .Define(w => w.Task(
                    "fooTask",
                    context =>
                    {
                        context.Log.Info("Log message!");
                    }))
                .GetRegisteredTasks()
                .DefinitionById("fooTask")
                    .AssertDefined()
                    .AssertHasNoDependencies();
        }

        [Test]
        public void Register_Action_ShouldRegisterTask()
        {
            QuickWorkflow
                .Define(w => w.Task(
                    "fooTask",
                    () => { }))
                .GetRegisteredTasks()
                .DefinitionById("fooTask")
                    .AssertDefined()
                    .AssertHasNoDependencies();
        }
    }
}