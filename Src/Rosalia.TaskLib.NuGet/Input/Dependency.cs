namespace Rosalia.TaskLib.NuGet.Input
{
    public class Dependency
    {
        public Dependency(string id, string version, string targetFramework)
        {
            Id = id;
            Version = version;
            TargetFramework = targetFramework;
        }

        public string Id { get; private set; }

        public string Version { get; private set; }

        public string TargetFramework { get; private set; }
    }
}