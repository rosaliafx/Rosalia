namespace Rosalia.TaskLib.Git.Tasks
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.Git.Input;
    using Rosalia.TaskLib.Standard.Tasks;

    public abstract class AbstractGitTask<T, TInput, TOutput> : ExternalToolTask<T, TInput, TOutput> 
        where TInput : GitInput 
        where TOutput : class
    {
        protected AbstractGitTask()
        {
        }

        protected AbstractGitTask(TInput input) : base(input)
        {
        }

        protected AbstractGitTask(Func<TaskContext<T>, TInput> inputProvider) : base(inputProvider)
        {
        }

        protected AbstractGitTask(Action<TOutput, T> applyResultToContext) : base(applyResultToContext)
        {
        }

        protected AbstractGitTask(TInput input, Action<TOutput, T> applyResultToContext) : base(input, applyResultToContext)
        {
        }

        protected AbstractGitTask(Func<TaskContext<T>, TInput> inputProvider, Action<TOutput, T> applyResultToContext) : base(inputProvider, applyResultToContext)
        {
        }

        protected override string DefaultToolPath
        {
            get { return "git"; }
        }

        protected override IEnumerable<IFile> GetToolPathLookup(TaskContext<T> context)
        {
            // user can set GIT_HOME variable to point exact installed version.
            var gitHome = context.Environment.GetVariable("GIT_HOME");
            if (!string.IsNullOrEmpty(gitHome))
            {
                var gitHomeDirectory = new DefaultDirectory(gitHome);

                yield return gitHomeDirectory.GetFile("git.exe");
                yield return gitHomeDirectory.GetDirectory("bin").GetFile("git.exe");
            }

            yield return context.Environment.ProgramFilesX86.GetDirectory(@"Git\bin").GetFile("git.exe");
            yield return context.Environment.ProgramFiles.GetDirectory(@"Git\bin").GetFile("git.exe");
        }

        protected override void FillMessageLevelDetectors(IList<Func<string, MessageLevel?>> detectors)
        {
            base.FillMessageLevelDetectors(detectors);

            detectors.Add(message =>
            {
                if (message.StartsWith("fatal:"))
                {
                    return MessageLevel.Error;
                }

                return null;
            });
        }
    }
}