namespace Rosalia.Runner.Console.Steps
{
    using System.IO;
    using Rosalia.FileSystem;
    using Rosalia.Runner.Console.CommandLine;
    using Rosalia.Runner.Console.CommandLine.Support;

    public class SetupInputFileStep : IProgramStep
    {
        public int UnhandledExceptionReturnCode
        {
            get { return ExitCode.Error.WrongInputFile; }
        }

        public int? Execute(ProgramContext context)
        {
            IFile inputFile = GetInputFile(context.Options, context.WorkDirectory);
            if (!inputFile.Exists)
            {
                context.Log.Error("Input file {0} does not exist!", inputFile.AbsolutePath);
                Utils.ShowHelp(context.OptionsConfig);
                
                return ExitCode.Error.WrongInputFile;
            }

            context.InputFile = inputFile;

            return null;
        }

        private static IFile GetInputFile(RosaliaOptions options, IDirectory workDirectory)
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
    }
}