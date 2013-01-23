namespace Rosalia.Core.Context.Environment
{
    using Rosalia.Core.FileSystem;

    public interface IEnvironment
    {
        IDirectory ProgramFilesX86 { get; }

        IDirectory ProgramFiles { get; }

        string GetVariable(string variable);

        void SetVariable(string variable, string value);
    }
}