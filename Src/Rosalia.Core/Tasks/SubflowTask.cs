namespace Rosalia.Core.Tasks
{
    using Rosalia.Core.Api;
    using Rosalia.Core.Engine.Composing;
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
            RegisteredTasks definitions = _taskRegistry.GetRegisteredTasks();
            Identities tasksToExecute = _tasksToExecute.IsEmpty ? definitions.StartupTaskIds : _tasksToExecute;
            Layer[] layers = _layersComposer.Compose(definitions, tasksToExecute);

            foreach (Layer layer in layers)
            {
                ITaskResult<IdentityWithResult[]> layerResults = context.ExecutionStrategy.Execute(layer, context.CreateFor);
                if (!layerResults.IsSuccess)
                {
                    return new FailureResult<T>(layerResults.Error);
                }

                context = context.CreateDerived(layerResults.Data);
            }

            Identity mainExecutableId = definitions.ResultTaskId;
            if (mainExecutableId == null)
            {
                return new SuccessResult<T>(default(T));
            }

            return new SuccessResult<T>(context.Results.GetValueOf<T>(mainExecutableId));
        }
    }
}