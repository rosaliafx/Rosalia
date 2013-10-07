namespace Rosalia.TaskLib.Git.Tasks
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Git.Output;

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
        private GetVersionOutput _result;

        protected override GetVersionOutput CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return _result;
        }

        protected override void ProcessOnOutputDataReceived(string message, ResultBuilder result, TaskContext context)
        {
            base.ProcessOnOutputDataReceived(message, result, context);

            if (!result.HasErrors)
            {
                var part = message.Split('-');
                if (part.Length != 3)
                {
                    throw new Exception(string.Format("Unexpected output format: {0}", message));
                }

                _result = new GetVersionOutput(part[0], int.Parse(part[1]), part[2]);
            }
        }

        protected override string GetToolArguments(TaskContext context, ResultBuilder result)
        {
            return "describe --tags --long";
        }
    }
}