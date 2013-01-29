namespace Rosalia.TaskLib.Git.Input
{
    /// <summary>
    /// A base class for git tasks.
    /// </summary>
    public class GitInput
    {
        public GitInput()
        {
        }

        public GitInput(string gitToolPath)
        {
            GitToolPath = gitToolPath;
        }

        public string GitToolPath { get; set; }
    }
}