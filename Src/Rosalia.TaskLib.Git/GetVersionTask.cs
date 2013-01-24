namespace Rosalia.TaskLib.Git
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.Standard;

    /// <summary>
    /// Extracts version info from a git repository using <c>git describe --tags</c> 
    /// command.  It is required for this task that a tag was created earlier. 
    /// Use <c>git tag -a [VERSION] -m "[TAG MESSAGE]"</c> to create a tag.
    /// Use <c>git push --tags</c> to share tags
    /// (see http://www.kernel.org/pub/software/scm/git/docs/git-tag.html)
    /// Task returns [VERSION] and the number of commits from the tag. 
    /// </summary>
    public class GetVersionTask<T> : ExternalToolTask<T, GetVersionInput, GetVersionOutput>
    {
        private GetVersionOutput _result;

        public GetVersionTask()
        {
        }

        public GetVersionTask(Action<GetVersionOutput, T> applyResultToContext) : base(applyResultToContext)
        {
        }

        public GetVersionTask(Func<TaskContext<T>, GetVersionInput> contextToInput) : base(contextToInput)
        {
        }

        public GetVersionTask(Func<TaskContext<T>, GetVersionInput> contextToInput, Action<GetVersionOutput, T> applyResultToContext) : base(contextToInput, applyResultToContext)
        {
        }

        protected override GetVersionOutput CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return _result;
        }

        protected override void ProcessOnOutputDataReceived(string message, GetVersionInput builder, ResultBuilder resultBuilder, TaskContext<T> context)
        {
            base.ProcessOnOutputDataReceived(message, builder, resultBuilder, context);

            if (message.StartsWith("fatal:"))
            {
                context.Logger.Error(message);
                resultBuilder.Fail();
                return;
            }

            var part = message.Split('-');
            if (part.Length != 3)
            {
                throw new Exception(string.Format("Unexpected output format: {0}", message));
            }

            _result = new GetVersionOutput(part[0], int.Parse(part[1]), part[2]);
        }

        protected override string GetToolArguments(GetVersionInput input, TaskContext<T> context)
        {
            return "describe --tags --long";
        }

        protected override string GetToolPath(GetVersionInput input, TaskContext<T> context)
        {
            if (input != null && !string.IsNullOrEmpty(input.GitToolPath))
            {
                return input.GitToolPath;
            }

            var gitLookaupPlaces = new List<IDirectory>
            {
                context.Environment.ProgramFiles.GetDirectory(@"Git\bin"),
                context.Environment.ProgramFilesX86.GetDirectory(@"Git\bin"),
                //// todo add more common git installation directories for auto-lookup
            };

            foreach (var directory in gitLookaupPlaces)
            {
                var gitExe = directory.GetFile("git.exe");
                if (gitExe.Exists)
                {
                    return gitExe.AbsolutePath;
                }
            }

            // no git.exe found with auto lookup. Hope git is in the path variable...
            return "git";
        }
    }
}