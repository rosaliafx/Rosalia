namespace Rosalia.Runner.Console.Steps
{
    using Rosalia.Core;
    using Rosalia.Core.Api;
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Environment;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;
    using Rosalia.Runner.Console.CommandLine;
    using Rosalia.Runner.Console.CommandLine.Support;

    public class ExecuteWorkflowStep : IProgramStep
    {
        public int? Execute(ProgramContext context)
        {
            ITaskResult<Nothing> result = ExecuteWorkflow(
                    context.Workflow,
                    context.Options.Tasks,
                    context.LogRenderer,
                    context.WorkDirectory);

            if (!result.IsSuccess)
            {
                return ExitCode.Error.WorkflowFailure;
            }

            return null;
        }

        private static ITaskResult<Nothing> ExecuteWorkflow(
            IWorkflow workflow,
            Identities tasks,
            ILogRenderer logRenderer,
            IDirectory workDirectory)
        {
            var executer = new SubflowTask<Nothing>(
                workflow,
                tasks);

            var context = new TaskContext(
                new ParallelExecutionStrategy(),
                logRenderer,
                workDirectory,
                new DefaultEnvironment());

            return executer.Execute(context);
        }
    }
}