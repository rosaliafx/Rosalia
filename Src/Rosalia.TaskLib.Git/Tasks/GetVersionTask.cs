namespace Rosalia.TaskLib.Git.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.TaskLib.Git.Results;

    /// <summary>
    /// Extracts version info from a git repository using <c>git describe --tags</c> 
    /// command.  It is required for this task that a tag was created earlier. 
    /// Use <c>git tag -a [VERSION] -m "[TAG MESSAGE]"</c> to create a tag.
    /// Use <c>git push --tags</c> to share tags
    /// (see http://www.kernel.org/pub/software/scm/git/docs/git-tag.html)
    /// Task returns [VERSION] and the number of commits from the tag. 
    /// </summary>
    public class GetVersionTask : AbstractGitTask<GetVersionOutput>
    {
        protected override GetVersionOutput CreateResult(int exitCode, TaskContext context, IList<Message> aggregatedOutput)
        {
            var targetLine = aggregatedOutput.FirstOrDefault(message => message.Level == MessageLevel.Info);
            if (targetLine == null)
            {
                throw new Exception("Could not parse output");
            }

            var part = targetLine.Text.Split('-');
            if (part.Length != 3)
            {
                throw new Exception(string.Format("Unexpected output format: {0}", targetLine.Text));
            }

            return new GetVersionOutput(part[0], int.Parse(part[1]), part[2]);
        }

        protected override bool AggregateForResultConstruction(string message, MessageLevel level)
        {
            return level == MessageLevel.Info;
        }

        protected override string GetToolArguments(TaskContext context)
        {
            return "describe --tags --long";
        }
    }
}