namespace Rosalia.TaskLib.Standard.Tests
{
    using System;
    using Moq;
    using Rosalia.Core;
    using Rosalia.Core.Tests;

    public abstract class ExternalToolTaskTestsBase<TContext, TInput, TResult> : TaskTestsBase<TContext>
        where TInput : class
        where TResult : class
        where TContext : new()
    {
        public void AssertCommand(ExternalToolTask<TContext, TInput, TResult> task, Action<string, string> assertAction)
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
            ExternalToolTask<TContext, TInput, TResult> task, 
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
                       foreach (var line in processOutput.Split(new []{ System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                       {
                           onInfo(line);
                       }
                   }
               });

            TResult result = null;
            task.ProcessStarter = processStarter.Object;
            task.ApplyResult((actualResult, actualContext) => { result = actualResult; });

            var context = CreateContext();
            var taskResult = context.Executer.Execute(task);

            assertResultAction(result, taskResult);
        }
    }
}