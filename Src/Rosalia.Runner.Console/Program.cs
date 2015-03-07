namespace Rosalia.Runner.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Environment;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;
    using Rosalia.Runner.Console.CommandLine;
    using Rosalia.Runner.Console.CommandLine.Support;
    using Rosalia.Runner.Console.Startup;

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
            OptionsConfig optionsConfig = null;
            var options = new RosaliaOptions();

            try
            {
                optionsConfig = new OptionsConfig
                {
                    {
                        new Option("nl|nologo", "Do not show Rosalia logo"), 
                        (v, s) => options.NoLogo = true
                    },
                    {
                        new Option("h|help|?", "Show this help message"), 
                        (v, s) => options.ShowHelp = true
                    },
                    {
                        new Option("p|prop|property", "Set property"), 
                        (v, s) => options.Properties.Add(s, v)
                    },
                    {
                        new Option("hl|hold", "Do not close console"), 
                        (v, s) => options.Hold = true
                    },
                    {
                        new Option("wd|workDirectory", "Work directory"), 
                        (v, s) => options.WorkDirectory = v
                    },
                    {
                        new Option("workflowBuildOutput", "Workflow project build output path"), 
                        (v, s) => options.WorkflowBuildOutputPath = v
                    },
                    {
                        new Option("workflowBuildConfiguration", "Workflow build configuration"), 
                        (v, s) => options.WorkflowProjectBuildConfiguration = v
                    },
                    {
                        new Option("o|out|output", "Path to a file to write log to"), 
                        (v, s) => options.OutputFiles.Add(v)
                    }
                };

                new OptionsParser().Parse(args, optionsConfig);

                if (args.Length > 0)
                {
                    options.InputFile = args[args.Length - 1];
                }
            }
            catch (Exception)
            {
                // todo log exception message
                ShowLogo();
                ShowHelp(optionsConfig);

                return Exit(true, ExitCode.Error.OptionsParsingFailure);
            }

            if (!options.NoLogo)
            {
                ShowLogo();
            }

            if (options.ShowHelp)
            {
                ShowHelp(optionsConfig);
                return Exit(options.Hold, ExitCode.Ok);
            }

            var logRenderers = new List<ILogRenderer>
            {
                new ColoredConsoleLogRenderer()
            };

            var workDirectory = string.IsNullOrEmpty(options.WorkDirectory) ?
                    Directory.GetCurrentDirectory() :
                    options.WorkDirectory;

            if (!Path.IsPathRooted(workDirectory))
            {
                workDirectory = Path.Combine(Directory.GetCurrentDirectory(), workDirectory);
            }

            Directory.SetCurrentDirectory(workDirectory);

            foreach (var path in options.OutputFiles)
            {
                var currentPath = path;
                if (Path.GetExtension(path).Equals(".html", StringComparison.InvariantCultureIgnoreCase))
                {
                    logRenderers.Add(new HtmlLogRenderer(new Lazy<TextWriter>(() => File.CreateText(currentPath))));
                }
            }

            ILogRenderer logRenderer = new CompositeLogRenderer(logRenderers.ToArray());

            var runner = new Runner();
            var workDirectoryObj = new DefaultDirectory(workDirectory);
            var runningOptions = new RunningOptions
            {
                InputFile = GetInputFile(options, workDirectoryObj),
                WorkflowBuildOutputPath = options.WorkflowBuildOutputPath,
                WorkflowProjectBuildConfiguration = options.WorkflowProjectBuildConfiguration,
                LogRenderer = logRenderer,
                WorkDirectory = workDirectoryObj,
                Properties = options.Properties
            };

            var initializationResult = runner.Init(runningOptions);

            if (!initializationResult.IsSuccess)
            {
                return Exit(options.Hold, ExitCode.Error.RunnerInitializationFailure);
            }

            Console.WriteLine();

            var executer = new SubflowTask<Nothing>(
                initializationResult.Workflow,
                Identities.Empty);
            
            var context = new TaskContext(
                new ParallelExecutionStrategy(), 
                logRenderer,
                runningOptions.WorkDirectory,
                new DefaultEnvironment());
            
            var result = executer.Execute(context);

            logRenderer.Dispose();

            if (result.IsSuccess)
            {
                return Exit(options.Hold, ExitCode.Ok);
            }

            return Exit(options.Hold, ExitCode.Error.WorkflowFailure);
        }

        private static IFile GetInputFile(RosaliaOptions options, DefaultDirectory workDirectory)
        {
            var inputFile = options.InputFile;
            if (string.IsNullOrEmpty(inputFile))
            {
                return null;
                
            }

            IFile result = Path.IsPathRooted(inputFile)
                ? new DefaultFile(inputFile)
                : workDirectory[options.InputFile].AsFile();

            return result;
        }

        private static void ShowHelp(OptionsConfig optionsConfig)
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine();
            Console.WriteLine("        Rosalia.exe [OPTIONS] WORKFLOW_DLL");
            Console.WriteLine("   or   Rosalia.exe [OPTIONS] WORKFLOW_PROJECT");
            Console.WriteLine();
            Console.WriteLine("where");
            Console.WriteLine();
            Console.WriteLine("    WORKFLOW_DLL       Absolute or relative path to a workflow dll file");
            Console.WriteLine("    WORKFLOW_PROJECT   Absolute or relative path to a workflow csprj file");
            Console.WriteLine();
            Console.WriteLine("and options include");
            Console.WriteLine();

            foreach (var option in optionsConfig)
            {
                Console.WriteLine("    " + option.Key.Description);
                foreach (var aliase in option.Key.Aliases)
                {
                    Console.WriteLine("        /{0}", aliase);
                }

                Console.WriteLine();
            }
        }

        private static void ShowLogo()
        {
            Console.WriteLine(
@"
  _____                 _ _       
 |  __ \               | (_)      
 | |__) |___  ___  __ _| |_  __ _ 
 |  _  // _ \/ __|/ _` | | |/ _` |
 | | \ \ (_) \__ \ (_| | | | (_| |
 |_|  \_\___/|___/\__,_|_|_|\__,_|
");
            Console.WriteLine(" v{0}", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine();
        }
    }
}