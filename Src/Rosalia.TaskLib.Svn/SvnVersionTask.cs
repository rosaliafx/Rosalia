namespace Rosalia.TaskLib.Svn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.FileSystem;
    using Rosalia.TaskLib.Standard.Tasks;

    public class SvnVersionTask : ExternalToolTask<SvnVersionResult>
    {
        private static readonly char[] AllowedTrailChars = new[] { 'M', 'S', 'P' };

        public IDirectory WorkingCopyPath { get; set; }

        public string TrailUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a tool should return 
        /// last-changed revisions rather than the current 
        /// (see svnversion.exe help for details).
        /// </summary>
        public bool Commited { get; set; }

        protected override string DefaultToolPath
        {
            get { return "svnversion"; }
        }

        protected override SvnVersionResult CreateResult(int exitCode, TaskContext context, IList<Message> aggregatedOutput)
        {
            var versionStringMessage = aggregatedOutput.FirstOrDefault(message => message.Level == MessageLevel.Info);
            if (versionStringMessage == null)
            {
                throw new Exception("Unexpected output");
            }

            var versionString = versionStringMessage.Text;
            if (string.IsNullOrEmpty(versionString))
            {
                throw new Exception("Unexpected output");
            }

            if (versionString.Equals("Unversioned directory", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception(string.Format("Working copy {0} is not versioned!", WorkingCopyPath));
            }

            var parts = versionString.Split(':');
            if (parts.Length < 1 || parts.Length > 2)
            {
                throw new Exception(string.Format("Unexpected tool output: {0}", versionString));
            }

            try
            {
                var min = ParseVersion(parts[0]);
                var max = parts.Length > 1 ? ParseVersion(parts[1]) : min;

                return new SvnVersionResult(min, max);
            }
            catch (FormatException ex)
            {
                throw new Exception(string.Format("Unexpected tool output: {0}", versionString), ex);
            }
        }

        protected override bool AggregateForResultConstruction(string message, MessageLevel level)
        {
            return true;
        }

        protected override string GetToolArguments(TaskContext context)
        {
            var builder = new StringBuilder();
            builder.Append(Commited ? "-c" : string.Empty);

            if (WorkingCopyPath != null)
            {
                builder.Append(" ");
                builder.Append(WorkingCopyPath.AbsolutePath);
                builder.Append(" ");
                builder.Append(TrailUrl);
            }

            return builder.ToString();
        }

        private SvnVersion ParseVersion(string version)
        {
            var trailBuilder = new StringBuilder();
            var trailRead = false;
            while ((!trailRead) && version.Length > 0)
            {
                var lastVersionChar = version[version.Length - 1];
                if (AllowedTrailChars.Any(trailChar => trailChar == lastVersionChar))
                {
                    version = version.Substring(0, version.Length - 1);
                    trailBuilder.Insert(0, lastVersionChar);
                }
                else
                {
                    trailRead = true;
                }
            }

            return new SvnVersion(int.Parse(version), trailBuilder.ToString());
        }
    }
}