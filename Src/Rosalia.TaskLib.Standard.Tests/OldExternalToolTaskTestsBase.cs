namespace Rosalia.TaskLib.Standard.Tests
{
    using System;
    using Moq;
    using Rosalia.Core;
    using Rosalia.Core.Tests;
    using Rosalia.TaskLib.Standard.Input;
    using Rosalia.TaskLib.Standard.Tasks;

    [Obsolete]
    public abstract class OldExternalToolTaskTestsBase<TContext, TInput, TResult> : TaskTestsBase<TContext>
        where TInput : ExternalToolInput, new() where TResult : class
        where TContext : new()
    {
        public void AssertCommand(OldExternalToolTask<TContext, TInput, TResult> task, Action<string, string> assertAction)
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
            OldExternalToolTask<TContext, TInput, TResult> task, 
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
            
            if (task.InputProvider == null)
            {
                task.InputProvider = taskContext => new TInput
                {
                    ToolPath = "fakeToolPath"  // tool path is required for most of external tasks
                };
            }

            task.ProcessStarter = processStarter.Object;
            task.ApplyResult((actualResult, actualContext) => { result = actualResult; });

            var context = CreateContext();
            var taskResult = context.Executer.Execute(task);

            assertResultAction(result, taskResult);
        }
    }
}