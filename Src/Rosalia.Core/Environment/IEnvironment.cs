namespace Rosalia.Core.Environment
{
    public interface IEnvironment
    {
        string this[string key] { get; set; }

        bool IsMono { get; }
    }
}