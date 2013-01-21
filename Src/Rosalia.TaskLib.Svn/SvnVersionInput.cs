namespace Rosalia.TaskLib.Svn
{
    public class SvnVersionInput
    {
        public SvnVersionInput(string svnExePath, string workingCopyPath) : this(svnExePath, workingCopyPath, null)
        {
        }

        public SvnVersionInput(string svnExePath, string workingCopyPath, string trailUrl)
        {
            SvnExePath = svnExePath;
            WorkingCopyPath = workingCopyPath;
            TrailUrl = trailUrl;
        }

        public SvnVersionInput(string svnExePath)
        {
            SvnExePath = svnExePath;
        }

        public string SvnExePath { get; private set; }

        public string WorkingCopyPath { get; private set; }

        public string TrailUrl { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether a tool should return 
        /// last-changed revisions rather than the current 
        /// (see svnversion.exe help for details).
        /// </summary>
        public bool Commited { get; set; }
    }
}