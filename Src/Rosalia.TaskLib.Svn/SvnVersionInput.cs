namespace Rosalia.TaskLib.Svn
{
    using Rosalia.TaskLib.Standard.Input;

    public class SvnVersionInput : ExternalToolInput
    {
        public SvnVersionInput()
        {
        }

        public SvnVersionInput(string workingCopyPath) : this(workingCopyPath, null)
        {
        }

        public SvnVersionInput(string workingCopyPath, string trailUrl)
        {
            WorkingCopyPath = workingCopyPath;
            TrailUrl = trailUrl;
        }

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