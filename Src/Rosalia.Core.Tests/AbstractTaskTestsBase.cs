namespace Rosalia.Core.Tests
{
    using System;
    using NUnit.Framework;
    using Rosalia.Core.Tasks;

    public abstract class AbstractTaskTestsBase<TTask> : TaskTestsBase where TTask : AbstractTask
    {
        private TTask _task;

        protected TTask Task
        {
            get { return _task; }
        }

        [Test]
        public void Execute_SucceedInBeforeExecuteCallback_ShouldSkipExecution()
        {
            Task.BeforeExecute = (context, result) => result.Succeed();
            Task.AfterExecute = context => Assert.Fail("Should not call after execute");

            Assert.That(Execute(Task).ResultType, Is.EqualTo(ResultType.Success));
        }

        [Test]
        public void Execute_FailInBeforeExecuteCallback_ShouldSkipExecution()
        {
            Task.BeforeExecute = (context, result) => result.Fail();
            Task.AfterExecute = context => Assert.Fail("Should not call after execute");

            Assert.That(Execute(Task).ResultType, Is.EqualTo(ResultType.Failure));
        }

        protected override void InternalInit()
        {
            base.InternalInit();

            _task = CreateTask();
        }

        protected virtual TTask CreateTask()
        {
            return Activator.CreateInstance<TTask>();
        }
    }
}