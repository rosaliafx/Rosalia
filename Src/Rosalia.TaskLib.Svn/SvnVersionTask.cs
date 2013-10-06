namespace Rosalia.TaskLib.Svn
{
    using System;
    using System.Linq;
    using System.Text;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Standard.Tasks;

    public class SvnVersionTask<T> : ExternalToolTask<T, SvnVersionResult>
    {
        private static readonly char[] AllowedTrailChars = new[] { 'M', 'S', 'P' };

        private SvnVersion _min;
        private SvnVersion _max;

        public string WorkingCopyPath { get; set; }

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

        protected override SvnVersionResult CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            if (resultBuilder.IsSuccess)
            {
                return new SvnVersionResult(_min, _max);    
            }

            return null;
        }

        protected override void ProcessOnOutputDataReceived(string message, ResultBuilder result, TaskContext<T> context)
        {
            base.ProcessOnOutputDataReceived(message, result, context);

            var versionString = message;
            if (!string.IsNullOrEmpty(versionString))
            {
                if (versionString.Equals("Unversioned directory", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.AddError("Working copy {0} is not versioned!", WorkingCopyPath);
                    result.Fail();
                    return;
                }

                var parts = versionString.Split(':');
                if (parts.Length < 1 || parts.Length > 2)
                {
                    result.AddError("Unexpected tool output: {0}", versionString);
                    result.Fail();
                    return;
                }

                try
                {
                    _min = ParseVersion(parts[0]);
                    _max = parts.Length > 1 ? ParseVersion(parts[1]) : _min;
                }
                catch (FormatException ex)
                {
                    throw new Exception(string.Format("Unexpected tool output: {0}", versionString), ex);
                }
            }
        }

        protected override string GetToolArguments(TaskContext<T> context, ResultBuilder result)
        {
            var builder = new StringBuilder();
            builder.Append(Commited ? "-c" : string.Empty);

            if (!string.IsNullOrEmpty(WorkingCopyPath))
            {
                builder.Append(" ");
                builder.Append(WorkingCopyPath);
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