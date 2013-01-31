namespace Rosalia.TaskLib.Git.Tasks
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Git.Input;
    using Rosalia.TaskLib.Git.Output;

    /// <summary>
    /// Extracts version info from a git repository using <c>git describe --tags</c> 
    /// command.  It is required for this task that a tag was created earlier. 
    /// Use <c>git tag -a [VERSION] -m "[TAG MESSAGE]"</c> to create a tag.
    /// Use <c>git push --tags</c> to share tags
    /// (see http://www.kernel.org/pub/software/scm/git/docs/git-tag.html)
    /// Task returns [VERSION] and the number of commits from the tag. 
    /// </summary>
    public class GetVersionTask<T> : AbstractGitTask<T, GitInput, GetVersionOutput>
    {
        private GetVersionOutput _result;

        public GetVersionTask()
        {
        }

        public GetVersionTask(Action<GetVersionOutput, T> applyResultToContext) : base(applyResultToContext)
        {
        }

        public GetVersionTask(Func<TaskContext<T>, GitInput> inputProvider)
            : base(inputProvider)
        {
        }

        public GetVersionTask(Func<TaskContext<T>, GitInput> inputProvider, Action<GetVersionOutput, T> applyResultToContext)
            : base(inputProvider, applyResultToContext)
        {
        }

        protected override GetVersionOutput CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return _result;
        }

        protected override void ProcessOnOutputDataReceived(string message, GitInput builder, ResultBuilder result, TaskContext<T> context)
        {
            base.ProcessOnOutputDataReceived(message, builder, result, context);

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

        protected override string GetToolArguments(GitInput input, TaskContext<T> context)
        {
            return "describe --tags --long";
        }
    }
}