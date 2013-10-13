namespace Rosalia.TaskLib.Standard.Tests
{
    using System;
    using Moq;
    using Rosalia.Core;
    using Rosalia.Core.Tests;
    using Rosalia.TaskLib.Standard.Tasks;

    public abstract class ExternalToolTaskTestsBase<TResult> : AbstractTaskTestsBase<ExternalToolTask<TResult>> 
        where TResult : class
    {
        public void AssertCommand(ExternalToolTask<TResult> task, Action<string, string> assertAction)
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
                   assertAction(path, arguments);
               });

            task.ProcessStarter = processStarter.Object;

            var context = CreateContext();
            context.Executer.Execute(task);
        }

        public void AssertProcessOutputParsing(
            ExternalToolTask<TResult> task,
            string processOutput,
            Action<TResult, ExecutionResult> assertResultAction)
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

            var context = CreateContext();
            var taskResult = context.Executer.Execute(task);

            assertResultAction(task.Result, taskResult);
        }
    }
}