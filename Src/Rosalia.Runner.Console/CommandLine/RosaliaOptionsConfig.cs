namespace Rosalia.Runner.Console.CommandLine
{
    using System;
    using Rosalia.Core;
    using Rosalia.Core.Logging;
    using Rosalia.Runner.Console.CommandLine.Support;

    public static class RosaliaOptionsConfig
    {
        public static OptionsConfig Create(RosaliaOptions options)
        {
            return new OptionsConfig
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
                    new Option("w|workflow", "Set workflow type (if multiples in assembly)"), 
                    (v, s) => options.Workflow = v
                },
                {
                    new Option("t|task", "Set task to run"), 
                    (v, s) => options.Tasks += new Identity(v)
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
                    new Option("ll|logLevel", "Log Level"), 
                    (v, s) => options.LogLevel = (MessageLevel?) Enum.Parse(typeof(MessageLevel), v)
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
        }
    }
}