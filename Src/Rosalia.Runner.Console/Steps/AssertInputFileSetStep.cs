namespace Rosalia.Runner.Console.Steps
{
    using Rosalia.Runner.Console.CommandLine;
    using Rosalia.Runner.Console.CommandLine.Support;

    public class AssertInputFileSetStep : IProgramStep
    {
        public int UnhandledExceptionReturnCode
        {
            get { return ExitCode.Error.UnknownError; }
        }

        public int? Execute(ProgramContext context)
        {
            if (context.Options.InputFile == null)
            {
                context.Log.Error("Input file path is not set");
                Utils.ShowHelp(context.OptionsConfig);

                return ExitCode.Error.WrongInputFile;
            }

            return null;
        }
    }
}