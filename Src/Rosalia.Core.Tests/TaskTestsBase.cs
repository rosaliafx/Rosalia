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
    public abstract class TaskTestsBase
    {
        private Mock<IExecuter> _executer;
        private MessageCollector _logger;
        private Mock<IDirectory> _workDirectory;
        private EnvironmentStub _environment;

        public Mock<IExecuter> Executer
        {
            get { return _executer; }
        }

        public EnvironmentStub Environment
        {
            get { return _environment; }
        }

        protected MessageCollector Logger
        {
            get { return _logger; }
        }

        protected Mock<IDirectory> WorkDirectory
        {
            get { return _workDirectory; }
        }

        protected static void AssertWasNotExecuted(Mock<ITask> child)
        {
            child.Verify(t => t.Execute(It.IsAny<TaskContext>()), Times.Never());
        }

        protected static void AssertWasExecuted(Mock<ITask> child)
        {
            child.Verify(t => t.Execute(It.IsAny<TaskContext>()), Times.Once());
        }

        [SetUp]
        public void Init()
        {
            _executer = new Mock<IExecuter>();
            _logger = new MessageCollector();
            _workDirectory = new Mock<IDirectory>();
            _environment = new EnvironmentStub
            {
                ProgramFiles = new DirectoryStub(),
                ProgramFilesX86 = new DirectoryStub()
            };

            Executer
                .Setup(x => x.Execute(It.IsAny<ITask>()))
                .Returns((ITask task) =>
                {
                    EventHandler<TaskMessageEventArgs> taskOnMessagePosted = (sender, args) => Logger.RegisterMessage(args.Message);

                    task.MessagePosted += taskOnMessagePosted;

                    if (task is AbstractTask)
                    {
                        ((AbstractTask)task).UnswallowedExceptionTypes = new[]
                        {
                            typeof(AssertionException),
                            typeof(MockException)
                        };
                    }

                    var result = task.Execute(CreateContext());

                    task.MessagePosted -= taskOnMessagePosted;

                    return result;
                });

            InternalInit();
        }

        protected virtual void InternalInit()
        {
        }

        protected Mock<ITask> CreateTask(ResultType resultType = ResultType.Success)
        {
            var mock = new Mock<ITask>();
            mock.Setup(x => x.Execute(It.IsAny<TaskContext>()))
                .Returns(new ExecutionResult(resultType));

            return mock;
        }

        protected TaskContext CreateContext()
        {
            return new TaskContext(
                Executer.Object, 
                WorkDirectory.Object,
                Environment);
        }

        protected ExecutionResult Execute(ITask task)
        {
            return CreateContext().Executer.Execute(task);
        }
    }
}