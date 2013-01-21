namespace Rosalia.TaskLib.Svn
{
    using System;
    using System.Linq;
    using System.Text;
    using Rosalia.Core;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.Standard;

    public class SvnVersionTask<T> : ExternalToolTask<T, SvnVersionInput, SvnVersionResult>
    {
        private static readonly char[] AllowedTrailChars = new[] { 'M', 'S', 'P' };

        private SvnVersion _min;
        private SvnVersion _max;

        public SvnVersionTask(Func<ExecutionContext<T>, SvnVersionInput> contextToInput)
            : base(contextToInput)
        {
        }

        protected override SvnVersionResult CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return new SvnVersionResult(_min, _max);
        }

        protected override void ProcessOnOutputDataReceived(string message, SvnVersionInput input, ResultBuilder resultBuilder, ExecutionContext<T> context)
        {
            base.ProcessOnOutputDataReceived(message, input, resultBuilder, context);

            var versionString = message;
            if (!string.IsNullOrEmpty(versionString))
            {
                if (versionString.Equals("Unversioned directory", StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Logger.Error("Working copy {0} is not versioned!", input.WorkingCopyPath);
                    resultBuilder.Fail();
                    return;
                }

                var parts = versionString.Split(':');
                if (parts.Length < 1 || parts.Length > 2)
                {
                    context.Logger.Error("Unexpected tool output: {0}", versionString);
                    resultBuilder.Fail();
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

        protected override string GetToolPath(SvnVersionInput input, ExecutionContext<T> context)
        {
            return input.SvnExePath;
        }

        protected override string GetToolArguments(SvnVersionInput input, ExecutionContext<T> context)
        {
            var builder = new StringBuilder();
            builder.Append(input.Commited ? "-c" : string.Empty);

            if (!string.IsNullOrEmpty(input.WorkingCopyPath))
            {
                builder.Append(" ");
                builder.Append(input.WorkingCopyPath);
                builder.Append(" ");
                builder.Append(input.TrailUrl);
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