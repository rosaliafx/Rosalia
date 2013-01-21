namespace Rosalia.Core.Tests
{
    using Moq;
    using NUnit.Framework;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Result;
    using Rosalia.Core.Tests.Stubs;

    [TestFixture]
    public abstract class TaskTestsBase<T> where T : new()
    {
        private Mock<IExecuter<T>> _executer;
        private LoggerStub _logger;
        private Mock<IDirectory> _workDirectory;
        private T _data;

        public Mock<IExecuter<T>> Executer
        {
            get { return _executer; }
        }

        protected LoggerStub Logger
        {
            get { return _logger; }
        }

        protected Mock<IDirectory> WorkDirectory
        {
            get { return _workDirectory; }
        }

        protected static void AssertWasNotExecuted(Mock<ITask<T>> child)
        {
            child.Verify(t => t.Execute(It.IsAny<ExecutionContext<T>>()), Times.Never());
        }

        protected static void AssertWasExecuted(Mock<ITask<T>> child)
        {
            child.Verify(t => t.Execute(It.IsAny<ExecutionContext<T>>()), Times.Once());
        }

        [SetUp]
        public void Init()
        {
            _data = new T();
            _executer = new Mock<IExecuter<T>>();
            _logger = new LoggerStub();
            _workDirectory = new Mock<IDirectory>();

            Executer
                .Setup(x => x.Execute(It.IsAny<ITask<T>>()))
                .Returns((ITask<T> task) =>
                {
                    if (task is AbstractTask<T>)
                    {
                        ((AbstractTask<T>)task).UnswallowedExceptionTypes = new[]
                        {
                            typeof(AssertionException),
                            typeof(MockException)
                        };
                    }

                    return task.Execute(CreateContext());
                });
        }

        protected Mock<ITask<T>> CreateTask(ResultType resultType = ResultType.Success)
        {
            var mock = new Mock<ITask<T>>();
            mock.Setup(x => x.Execute(It.IsAny<ExecutionContext<T>>()))
                .Returns(new ExecutionResult(resultType, null));

            return mock;
        }

        protected ExecutionContext<T> CreateContext()
        {
            return new ExecutionContext<T>(_data, Executer.Object, Logger, WorkDirectory.Object);
        }

        protected ExecutionResult Execute(ITask<T> task)
        {
            return CreateContext().Executer.Execute(task);
        }
    }
}