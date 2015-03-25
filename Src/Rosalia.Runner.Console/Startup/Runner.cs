namespace Rosalia.Runner.Console.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks;
    using Rosalia.Runner.Console.Startup.Instantiating;
    using Rosalia.Runner.Console.Startup.Lookup;

    public class Runner
    {
        private static readonly Identity Id = "Runner";

        public InitializationResult Init(RunningOptions options)
        {
            LogHelper log = null;
            try
            {
                
                
                log = InitLogger(options);
                log.Info("Initializing...");

                IWorkflowCreator workflowCreator = new ReflectionWorkflowCreator();
                IList<IWorkflowLookup> lookups = new List<IWorkflowLookup>
                {
                    new ExactAssemblyWorkflowLookup(),
                    new WorkflowProjectLookup()
                };

                WorkflowInfo workflowInfo = GetWorkflowInfo(options, lookups, log);
                IWorkflow workflow = (IWorkflow) workflowCreator.CreateWorkflow(workflowInfo);
                
                foreach (var property in options.Properties)
                {
                    var contextProperty = workflow.GetType().GetProperty(property.Key);
                    if (contextProperty != null)
                    {
                        var convertedPropertyValue = Convert.ChangeType(property.Value, contextProperty.PropertyType);
                        contextProperty.SetValue(workflow, convertedPropertyValue, new object[0]);
                    }
                }

                log.Info("Workflow initialized");

                return new InitializationResult(workflow);
            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    log.Error(ex.Message);
                }

                return new InitializationResult();
            }
        }

        private LogHelper InitLogger(RunningOptions options)
        {
            var logRenderer = options.LogRenderer;
            if (logRenderer == null)
            {
                throw new Exception("LogRenderer is not set");
            }

            //logRenderer.Init();

            return new LogHelper(m => logRenderer.Render(m, new Identities(Id)));
        }

        protected WorkflowInfo GetWorkflowInfo(RunningOptions options, IList<IWorkflowLookup> lookups, LogHelper log)
        {
            log.Info("Searching for workflows...");

            var lookupOptions = new LookupOptions(options);
            foreach (var lookup in lookups)
            {
                if (lookup.CanHandle(lookupOptions))
                {
                    log.Info("using lookup {0} for workflow searching", lookup.GetType());
                    var foundWorkflows = lookup.Find(lookupOptions);

                    return SelectDefaultWorkflow(foundWorkflows.ToList(), options, log);
                }
            }

            throw new Exception(string.Format("No lookup to handle input file: {0}", options.InputFile));
        }

        private WorkflowInfo SelectDefaultWorkflow(IList<WorkflowInfo> foundWorkflows, RunningOptions options, LogHelper log)
        {
            log.Info("workflows found: {0}", foundWorkflows.Count);

            if (foundWorkflows.Count == 0)
            {
                throw new Exception("No workflows found");
            }

            WorkflowInfo workflowToExecute = null;
            if (foundWorkflows.Count > 1)
            {
                if (string.IsNullOrEmpty(options.Workflow))
                {
                    throw new Exception("Multiple workflows found but default workflow was not set!");
                }

                workflowToExecute = foundWorkflows.FirstOrDefault(info => 
                    info.WorkflowType.FullName.Contains(options.Workflow));
            }
            else
            {
                workflowToExecute = foundWorkflows[0];
            }

            if (workflowToExecute == null)
            {
                throw new Exception("Could not select default workflow to execute.");
            }

            log.Info("Workflow to execute: {0}", workflowToExecute.WorkflowType.Name);

            return workflowToExecute;
        }
    }
}