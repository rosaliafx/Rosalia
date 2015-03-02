namespace Rosalia.TaskLib.Standard.Tasks
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;

    public abstract class ExternalToolTask<TResult> : AbstractTask<TResult> where TResult : class
    {
        private readonly IList<Func<string, MessageLevel?>> _messageLevelDetectors = new List<Func<string, MessageLevel?>>();

        private IProcessStarter _processStarter = new DefaultProcessStarter();

        public virtual IDirectory WorkDirectory { get; set; }

        public virtual string ToolPath { get; set; }

        public virtual string Arguments { get; set; }

        public IProcessStarter ProcessStarter
        {
            get { return _processStarter; }
            set { _processStarter = value; }
        }

        /// <summary>
        /// Gets default tool path or null if default path is not supported.
        /// </summary>
        protected virtual string DefaultToolPath
        {
            get { return null; }
        }

        protected override ITaskResult<TResult> SafeExecute(TaskContext context)
        {
            FillMessageLevelDetectors(_messageLevelDetectors);

            var toolPath = GetToolPath(context);
            var toolArguments = GetToolArguments(context, context);

            context.Log.Info(
                "Start external tool with command line: {0}{1} {2}",
                Environment.NewLine,
                toolPath,
                toolArguments);

            var workDirectory = GetToolWorkDirectory(context);
            var exitCode = ProcessStarter.StartProcess(
                toolPath,
                toolArguments,
                workDirectory == null ? null : workDirectory.AbsolutePath,
                outpuMessage =>
                {
                    ProcessOnOutputDataReceived(outpuMessage, context);
                },
                errorMessage =>
                {
                    ProcessOnErrorDataReceived(errorMessage, context);
                });

            return ProcessExitCode(exitCode, context);
        }

        protected virtual IEnumerable<IFile> GetToolPathLookup(TaskContext context)
        {
            yield break;
        }

        protected virtual void FillMessageLevelDetectors(IList<Func<string, MessageLevel?>> detectors)
        {
        }

        protected virtual string GetToolArguments(TaskContext context, TaskContext result)
        {
            return Arguments ?? string.Empty;
        }

        protected virtual string GetToolPath(TaskContext context)
        {
            if (!string.IsNullOrEmpty(ToolPath))
            {
                return ToolPath;
            }

            foreach (var toolFile in GetToolPathLookup(context))
            {
                if (toolFile.Exists)
                {
                    return toolFile.AbsolutePath;
                }
            }

            if (!string.IsNullOrEmpty(DefaultToolPath))
            {
                return DefaultToolPath;
            }

            throw new Exception("Tool path is not set!");
        }

        protected virtual ITaskResult<TResult> ProcessExitCode(int exitCode, TaskContext context)
        {
            if (exitCode != 0)
            {
                context.Log.Error("Pocess exit with code {0}", exitCode);
                return new FailureResult<TResult>(null);
            }

            return new SuccessResult<TResult>(CreateResult(exitCode, context));
        }

        protected abstract TResult CreateResult(int exitCode, TaskContext context);

        protected virtual void ProcessOnOutputDataReceived(string message, TaskContext context)
        {
            foreach (var messageLevelDetector in _messageLevelDetectors)
            {
                var level = messageLevelDetector(message);

                if (level.HasValue)
                {
                    context.Log.AddMessage(level.Value, message);
                    return;
                }
            }

            context.Log.Info(message);
        }

        protected virtual void ProcessOnErrorDataReceived(string message, TaskContext context)
        {
            context.Log.Error(message);
        }

        protected virtual IDirectory GetToolWorkDirectory(TaskContext context)
        {
            return WorkDirectory ?? context.WorkDirectory;
        }
    }
}