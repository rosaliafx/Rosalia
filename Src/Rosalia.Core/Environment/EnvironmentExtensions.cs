namespace Rosalia.Core.Environment
{
    using Rosalia.FileSystem;

    public static class EnvironmentExtensions
    {
        public static IDirectory ProgramFilesX86(this IEnvironment environment)
        {
            return environment.GetDirectory("PROGRAMFILES(X86)");
        }

        public static IDirectory ProgramFiles(this IEnvironment environment)
        {
            return environment.GetDirectory("PROGRAMFILES");
        }

        public static IDirectory GetDirectory(this IEnvironment environment, string envKey)
        {
            var envValue = environment[envKey];
            return string.IsNullOrEmpty(envValue) ? null : new DefaultDirectory(envValue);
        }
    }
}