namespace Rosalia.TaskLib.Standard
{
    public interface IExternalToolAware
    {
        string Path { get; }

        string Arguments { get; }
    }
}