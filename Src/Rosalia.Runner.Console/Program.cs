namespace Rosalia.Runner.Console
{
    using System;
    using Rosalia.Runner.Console.CommandLine.Support;
    using Rosalia.Runner.Console.Steps;

    public class Program
    {
        public static int Exit(bool hold, int exitCode)
        {
            if (hold)
            {
                Console.WriteLine();
                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();
            }

            return exitCode;
        }

        public static int Main(string[] args)
        {
            var context = new ProgramContext
            {
                Args = args
            };

            var steps = new ProgramSteps
            {
                new ParseInputStep(),
                new ShowLogoStep(),
                new ShowHelpStep(),
                new SetupWorkDirectoryStep(),
                new SetupLogRendererStep(),
                new AssertInputFileSetStep(),
                new SetupInputFileStep(),
                new InitializeWorkflowStep(),
                new ExecuteWorkflowStep()
            };

            int result = steps.Execute(context);
            bool hold = context.Options != null && context.Options.Hold;

            return Exit(hold, result);

//            OptionsConfig optionsConfig = null;
//            RosaliaOptions options = new RosaliaOptions();

//            try
//            {
//                optionsConfig = RosaliaOptionsConfig.Create(options);
//
//                new OptionsParser().Parse(args, optionsConfig);
//
//                if (args.Length > 0)
//                {
//                    options.InputFile = args[args.Length - 1];
//                }
//            }
//            catch (Exception ex)
//            {
//                ShowLogo();
//                ShowHelp(optionsConfig);
//
//                Console.WriteLine("An error occured while parsing input arguments: {0}", ex.Message);
//
//                return Exit(false, ExitCode.Error.OptionsParsingFailure);
//            }

//            if (!options.NoLogo)
//            {
//                ShowLogo();
//            }

//            if (options.ShowHelp)
//            {
//                ShowHelp(optionsConfig);
//                return Exit(options.Hold, ExitCode.Ok);
//            }

//            string workDirectory = SetupWorkDirectory(options);

//            using (ILogRenderer logRenderer = SetupLogRenderer(options))
//            {
//                var log = new LogHelper(message => logRenderer.Render(message, "Runner"));

//                if (options.InputFile == null)
//                {
//                    log.Error("Input file path is not set");
//                    ShowHelp(optionsConfig);
//                    return Exit(false, ExitCode.Error.OptionsParsingFailure);
//                }

//                DefaultDirectory workDirectoryObj = new DefaultDirectory(workDirectory);
//                IFile inputFile = GetInputFile(options, workDirectoryObj);
//                if (!inputFile.Exists)
//                {
//                    log.Error("Input file {0} does not exist!", inputFile.AbsolutePath);
//                    ShowHelp(optionsConfig);
//                    return Exit(false, ExitCode.Error.OptionsParsingFailure);
//                }

//                InitializationResult initializationResult = InitializeWorkflow(options, workDirectoryObj, logRenderer, inputFile);
//
//                if (!initializationResult.IsSuccess)
//                {
//                    return Exit(options.Hold, ExitCode.Error.RunnerInitializationFailure);
//                }
//
//                Console.WriteLine();

//                ITaskResult<Nothing> result = ExecuteWorkflow(
//                    initializationResult.Workflow, 
//                    options.Tasks, 
//                    logRenderer, 
//                    workDirectoryObj);
//
//                if (result.IsSuccess)
//                {
//                    return Exit(options.Hold, ExitCode.Ok);
//                }
//            }

//            return Exit(options.Hold, ExitCode.Error.WorkflowFailure);
        }

//        private static ITaskResult<Nothing> ExecuteWorkflow(
//            IWorkflow workflow, 
//            Identities tasks,
//            ILogRenderer logRenderer, 
//            DefaultDirectory workDirectoryObj)
//        {
//            var executer = new SubflowTask<Nothing>(
//                workflow,
//                tasks);
//
//            var context = new TaskContext(
//                new ParallelExecutionStrategy(),
//                logRenderer,
//                workDirectoryObj,
//                new DefaultEnvironment());
//
//            return executer.Execute(context);
//        }
//
//        private static InitializationResult InitializeWorkflow(
//            RosaliaOptions options, 
//            DefaultDirectory workDirectory,
//            ILogRenderer logRenderer, 
//            IFile inputFile)
//        {
//            var runner = new Runner();
//
//            var runningOptions = new RunningOptions
//            {
//                InputFile = inputFile,
//                WorkflowBuildOutputPath = options.WorkflowBuildOutputPath,
//                WorkflowProjectBuildConfiguration = options.WorkflowProjectBuildConfiguration,
//                LogRenderer = logRenderer,
//                WorkDirectory = workDirectory,
//                Properties = options.Properties
//            };
//
//            return runner.Init(runningOptions);
//        }

//        private static ILogRenderer SetupLogRenderer(RosaliaOptions options)
//        {
//            var logRenderers = new List<ILogRenderer>
//            {
//                new ColoredConsoleLogRenderer()
//            };
//
//            foreach (var path in options.OutputFiles)
//            {
//                var currentPath = path;
//                if (Path.GetExtension(path).Equals(".html", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    logRenderers.Add(new HtmlLogRenderer(new Lazy<TextWriter>(() => File.CreateText(currentPath))));
//                }
//            }
//
//            return new CompositeLogRenderer(logRenderers.ToArray());
//        }

//        private static string SetupWorkDirectory(RosaliaOptions options)
//        {
//            var workDirectory = string.IsNullOrEmpty(options.WorkDirectory)
//                ? Directory.GetCurrentDirectory()
//                : options.WorkDirectory;
//
//            if (!Path.IsPathRooted(workDirectory))
//            {
//                workDirectory = Path.Combine(Directory.GetCurrentDirectory(), workDirectory);
//            }
//
//            Directory.SetCurrentDirectory(workDirectory);
//            return workDirectory;
//        }

//        private static IFile GetInputFile(RosaliaOptions options, DefaultDirectory workDirectory)
//        {
//            var inputFile = options.InputFile;
//            if (string.IsNullOrEmpty(inputFile))
//            {
//                return null;
//                
//            }
//
//            IFile result = Path.IsPathRooted(inputFile)
//                ? new DefaultFile(inputFile)
//                : workDirectory[options.InputFile].AsFile();
//
//            return result;
//        }

        

        
    }
}