namespace Rosalia.Runner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.Context.Environment;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Result;
    using Rosalia.Core.Watchers.Logging;
    using Rosalia.Core.Watchers.Visualization;
    using Rosalia.Core.Watching;
    using Rosalia.Runner.Instantiating;
    using Rosalia.Runner.Lookup;

    public class Runner : IDisposable
    {
        private ILogRenderer _logRenderer;
        private IWorkflow _workflow;
        private object _context;

        public InitializationResult Init(RunningOptions options)
        {
            try
            {
                InitLogger(options);

                _logRenderer.AppendMessage(0, "Initializing...", MessageLevel.Info);

                options.InputFile = SanitizeInputFilePath(options);

                IWorkflowCreator workflowCreator = new ReflectionWorkflowCreator();
                IContextCreator contextCreator = new ReflectionContextCreator();
                IList<IWorkflowWatcher> watchers = new List<IWorkflowWatcher>
                {
                    new LoggingWatcher(_logRenderer),
                    new LogTaskStartWatcher(),
                };

                foreach (var outputPath in options.VisualisationFilesPath)
                {
                    var extension = Path.GetExtension(outputPath);
                    if (extension.Equals(".svg", StringComparison.InvariantCultureIgnoreCase))
                    {
                        watchers.Add(new SvgVisualizeWatcher(outputPath));
                    }

                    if (extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        watchers.Add(new GraphicsVisualizeWatcher(outputPath));
                    }
                }

                IList<IWorkflowLookup> lookups = new List<IWorkflowLookup>
                {
                    new ExactAssemblyWorkflowLookup(),
                    new WorkflowProjectLookup()
                };

                var workflowInfo = GetWorkflowInfo(options, lookups);

                _workflow = (IWorkflow)workflowCreator.CreateWorkflow(workflowInfo);
                
                _workflow.Init(new WorkflowContext(
                    new DefaultEnvironment(), 
                    new DefaultDirectory(options.WorkDirectory)));

                _context = contextCreator.CreateContext(workflowInfo);

                foreach (var watcher in watchers)
                {
                    watcher.Register(_workflow);
                }

                _logRenderer.AppendMessage(0, "Workflow initialized", MessageLevel.Success);

                return new InitializationResult(_workflow, _context);
            }
            catch (Exception ex)
            {
                if (_logRenderer != null)
                {
                    _logRenderer.AppendMessage(0, ex.Message, MessageLevel.Error);
                }

                return new InitializationResult();
            }
        }

        private string SanitizeInputFilePath(RunningOptions options)
        {
            var inputFile = options.InputFile;

            if (string.IsNullOrEmpty(inputFile))
            {
                throw new Exception("Input file path is not set");
            }

            if (!Path.IsPathRooted(inputFile))
            {
                inputFile = Path.Combine(options.WorkDirectory, options.InputFile);
            }

            if (!File.Exists(inputFile))
            {
                throw new Exception(string.Format("Input file {0} does not exist!", inputFile));
            }

            return inputFile;
        }

        private void InitLogger(RunningOptions options)
        {
            _logRenderer = options.LogRenderer;
            if (_logRenderer == null)
            {
                throw new Exception("LogRenderer is not set");
            }

            _logRenderer.Init();
        }

        protected WorkflowInfo GetWorkflowInfo(RunningOptions options, IList<IWorkflowLookup> lookups)
        {
            _logRenderer.AppendMessage(1, "Searching for workflows...", MessageLevel.Info);

            var lookupOptions = new LookupOptions(options);
            foreach (var lookup in lookups)
            {
                if (lookup.CanHandle(lookupOptions))
                {
                    var message = string.Format("using lookup {0} for workflow searching", lookup.GetType());
                    _logRenderer.AppendMessage(1, message, MessageLevel.Info);
                    var foundWorkflows = lookup.Find(lookupOptions);
                    return SelectDefaultWorkflow(foundWorkflows.ToList(), options);
                }
            }

            throw new Exception(string.Format("No lookup to handle input file: {0}", options.InputFile));
        }

        private WorkflowInfo SelectDefaultWorkflow(IList<WorkflowInfo> foundWorkflows, RunningOptions options)
        {
            _logRenderer.AppendMessage(1, string.Format("workflows found: {0}", foundWorkflows.Count), MessageLevel.Info);

            if (foundWorkflows.Count == 0)
            {
                throw new Exception("No workflows found");
            }

            WorkflowInfo workflowToExecute = null;
            if (foundWorkflows.Count > 1)
            {
                if (string.IsNullOrEmpty(options.DefaultWorkflowType))
                {
                    throw new Exception("Multiple workflows found but default workflow was not set!");
                }

                workflowToExecute = foundWorkflows.FirstOrDefault(info =>
                    info.WorkflowType.FullName == options.DefaultWorkflowType ||
                    info.WorkflowType.Name == options.DefaultWorkflowType);
            }
            else
            {
                workflowToExecute = foundWorkflows[0];
            }

            if (workflowToExecute == null)
            {
                throw new Exception("Could not select default workflow to execute.");
            }

            _logRenderer.AppendMessage(1, string.Format("Workflow to execute: {0}", workflowToExecute.WorkflowType.Name), MessageLevel.Info);

            return workflowToExecute;
        }

        public ExecutionResult ExecuteDefaultWorkflow()
        {
            return _workflow.Execute(_context);
        }

        public void Dispose()
        {
            if (_logRenderer != null)
            {
                _logRenderer.Dispose();
            }
        }
    }
}