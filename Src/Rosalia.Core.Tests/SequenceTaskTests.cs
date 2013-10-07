namespace Rosalia.Core.Tests
{
    using System.Collections.Generic;
    using Moq;
    using NUnit.Framework;
    using Rosalia.Core.Tasks.Flow;

    public class SequenceTaskTests : TaskTestsBase
    {
        [Test]
        public void Execute_SuccessfulSingleTask_ShouldSucceed()
        {
            var context = CreateContext();
            var task = new SequenceTask(new SuccessTask());

            var result = task.Execute(context);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Success));
        }

        [Test]
        public void Execute_SuccessfulSingleTask_ShouldCallChildExecute()
        {
            var context = CreateContext();
            var child = new SuccessTask();
            var task = new SequenceTask(child);

            task.Execute(context);

            Executer.Verify(e => e.Execute(child), Times.Once());
        }

        [Test]
        public void Execute_HasFailureTask_ShouldFail()
        {
            var context = CreateContext();
            var task = new SequenceTask(new SuccessTask(), new FailureTask());

            var result = task.Execute(context);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Failure));
        }

        [Test]
        public void Execute_MultipleTasks_ShouldStopOnFailure()
        {
            var context = CreateContext();
            var successChildren = new List<ITask>
            {
                new SuccessTask(),
                new SuccessTask(),
                new SuccessTask(),
                new SuccessTask()
            };

            var afterFailureTask = new List<ITask>
            {
                new SuccessTask(),
                new SuccessTask(),
                new SuccessTask(),
                new SuccessTask()
            };

            var failureChild = new FailureTask();
            var allChildren = new List<ITask>();
            
            allChildren.AddRange(successChildren);
            allChildren.Add(failureChild);
            allChildren.AddRange(afterFailureTask);

            var task = new SequenceTask(allChildren);

            task.Execute(context);

            foreach (var successChild in successChildren)
            {
                var child = successChild;
                Executer.Verify(e => e.Execute(child), Times.Once());
            }

            Executer.Verify(e => e.Execute(failureChild), Times.Once());

            foreach (var afterFailureChild in afterFailureTask)
            {
                var child = afterFailureChild;
                Executer.Verify(e => e.Execute(child), Times.Never());
            }
        }
    }
}