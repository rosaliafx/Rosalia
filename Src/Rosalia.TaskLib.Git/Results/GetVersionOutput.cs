namespace Rosalia.TaskLib.Git.Results
{
    public class GetVersionOutput
    {
        public GetVersionOutput(string tag, int commitsCount, string lastCommitKey)
        {
            Tag = tag;
            CommitsCount = commitsCount;
            LastCommitKey = lastCommitKey;
        }

        public string Tag { get; private set; }

        public int CommitsCount { get; private set; }

        public string LastCommitKey { get; private set; }
    }
}