namespace Rosalia.TestingSupport.Helpers
{
    using System;
    using System.IO;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using Rosalia.Core;
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Environment;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;
    using Rosalia.TaskLib.Standard;
    using Rosalia.TaskLib.Standard.Tasks;
    using Rosalia.TestingSupport.FileSystem;

    public static class TaskExtensions
    {
        public static ResultWrapper<T> Execute<T>(this ITask<T> task, IDirectory workDirectory = null) where T : class
        {
            var spyLogRenderer = new SpyLogRenderer();
            var logRenderer = new CompositeLogRenderer(
                new SimpleLogRenderer(),
                spyLogRenderer);

            ITaskResult<T> result = task.Execute(CreateContext(workDirectory, logRenderer).CreateFor(new Identity("TestTask")));

            return new ResultWrapper<T>(
                result,
                spyLogRenderer.Messages);
        }

        public static TaskContext CreateContext(IDirectory workDirectory = null, ILogRenderer logRenderer = null, IEnvironment environment = null)
        {
            return new TaskContext(
                new SequenceExecutionStrategy(), 
                logRenderer ?? new SimpleLogRenderer(), 
                workDirectory ?? new DirectoryStub(Directory.GetCurrentDirectory()),
                environment ?? new DefaultEnvironment());
        }

        public static void AssertCommand<TResult>(this ExternalToolTask<TResult> task, Action<string, string> assertAction) where TResult : class
        {
            AssertCommand<TResult>(task, null, assertAction);
        }

        public static void AssertCommand<TResult>(this ExternalToolTask<TResult> task, IDirectory workDirectoryStub, Action<string, string> assertAction) where TResult : class
        {
            task.UnswallowedExceptionTypes = new[]
            {
                typeof (NUnitException),
                typeof (ResultStateException)
            };

            var processStarter = new Mock<IProcessStarter>();
            processStarter
                .Setup(x => x.StartProcess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<string>>(),
                    It.IsAny<Action<string>>()))
               .Callback((string path, string arguments, string workDirectory, Action<string> onInfo, Action<string> onError) =>
               {
                   assertAction(path, arguments);
               });

            task.ProcessStarter = processStarter.Object;

            task.Execute(workDirectoryStub).AssertSuccess();
        }

        public static void AssertProcessOutputParsing<TResult>(
            this ExternalToolTask<TResult> task,
            string processOutput,
            Action<ResultWrapper<TResult>> assertResultAction) where TResult : class
        {
            var processStarter = new Mock<IProcessStarter>();
            processStarter
                .Setup(x => x.StartProcess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<string>>(),
                    It.IsAny<Action<string>>()))
               .Callback((string path, string arguments, string workDirectory, Action<string> onInfo, Action<string> onError) =>
               {
                   if (!string.IsNullOrEmpty(processOutput))
                   {
                       foreach (var line in processOutput.Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                       {
                           onInfo(line);
                       }
                   }
               });

            if (task.ToolPath == null)
            {
                task.ToolPath = "fakeToolPath";  // tool path is required for most of external tasks
            }

            task.ProcessStarter = processStarter.Object;

            var taskResult = Execute(task);

            assertResultAction(taskResult);
        }


    }
}