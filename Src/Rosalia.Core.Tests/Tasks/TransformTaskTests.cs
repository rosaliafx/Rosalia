namespace Rosalia.Core.Tests.Tasks
{
    using System;
    using NUnit.Framework;
    using Rosalia.Core.Tasks;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class TransformTaskTests
    {
        [Test]
        public void Execute_NestedTaskSucceed_ShouldTransform()
        {
            var task = new TransformTask<string, string>(
                Task.Const("foo"),
                val => val.ToUpper());

            task.Execute().AssertDataIs("FOO");
        }

        [Test]
        public void Execute_NestedTaskFailed_ShouldFail()
        {
            var task = new TransformTask<string, string>(
                Task.Failure<string>("error message"),
                val => val.ToUpper());

            task.Execute().AssertFailedWith("error message");
        }

        [Test]
        public void Execute_TransformerFailed_ShouldReturnFailure()
        {
            var task = new TransformTask<string, string>(
                Task.Const("foo"),
                val => { throw new Exception("An error has occurred"); });

            task.Execute().AssertFailedWith("An error has occurred");
        }
    }
}