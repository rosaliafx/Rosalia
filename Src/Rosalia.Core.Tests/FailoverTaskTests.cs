namespace Rosalia.Core.Tests
{
    using NUnit.Framework;
    using Rosalia.Core.Tasks.Flow;

    public class FailoverTaskTests : TaskTestsBase<object>
    {
        [Test]
        public void Execute_TargetTaskSucceed_ShouldSucceed()
        {
            var targetTask = CreateTask();
            var failoverTask = CreateTask();

            var task = new FailoverTask<object>(targetTask.Object, failoverTask.Object);

            var result = Execute(task);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Success));
        }

        [Test]
        public void Execute_TargetTaskFailedAndFailoverSucceed_ShouldSucceed()
        {
            var targetTask = CreateTask(ResultType.Failure);
            var failoverTask = CreateTask();

            var task = new FailoverTask<object>(targetTask.Object, failoverTask.Object);

            var result = Execute(task);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Success));
        }

        [Test]
        public void Execute_TargetTaskFailedAndFailoverFailed_ShouldFail()
        {
            var targetTask = CreateTask(ResultType.Failure);
            var failoverTask = CreateTask(ResultType.Failure);

            var task = new FailoverTask<object>(targetTask.Object, failoverTask.Object);

            var result = Execute(task);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Failure));
        }

        [Test]
        public void Execute_TargetTaskSucceed_ShouldNotExecuteFailover()
        {
            var targetTask = CreateTask();
            var failoverTask = CreateTask();

            var task = new FailoverTask<object>(targetTask.Object, failoverTask.Object);

            Execute(task);

            AssertWasExecuted(targetTask);
            AssertWasNotExecuted(failoverTask);
        }

        [Test]
        public void Execute_TargetTaskFailed_ShouldExecuteFailover()
        {
            var targetTask = CreateTask(ResultType.Failure);
            var failoverTask = CreateTask();

            var task = new FailoverTask<object>(targetTask.Object, failoverTask.Object);

            Execute(task);

            AssertWasExecuted(targetTask);
            AssertWasExecuted(failoverTask);
        }
    }
}