namespace Rosalia.Runner.Console.Steps
{
    using System;
    using Rosalia.Runner.Console.CommandLine;
    using Rosalia.Runner.Console.CommandLine.Support;

    public class ParseInputStep : IProgramStep
    {
        public int UnhandledExceptionReturnCode
        {
            get { return ExitCode.Error.OptionsParsingFailure; }
        }

        public int? Execute(ProgramContext context)
        {
            try
            {
                context.Options = new RosaliaOptions();
                context.OptionsConfig = RosaliaOptionsConfig.Create(context.Options);

                new OptionsParser().Parse(context.Args, context.OptionsConfig);

                if (context.Args.Length > 0)
                {
                    context.Options.InputFile = context.Args[context.Args.Length - 1];
                }
            }
            catch (Exception ex)
            {
                DisplayLogoAndHelp(context);

                throw new Exception("An error occured while parsing input arguments", ex);
            }

            return null;
        }

        private static void DisplayLogoAndHelp(ProgramContext context)
        {
            try
            {
                Utils.ShowLogo();
                Utils.ShowHelp(context.OptionsConfig);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}