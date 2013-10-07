namespace Rosalia.TaskLib.Standard.Tasks
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;

    public abstract class ExternalToolTask<TResult> : TaskWithResult<TResult> where TResult : class
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

        protected override TResult Execute(TaskContext context, ResultBuilder result)
        {
            FillMessageLevelDetectors(_messageLevelDetectors);

            var toolPath = GetToolPath(context, result);
            var toolArguments = GetToolArguments(context, result);

            result.AddInfo(
                "Start external tool with command line: {0}{1} {2}",
                Environment.NewLine,
                toolPath,
                toolArguments);

            var exitCode = ProcessStarter.StartProcess(
                toolPath,
                toolArguments,
                GetToolWorkDirectory(context).AbsolutePath,
                outpuMessage =>
                {
                    try
                    {
                        ProcessOnOutputDataReceived(outpuMessage, result, context);
                    }
                    catch (Exception ex)
                    {
                        HandleExecutionException(ex, result);
                    }
                },
                errorMessage =>
                {
                    try
                    {
                        ProcessOnErrorDataReceived(errorMessage, result, context);
                    }
                    catch (Exception ex)
                    {
                        HandleExecutionException(ex, result);
                    }
                });

            return ProcessExitCode(exitCode, result);
        }

        protected virtual IEnumerable<IFile> GetToolPathLookup(TaskContext context, ResultBuilder result)
        {
            yield break;
        }

        protected virtual void FillMessageLevelDetectors(IList<Func<string, MessageLevel?>> detectors)
        {
        }

        protected virtual string GetToolArguments(TaskContext context, ResultBuilder result)
        {
            return Arguments ?? string.Empty;
        }

        protected virtual string GetToolPath(TaskContext context, ResultBuilder result)
        {
            if (!string.IsNullOrEmpty(ToolPath))
            {
                return ToolPath;
            }

            foreach (var toolFile in GetToolPathLookup(context, result))
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

        protected virtual TResult ProcessExitCode(int exitCode, ResultBuilder resultBuilder)
        {
            if (exitCode != 0)
            {
                resultBuilder.FailWithError("Pocess exit with code {0}", exitCode);
                return null;
            }

            return CreateResult(exitCode, resultBuilder);
        }

        protected abstract TResult CreateResult(int exitCode, ResultBuilder resultBuilder);

        protected virtual void ProcessOnOutputDataReceived(string message, ResultBuilder result, TaskContext context)
        {
            foreach (var messageLevelDetector in _messageLevelDetectors)
            {
                var level = messageLevelDetector(message);

                if (level.HasValue)
                {
                    result.AddMessage(level.Value, message);
                    return;
                }
            }

            result.AddInfo(message);
        }

        protected virtual void ProcessOnErrorDataReceived(string message, ResultBuilder resultBuilder, TaskContext context)
        {
            resultBuilder.AddError(message);
        }

        protected virtual IDirectory GetToolWorkDirectory(TaskContext context)
        {
            return WorkDirectory ?? context.WorkDirectory;
        }
    }
}