namespace Rosalia.Runner.Console.Steps
{
    using Rosalia.Core.Logging;
    using Rosalia.FileSystem;
    using Rosalia.Runner.Console.CommandLine;
    using Rosalia.Runner.Console.CommandLine.Support;
    using Rosalia.Runner.Console.Startup;

    public class InitializeWorkflowStep : IProgramStep
    {
        public int? Execute(ProgramContext context)
        {
            InitializationResult initializationResult = InitializeWorkflow(
                context.Options, 
                context.WorkDirectory, 
                context.LogRenderer, 
                context.InputFile);

            if (!initializationResult.IsSuccess)
            {
                return ExitCode.Error.RunnerInitializationFailure;
            }

            context.Workflow = initializationResult.Workflow;

            System.Console.WriteLine();
            return null;
        }

        private static InitializationResult InitializeWorkflow(
            RosaliaOptions options,
            IDirectory workDirectory,
            ILogRenderer logRenderer,
            IFile inputFile)
        {
            var runner = new Runner();

            var runningOptions = new RunningOptions
            {
                InputFile = inputFile,
                WorkflowBuildOutputPath = options.WorkflowBuildOutputPath,
                WorkflowProjectBuildConfiguration = options.WorkflowProjectBuildConfiguration,
                LogRenderer = logRenderer,
                WorkDirectory = workDirectory,
                Properties = options.Properties
            };

            return runner.Init(runningOptions);
        }
    }
}