namespace Rosalia.TaskLib.Git.Tasks
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Environment;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.FileSystem;
    using Rosalia.TaskLib.Standard.Tasks;

    public abstract class AbstractGitTask<TOutput> : ExternalToolTask<TOutput> 
        where TOutput : class
    {
        protected override string DefaultToolPath
        {
            get { return "git"; }
        }

        public string RawCommand { get; set; }

        public override string Arguments
        {
            get
            {
                return RawCommand;
            }

            set
            {
                RawCommand = value;
            }
        }

        protected override IEnumerable<IFile> GetToolPathLookup(TaskContext context)
        {
            // user can set GIT_HOME variable to point exact installed version.
            var gitHome = context.Environment["GIT_HOME"];
            if (!string.IsNullOrEmpty(gitHome))
            {
                var gitHomeDirectory = new DefaultDirectory(gitHome);

                yield return gitHomeDirectory.GetFile("git.exe");
                yield return gitHomeDirectory.GetDirectory("bin").GetFile("git.exe");
            }

            yield return context.Environment.ProgramFilesX86()["Git"]["bin"]["git.exe"];
            yield return context.Environment.ProgramFiles()["Git"]["bin"]["git.exe"];
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