namespace Rosalia.Runner.Console.Steps
{
    using System.IO;
    using Rosalia.FileSystem;
    using Rosalia.Runner.Console.CommandLine;
    using Rosalia.Runner.Console.CommandLine.Support;

    public class SetupWorkDirectoryStep : IProgramStep
    {
        public int UnhandledExceptionReturnCode
        {
            get { return ExitCode.Error.UnknownError; }
        }

        public int? Execute(ProgramContext context)
        {
            var workDirectory = string.IsNullOrEmpty(context.Options.WorkDirectory)
                ? Directory.GetCurrentDirectory()
                : context.Options.WorkDirectory;

            if (!Path.IsPathRooted(workDirectory))
            {
                workDirectory = Path.Combine(Directory.GetCurrentDirectory(), workDirectory);
            }

            Directory.SetCurrentDirectory(workDirectory);

            context.WorkDirectory = new DefaultDirectory(workDirectory);

            return null;
        }
    }
}