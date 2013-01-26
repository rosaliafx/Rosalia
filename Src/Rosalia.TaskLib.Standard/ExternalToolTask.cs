namespace Rosalia.TaskLib.Standard
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;

    public abstract class ExternalToolTask<TContext, TInput, TResult> :
        ExtendedTask<TContext, TInput, TResult>
        where TInput : class
        where TResult : class
    {
        private readonly IList<Func<string, MessageLevel?>> _messageLevelDetectors = new List<Func<string, MessageLevel?>>();

        private IProcessStarter _processStarter = new DefaultProcessStarter();

        protected ExternalToolTask()
        {
        }

        protected ExternalToolTask(TInput input) : base(input)
        {
        }

        protected ExternalToolTask(Func<TaskContext<TContext>, TInput> contextToInput) : base(contextToInput)
        {
        }

        protected ExternalToolTask(Action<TResult, TContext> applyResultToContext) : base(applyResultToContext)
        {
        }

        protected ExternalToolTask(TInput input, Action<TResult, TContext> applyResultToContext) : base(input, applyResultToContext)
        {
        }

        protected ExternalToolTask(Func<TaskContext<TContext>, TInput> contextToInput, Action<TResult, TContext> applyResultToContext) : base(contextToInput, applyResultToContext)
        {
        }

        public IProcessStarter ProcessStarter
        {
            get { return _processStarter; }
            set { _processStarter = value; }
        }

        protected override TResult Execute(TInput input, TaskContext<TContext> context, ResultBuilder result)
        {
            FillMessageLevelDetectors(_messageLevelDetectors);

            var toolPath = GetToolPath(input, context);
            var toolArguments = GetToolArguments(input, context);

            result.AddInfo(
                "Start external tool with command line: {0}{1}{2}",
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
                        ProcessOnOutputDataReceived(outpuMessage, input, result, context);
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
                        ProcessOnErrorDataReceived(errorMessage, input, result, context);
                    }
                    catch (Exception ex)
                    {
                        HandleExecutionException(ex, result);
                    }
                });

            return ProcessExitCode(exitCode, result);
        }

        protected virtual void FillMessageLevelDetectors(IList<Func<string, MessageLevel?>> detectors)
        {
        }

        protected virtual string GetToolArguments(TInput input, TaskContext<TContext> context)
        {
            var externalToolAware = input as IExternalToolAware;
            if (externalToolAware != null)
            {
                return externalToolAware.Arguments;
            }

            return string.Empty;
        }

        protected virtual string GetToolPath(TInput input, TaskContext<TContext> context)
        {
            var externalToolAware = input as IExternalToolAware;
            if (externalToolAware != null)
            {
                return externalToolAware.Path;
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

        protected virtual void ProcessOnOutputDataReceived(string message, TInput builder, ResultBuilder result, TaskContext<TContext> context)
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

        protected virtual void ProcessOnErrorDataReceived(string message, TInput builder, ResultBuilder resultBuilder, TaskContext<TContext> context)
        {
            resultBuilder.AddError(message);
        }

        protected virtual IDirectory GetToolWorkDirectory(TaskContext<TContext> context)
        {
            return context.WorkDirectory;
        }
    }
}