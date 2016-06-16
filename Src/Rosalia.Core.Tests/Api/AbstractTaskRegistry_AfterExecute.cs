namespace Rosalia.Core.Tests.Api
{
    using System;
    using NUnit.Framework;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class AbstractTaskRegistry_AfterExecute
    {
        [Test]
        public void SuccessResult_AfterExecuteShouldBeCalled()
        {
            var isCalled = false;

            SubflowWithAfterExecute<string>
                .Define(
                    that => that.Task("foo", Task.Const("foo")),
                    (c, r) =>
                    {
                        isCalled = true;
                    })
                .Execute();

            Assert.That(isCalled, "AfterExecute is called");
        }

        [Test]
        public void SuccessResult_AfterExecuteShouldHaveAccessToResult()
        {
            var isCalled = false;

            SubflowWithAfterExecute<string>
                .Define(
                    that => that.Task("foo", Task.Const("foo")),
                    (_, result) =>
                    {
                        isCalled = true;

                        result
                            .AssertSuccess()
                            .AssertDataIs("foo");
                    })
                .Execute();

            Assert.That(isCalled, "AfterExecute is called");
        }

        [Test]
        public void SuccessResult_AfterExecuteShouldHaveAccessToResults()
        {
            var isCalled = false;

            SubflowWithAfterExecute<string>
                .Define(
                    that =>
                    {
                        var bar = that.Task("bar", Task.Const("barValue"));

                        return that.Task("foo", Task.Const("fooValue"), that.DependsOn(bar));
                    },
                    (context, _) =>
                    {
                        isCalled = true;

                        Assert.That(context.Results.GetValueOf<string>("foo"), Is.EqualTo("fooValue"));
                        Assert.That(context.Results.GetValueOf<string>("bar"), Is.EqualTo("barValue"));
                    })
                .Execute();

            Assert.That(isCalled, "AfterExecute is called");
        }

        [Test]
        public void FailureResult_AfterExecuteShouldHaveAccessToResult()
        {
            var isCalled = false;

            SubflowWithAfterExecute<string>
                .Define(
                    that => that.Task("foo", Task.Failure<string>("Error message")),
                    (_, result) =>
                    {
                        isCalled = true;

                        result.AssertFailure();
                    })
                .Execute()
                .AssertFailedWith("Error message");

            Assert.That(isCalled, "AfterExecute is called");
        }

        [Test]
        public void SuccessResult_AfterExecuteCouldSubstituteResult()
        {
            SubflowWithAfterExecute<string>
                .Define(
                    that => that.Task("foo", Task.Const("fooValue")),
                    (_, result) =>
                    {
                        result.AssertSuccess();

                        return new FailureResult<string>("AfterExecute check error");
                    })
                .Execute()
                .AssertFailedWith("AfterExecute check error");
        }

        [Test]
        public void SuccessResult_AfterExecuteCouldThrow()
        {
            SubflowWithAfterExecute<string>
                .Define(
                    that => that.Task("foo", Task.Const("fooValue")),
                    (_, result) =>
                    {
                        throw new Exception(result.Data + " is not valid");
                    })
                .Execute()
                .AssertFailedWith("fooValue is not valid");
        }
    }

    internal class SubflowWithAfterExecute<T> : QuickSubflow<T> where T : class
    {
        private readonly Func<TaskContext, ITaskResult<T>, ITaskResult<T>> _afterExecute;

        public SubflowWithAfterExecute(
            Func<TaskRegistry<T>, ITaskFuture<T>> defineAction, 
            Func<TaskContext, ITaskResult<T>, ITaskResult<T>> afterExecute) : base(defineAction)
        {
            _afterExecute = afterExecute;
        }

        public override ITaskResult<T> AfterExecute(TaskContext context, ITaskResult<T> result)
        {
            return _afterExecute.Invoke(context, result);
        }

        public static SubflowWithAfterExecute<T> Define(
            Func<TaskRegistry<T>, ITaskFuture<T>> defineAction, 
            Func<TaskContext, ITaskResult<T>, ITaskResult<T>> afterExecute)
        {
            return new SubflowWithAfterExecute<T>(defineAction, afterExecute);
        }

        public static SubflowWithAfterExecute<T> Define(
            Func<TaskRegistry<T>, ITaskFuture<T>> defineAction, 
            Action<TaskContext, ITaskResult<T>> afterExecute)
        {
            return new SubflowWithAfterExecute<T>(defineAction, (context, result) =>
            {
                afterExecute.Invoke(context, result);
                return result;
            });
        }
    }
}