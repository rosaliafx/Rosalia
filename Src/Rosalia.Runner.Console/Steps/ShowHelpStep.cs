namespace Rosalia.Runner.Console.Steps
{
    using Rosalia.Runner.Console.CommandLine;
    using Rosalia.Runner.Console.CommandLine.Support;

    public class ShowHelpStep : IProgramStep
    {
        public int UnhandledExceptionReturnCode
        {
            get { return ExitCode.Error.UnknownError; }
        }

        public int? Execute(ProgramContext context)
        {
            if (context.Options.ShowHelp)
            {
                Utils.ShowHelp(context.OptionsConfig);
                return ExitCode.Ok;
            }

            return null;
        }
    }
}