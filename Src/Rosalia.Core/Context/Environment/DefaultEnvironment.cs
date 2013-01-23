namespace Rosalia.Core.Context.Environment
{
    using Rosalia.Core.FileSystem;

    public class DefaultEnvironment : IEnvironment
    {
        public IDirectory ProgramFilesX86 { get; private set; }

        public IDirectory ProgramFiles { get; private set; }

        public string GetVariable(string variable)
        {
            return System.Environment.GetEnvironmentVariable(variable);
        }

        public void SetVariable(string variable, string value)
        {
            System.Environment.SetEnvironmentVariable(variable, value);
        }
    }
}