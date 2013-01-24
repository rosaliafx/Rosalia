namespace Rosalia.Core.Context.Environment
{
    using Rosalia.Core.FileSystem;

    public class DefaultEnvironment : IEnvironment
    {
        public IDirectory ProgramFilesX86
        {
            get
            {
                return new DefaultDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86));
            }
        }

        public IDirectory ProgramFiles
        {
            get
            {
                return new DefaultDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles));
            }
        }

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