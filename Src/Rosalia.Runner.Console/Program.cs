namespace Rosalia.Runner.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.Watchers.Logging;

    public class Program
    {
        public static class ExitCode
        {
            public const int Ok = 0;

            public static class Error
            {
                public const int WorkflowFailure = -1;
                public const int RunnerInitializationFailure = -2;
                public const int OptionsParsingFailure = -3;
            }
        }

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
            var options = new ProgramOptions();

            try
            {
                optionsConfig = new OptionsConfig
                {
                    {
                        new Option("nl|nologo", "Do not show Rosalia logo"), 
                        v => options.NoLogo = true
                    },
                    {
                        new Option("h|help|?", "Show this help message"), 
                        v => options.ShowHelp = true
                    },
                    {
                        new Option("hl|hold", "Do not close console"), 
                        v => options.Hold = true
                    },
                    {
                        new Option("w|workflow", "Default workflow type (used if multiple workflows found)"), 
                        v => options.DefaultWorkflow = v
                    },
                    {
                        new Option("wd|workDirectory", "Work directory"), 
                        v => options.WorkDirectory = v
                    },
                    {
                        new Option("workflowBuildOutput", "Workflow project build output path"), 
                        v => options.WorkflowBuildOutputPath = v
                    },
                    {
                        new Option("workflowBuildConfiguration", "Workflow build configuration"), 
                        v => options.WorkflowProjectBuildConfiguration = v
                    },
                    {
                        new Option("vis|visualize", "Path to a graphical file to visualize a workflow"), 
                        v => options.VisualisationFilesPath.Add(v)
                    },
                    {
                        new Option("l|log", "Path to a file to write log to"), 
                        v => options.LogFilesPath.Add(v)
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

            foreach (var path in options.LogFilesPath)
            {
                var currentPath = path;
                if (Path.GetExtension(path).Equals(".html", StringComparison.InvariantCultureIgnoreCase))
                {
                    logRenderers.Add(new HtmlLogRenderer(new Lazy<TextWriter>(() => File.CreateText(currentPath))));
                }
            }

            ILogRenderer logRenderer = new CompositeLogRenderer(logRenderers);

            using (var runner = new Runner())
            {
                

                var initializationResult = runner.Init(new RunningOptions
                {
                    InputFile = options.InputFile,
                    DefaultWorkflowType = options.DefaultWorkflow,
                    WorkflowBuildOutputPath = options.WorkflowBuildOutputPath,
                    WorkflowProjectBuildConfiguration = options.WorkflowProjectBuildConfiguration,
                    LogRenderer = logRenderer,
                    WorkDirectory = workDirectory,
                    VisualisationFilesPath = options.VisualisationFilesPath,
                });

                if (!initializationResult.IsSuccess)
                {
                    return Exit(options.Hold, ExitCode.Error.RunnerInitializationFailure);
                }

                Console.WriteLine();
                var result = runner.ExecuteDefaultWorkflow();
                if (result.ResultType == ResultType.Success)
                {
                    return Exit(options.Hold, ExitCode.Ok);
                }

                return Exit(options.Hold, ExitCode.Error.WorkflowFailure);
            }
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
            Console.WriteLine("    WROKFLOW_DLL       Absolute or relative path to a workflow dll file");
            Console.WriteLine("    WROKFLOW_PROJECT   Absolute or relative path to a workflow csprj file");
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
