namespace Rosalia.Core.Tests.Engine.Execution
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using NUnit.Framework;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Interception;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public abstract class ExecutionStrategyTests
    {
        private static readonly Func<Identity, TaskContext> ContextFactory = id => TaskExtensions.CreateContext().CreateFor(id);

        protected abstract IExecutionStrategy CreateStrategy();

        [Test]
        public void Execute_ShouldCallInterceptorBeforeTaskExecution()
        {
            var strategy = CreateStrategy();
            var interceptor = new Mock<ITaskInterceptor>();

            var task = new ValTask<string>("foo");
            strategy.Execute(
                new LayerBuilder
                {
                    { "task1", task },
                },
                ContextFactory,
                interceptor.Object);

            interceptor.Verify(x => x.BeforeTaskExecute("task1", task, It.IsAny<TaskContext>()));
        }

        [Test]
        public void Execute_ShouldCallInterceptorAfterTaskExecution()
        {
            var strategy = CreateStrategy();
            var interceptor = new Mock<ITaskInterceptor>();

            var task = new ValTask<string>("foo");
            strategy.Execute(
                new LayerBuilder
                {
                    { "task1", task },
                },
                ContextFactory,
                interceptor.Object);

            interceptor.Verify(x => x.AfterTaskExecute("task1", task, It.IsAny<TaskContext>(), It.IsAny<ITaskResult<string>>()));
        }

        [Test]
        public void Execute_SuccessTasks_ShouldReturnResults()
        {
            var strategy = CreateStrategy();
            var result = strategy.Execute(
                new LayerBuilder
                {
                    { "task1", new ValTask<string>("foo") },
                    { "task2", new ValTask<string>("bar") }
                },
                ContextFactory, 
                null);

            result.AssertSuccess();

            var ids = result.Data.Select(x => x.Identity.Value).ToArray();

            Assert.That(ids, Contains.Item("task1"));
            Assert.That(ids, Contains.Item("task2"));
            Assert.That(result.Data.First(x => x.Identity.Value == "task1").Value, Is.EqualTo("foo"));
            Assert.That(result.Data.First(x => x.Identity.Value == "task2").Value, Is.EqualTo("bar"));
        }

        [Test]
        public void Execute_FailureTasks_ShouldReturnFailure()
        {
            var strategy = CreateStrategy();
            var result = strategy.Execute(
                new LayerBuilder
                {
                    { "task1", new ValTask<string>("foo") },
                    { "task2", new FailureTask<string>() }
                },
                ContextFactory, 
                null);

            result.AssertFailure();
        }
    }

    internal class LayerBuilder : IEnumerable<ExecutableWithIdentity>
    {
        private readonly IList<ExecutableWithIdentity> _items = new List<ExecutableWithIdentity>();

        public void Add<T>(Identity identity, ITask<T> task) where T : class
        {
            _items.Add(new ExecutableWithIdentity(task, identity));
        }

        public IEnumerator<ExecutableWithIdentity> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator Layer(LayerBuilder builder)
        {
            return new Layer(builder._items.ToArray());
        }
    }
}