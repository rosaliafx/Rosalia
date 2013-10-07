namespace Rosalia.Core.Tests
{
    using System;
    using Moq;
    using NUnit.Framework;
    using Rosalia.Core.Tasks.Flow;

    public class ForkTaskTest : TaskTestsBase
    {
        [Test]
        public void Execute_ConditionIsTrue_ShouldExecuteChild()
        {
            var child = new Mock<ITask>();

            var task = new ForkTask()
                .If(c => true)
                .Then(child.Object);

            Execute(task);

            AssertWasExecuted(child);
        }

        [Test]
        public void Execute_ConditionIsFalse_ShouldNotExecuteChild()
        {
            var child = new Mock<ITask>();

            var task = new ForkTask()
                .If(c => false)
                .Then(child.Object);

            Execute(task);

            AssertWasNotExecuted(child);
        }

        [Test]
        public void Execute_ChildFailed_ShouldFail()
        {
            var task = new ForkTask()
                .If(c => true)
                .Then(new FailureTask());

            var result = Execute(task);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Failure));
        }

        [Test]
        public void Execute_MultipleTask_ShouldProceedOnStepComplete()
        {
            var child1 = CreateTask();
            var child2 = CreateTask();

            var task = new ForkTask()
                .If(c => true).Then(child1.Object)
                .If(c => true).Then(child2.Object);

            Execute(task);

            AssertWasExecuted(child1);
            AssertWasExecuted(child2);
        }

        [Test]
        public void Execute_MultipleTaskWithElse_ShouldBreakOnStepComplete()
        {
            var child1 = CreateTask();
            var child2 = CreateTask();

            var task = new ForkTask()
                .If(c => true).Then(child1.Object)
                .Else().If(c => true).Then(child2.Object);

            Execute(task);

            AssertWasExecuted(child1);
            AssertWasNotExecuted(child2);
        }

        [Test] 
        [ExpectedException(ExpectedException = typeof(InvalidOperationException))]
        public void Else_NoChildren_ShouldFail()
        {
            var task = new ForkTask();

            task.Else();
        }
    }
}