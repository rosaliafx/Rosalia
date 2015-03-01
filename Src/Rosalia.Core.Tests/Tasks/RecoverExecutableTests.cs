namespace Rosalia.Core.Tests.Tasks
{
    using NUnit.Framework;
    using Rosalia.Core.Tasks;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class RecoverExecutableTests
    {
        [Test]
        public void NestedTaskSucceed_ShouldReturnTaskValue()
        {
            var recovery = new RecoverTask<string>(
                Task.Const("foo"),
                () => "bar");

            var result = recovery.Execute();

            result.AssertSuccess();
            result.AssertDataIs("foo");
        }

        [Test]
        public void NestedTaskFailed_ShouldReturnProviderValue()
        {
            var recovery = new RecoverTask<string>(
                Task.Failure<string>(),
                () => "bar");

            var result = recovery.Execute();

            result.AssertSuccess();
            result.AssertDataIs("bar");
        }

        [Test]
        public void NestedTaskFailed_ShouldReturnDefaultValue()
        {
            var recovery = new RecoverTask<string>(
                Task.Failure<string>(),
                "bar");

            var result = recovery.Execute();

            result.AssertSuccess();
            result.AssertDataIs("bar");
        }

        [Test]
        public void NestedTaskFailedAndNoDefaultValue_ShouldReturnNull()
        {
            var recovery = new RecoverTask<string>(Task.Failure<string>());

            var result = recovery.Execute();

            result.AssertSuccess();
            result.AssertDataIs(null);
        }
    }
}