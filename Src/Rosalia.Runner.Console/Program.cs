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

            DisposeLogRenderer(context);

            return Exit(hold, result);
        }

        private static void DisposeLogRenderer(ProgramContext context)
        {
            if (context.LogRenderer != null)
            {
                try
                {
                    context.LogRenderer.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured while disposing: {0}", ex.Message);
                }
            }
        }
    }
}