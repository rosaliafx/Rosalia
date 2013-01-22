namespace Rosalia.TaskLib.NuGet
{
    public class File
    {
        public File(string src, string target, string exclude)
        {
            Src = src;
            Target = target;
            Exclude = exclude;
        }

        public string Src { get; private set; }

        public string Target { get; private set; }

        public string Exclude { get; private set; }
    }
}