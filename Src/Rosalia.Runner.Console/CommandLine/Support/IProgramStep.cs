namespace Rosalia.Runner.Console.CommandLine.Support
{
    public interface IProgramStep
    {
        int? Execute(ProgramContext context);
    }
}