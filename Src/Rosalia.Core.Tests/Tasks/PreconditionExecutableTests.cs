namespace Rosalia.Core.Tests.Tasks
{
    using NUnit.Framework;
    using Rosalia.Core.Tasks;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class PreconditionExecutableTests
    {
        [Test]
        public void PreconditionTrue_ShouldExecuteNestedTask()
        {
            var precondition = new PreconditionTask<string>(
                Task.Const("foo"),
                () => true);

            var result = TaskExtensions.Execute(precondition);

            result.AssertSuccess();
            result.AssertDataIs("foo");
        }

        [Test]
        public void PreconditionFalse_ShouldReturnValueFromProvider()
        {
            var precondition = new PreconditionTask<string>(
                Task.Const("foo"),
                () => false,
                () => "bar");

            var result = TaskExtensions.Execute(precondition);

            result.AssertSuccess();
            result.AssertDataIs("bar");
        }

        [Test]
        public void PreconditionFalse_ShouldReturnDefaultValue()
        {
            var precondition = new PreconditionTask<string>(
                Task.Const("foo"),
                () => false,
                "bar");

            var result = TaskExtensions.Execute(precondition);

            result.AssertSuccess();
            result.AssertDataIs("bar");
        }

        [Test]
        public void PreconditionFalseNoDefault_ShouldReturnNull()
        {
            var precondition = new PreconditionTask<string>(
                Task.Const("foo"),
                () => false);

            var result = TaskExtensions.Execute(precondition);

            result.AssertSuccess();
            result.AssertDataIs(null);
        }
    }
}