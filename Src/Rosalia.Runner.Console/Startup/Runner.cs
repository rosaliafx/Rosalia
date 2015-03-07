namespace Rosalia.Runner.Console.Startup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Rosalia.Core;
    using Rosalia.Core.Api;
    using Rosalia.Core.Logging;
    using Rosalia.FileSystem;
    using Rosalia.Runner.Console.Startup.Instantiating;
    using Rosalia.Runner.Console.Startup.Lookup;

    public class Runner
    {
        public InitializationResult Init(RunningOptions options)
        {
            try
            {
                if (options.InputFile == null)
                {
                    throw new Exception("Input file path is not set");
                }

                if (!options.InputFile.Exists)
                {
                    throw new Exception(string.Format("Input file {0} does not exist!", options.InputFile.AbsolutePath));
                }
                InitLogger(options);

                // todo log runner messages
                //_logRenderer.AppendMessage(0, "Initializing...", MessageLevel.Info, MessageType.System);

                //options.InputFile = new DefaultFile(SanitizeInputFilePath(options));

                IWorkflowCreator workflowCreator = new ReflectionWorkflowCreator();
//                IContextCreator contextCreator = new ReflectionContextCreator();
//                IList<IWorkflowMonitor> watchers = new List<IWorkflowMonitor>
//                {
//                    new LoggingMonitor(_logRenderer),
//                };
//
//                foreach (var outputPath in options.VisualisationFilesPath)
//                {
//                    var extension = Path.GetExtension(outputPath);
//                    if (extension.Equals(".svg", StringComparison.InvariantCultureIgnoreCase))
//                    {
//                        watchers.Add(new SvgVisualizeMonitor(outputPath));
//                    }
//
//                    if (extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
//                    {
//                        watchers.Add(new GraphicsVisualizeMonitor(outputPath));
//                    }
//                }

                IList<IWorkflowLookup> lookups = new List<IWorkflowLookup>
                {
                    new ExactAssemblyWorkflowLookup(),
                    new WorkflowProjectLookup()
                };

                var workflowInfo = GetWorkflowInfo(options, lookups);

                var _workflow = (IWorkflow)workflowCreator.CreateWorkflow(workflowInfo);
                
                foreach (var property in options.Properties)
                {
                    var contextProperty = _workflow.GetType().GetProperty(property.Key);
                    if (contextProperty != null)
                    {
                        var convertedPropertyValue = Convert.ChangeType(property.Value, contextProperty.PropertyType);
                        contextProperty.SetValue(_workflow, convertedPropertyValue, new object[0]);
                    }
                }

//                foreach (var watcher in watchers)
//                {
//                    watcher.Register(_workflow);
//                }

                // todo log
                //_logRenderer.AppendMessage(0, "Workflow initialized", MessageLevel.Success, MessageType.System);

                return new InitializationResult(_workflow);
            }
            catch (Exception ex)
            {
                if (options.LogRenderer != null)
                {
                    // todo log
                    //options.LogRenderer.Render( ex.Message, MessageLevel.Error);
                }

                return new InitializationResult();
            }
        }

        
        private void InitLogger(RunningOptions options)
        {
            var logRenderer = options.LogRenderer;
            if (logRenderer == null)
            {
                throw new Exception("LogRenderer is not set");
            }

            logRenderer.Init();
        }

        protected WorkflowInfo GetWorkflowInfo(RunningOptions options, IList<IWorkflowLookup> lookups)
        {
            var _logRenderer = options.LogRenderer;
            _logRenderer.Render(new Message("Searching for workflows...", MessageLevel.Info), new Identity("Runner"));

            var lookupOptions = new LookupOptions(options);
            foreach (var lookup in lookups)
            {
                if (lookup.CanHandle(lookupOptions))
                {
                    var message = string.Format("using lookup {0} for workflow searching", lookup.GetType());
                    _logRenderer.Render(new Message(message, MessageLevel.Info), new Identity("Runner"));
                    var foundWorkflows = lookup.Find(lookupOptions);
                    return SelectDefaultWorkflow(foundWorkflows.ToList(), options);
                }
            }

            throw new Exception(string.Format("No lookup to handle input file: {0}", options.InputFile));
        }

        private WorkflowInfo SelectDefaultWorkflow(IList<WorkflowInfo> foundWorkflows, RunningOptions options)
        {
            var _logRenderer = options.LogRenderer;
            _logRenderer.Render(new Message(string.Format("workflows found: {0}", foundWorkflows.Count), MessageLevel.Info), new Identity("Runner"));

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

            _logRenderer.Render(new Message(string.Format("Workflow to execute: {0}", workflowToExecute.WorkflowType.Name), MessageLevel.Info), new Identity("Runner"));
//            _logRenderer.AppendMessage(
//                1, 
//                string.Format("Workflow to execute: {0}", workflowToExecute.WorkflowType.Name), 
//                MessageLevel.Info,
//                MessageType.System);

            return workflowToExecute;
        }

//        public void Dispose()
//        {
//            if (_logRenderer != null)
//            {
//                _logRenderer.Dispose();
//            }
//        }
    }
}