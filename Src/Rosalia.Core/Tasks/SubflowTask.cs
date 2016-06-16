namespace Rosalia.Core.Tasks
{
    using Rosalia.Core.Api;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Interception;
    using Rosalia.Core.Tasks.Results;

    public class SubflowTask<T> : AbstractTask<T> where T : class
    {
        private readonly ITaskRegistry<T> _taskRegistry;
        private readonly ILayersComposer _layersComposer = new SimpleLayersComposer();
        private readonly Identities _tasksToExecute;

        public SubflowTask(ITaskRegistry<T> taskRegistry, Identities tasksToExecute)
        {
            _taskRegistry = taskRegistry;
            _tasksToExecute = tasksToExecute;
        }

        protected override ITaskResult<T> SafeExecute(TaskContext context)
        {
            _taskRegistry.Environment = context.Environment;
            _taskRegistry.WorkDirectory = context.WorkDirectory;

            ResultWithContext resultData = ExecuteAllTasks(context);

            ITaskRegistryInterceptor<T> interceptor = _taskRegistry as ITaskRegistryInterceptor<T>;
            if (interceptor != null)
            {
                ITaskResult<T> interceptedResult = interceptor.AfterExecute(resultData.Context, resultData.Result);
                if (interceptedResult != null)
                {
                    return interceptedResult;
                }
            }

            return resultData.Result;
        }

        private ResultWithContext ExecuteAllTasks(TaskContext context)
        {
            RegisteredTasks definitions = _taskRegistry.GetRegisteredTasks();
            Identities tasksToExecute = _tasksToExecute.IsEmpty ? definitions.StartupTaskIds : _tasksToExecute;
            Layer[] layers = _layersComposer.Compose(definitions, tasksToExecute);

            foreach (Layer layer in layers)
            {
                ITaskResult<IdentityWithResult[]> layerResults = context.ExecutionStrategy.Execute(layer, context.CreateFor, context.Interceptor);
                if (!layerResults.IsSuccess)
                {
                    return new ResultWithContext(new FailureResult<T>(layerResults.Error), context);
                }

                context = context.CreateDerived(layerResults.Data);
            }

            Identity mainExecutableId = definitions.ResultTaskId;
            if (mainExecutableId == null)
            {
                return new ResultWithContext(new SuccessResult<T>(default(T)), context);
            }

            return new ResultWithContext(new SuccessResult<T>(context.Results.GetValueOf<T>(mainExecutableId)), context);
        }

        internal class ResultWithContext
        {
            private readonly ITaskResult<T> _result;
            private readonly TaskContext _context;

            public ResultWithContext(ITaskResult<T> result, TaskContext context)
            {
                _result = result;
                _context = context;
            }

            public ITaskResult<T> Result
            {
                get { return _result; }
            }

            public TaskContext Context
            {
                get { return _context; }
            }
        }
    }
}