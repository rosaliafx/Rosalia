namespace Rosalia.Core.Tests
{
    using System;
    using Moq;
    using NUnit.Framework;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tests.Stubs;

    [TestFixture]
    public abstract class TaskTestsBase<T> where T : new()
    {
        private Mock<IExecuter<T>> _executer;
        private MessageCollector _logger;
        private Mock<IDirectory> _workDirectory;
        private T _data;
        private EnvironmentStub _environment;

        public Mock<IExecuter<T>> Executer
        {
            get { return _executer; }
        }

        protected MessageCollector Logger
        {
            get { return _logger; }
        }

        protected Mock<IDirectory> WorkDirectory
        {
            get { return _workDirectory; }
        }

        public EnvironmentStub Environment
        {
            get { return _environment; }
        }

        protected T Data
        {
            get { return _data; }
        }

        protected static void AssertWasNotExecuted(Mock<ITask<T>> child)
        {
            child.Verify(t => t.Execute(It.IsAny<TaskContext<T>>()), Times.Never());
        }

        protected static void AssertWasExecuted(Mock<ITask<T>> child)
        {
            child.Verify(t => t.Execute(It.IsAny<TaskContext<T>>()), Times.Once());
        }

        [SetUp]
        public void Init()
        {
            _data = new T();
            _executer = new Mock<IExecuter<T>>();
            _logger = new MessageCollector();
            _workDirectory = new Mock<IDirectory>();
            _environment = new EnvironmentStub
            {
                ProgramFiles = new DirectoryStub(),
                ProgramFilesX86 = new DirectoryStub()
            };

            Executer
                .Setup(x => x.Execute(It.IsAny<ITask<T>>()))
                .Returns((ITask<T> task) =>
                {
                    EventHandler<TaskMessageEventArgs> taskOnMessagePosted = (sender, args) => Logger.RegisterMessage(args.Message);

                    task.MessagePosted += taskOnMessagePosted;

                    if (task is AbstractTask<T>)
                    {
                        ((AbstractTask<T>)task).UnswallowedExceptionTypes = new[]
                        {
                            typeof(AssertionException),
                            typeof(MockException)
                        };
                    }

                    var result = task.Execute(CreateContext());

                    task.MessagePosted -= taskOnMessagePosted;

                    return result;
                });
        }

        protected Mock<ITask<T>> CreateTask(ResultType resultType = ResultType.Success)
        {
            var mock = new Mock<ITask<T>>();
            mock.Setup(x => x.Execute(It.IsAny<TaskContext<T>>()))
                .Returns(new ExecutionResult(resultType));

            return mock;
        }

        protected TaskContext<T> CreateContext()
        {
            return new TaskContext<T>(
                Data, 
                Executer.Object, 
                WorkDirectory.Object,
                Environment);
        }

        protected ExecutionResult Execute(ITask<T> task)
        {
            return CreateContext().Executer.Execute(task);
        }
    }
}