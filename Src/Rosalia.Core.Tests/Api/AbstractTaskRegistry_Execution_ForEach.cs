namespace Rosalia.Core.Tests.Api
{
    using System.Linq;
    using NUnit.Framework;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class AbstractTaskRegistry_Execution_ForEach
    {
        [Test]
        public void Execute_NestedTaskFails_ShouldFail()
        {
            IWorkflow workflow = QuickWorkflow.Define(w =>
            {
                w.Task(
                    "result",
                    w.ForEach(Enumerable.Range(1, 5)).Do(
                        i => i == 3 ? Task.Failure<Nothing>("3 is not available") : Task.Const(Nothing.Value),
                        i => i.ToString()));
            });

            workflow.Execute().AssertFailedWith("3 is not available");
        }
    }
}