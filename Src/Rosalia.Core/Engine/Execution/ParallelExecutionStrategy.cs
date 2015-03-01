using SystemTask = System.Threading.Tasks.Task;

namespace Rosalia.Core.Engine.Execution
{
    using System.Collections.Concurrent;
    using System.Linq;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class ParallelExecutionStrategy : IExecutionStrategy
    {
        public ITaskResult<IResultsStorage> Execute(Layer[] layers, ILogRenderer logRenderer)
        {
            var context = new TaskContext(this, logRenderer);
            FailureResult<IResultsStorage> failure = null;

            foreach (var layer in layers)
            {
                var results = new ConcurrentDictionary<Identity, object>();
                var currentContext = context;
                var layerTasks = layer.Items.Select(item => SystemTask.Factory.StartNew(() =>
                {
                    Identity id = item.Id;
                    ITaskResult<object> result = item.Task.Execute(currentContext.CreateFor(id));

                    if (result.IsSuccess)
                    {
                        results[id] = result.Data;
                    }
                    else
                    {
                        failure = new FailureResult<IResultsStorage>(result.Error);
                    }
                })).ToArray();

                SystemTask.WaitAll(layerTasks);

                if (failure != null)
                {
                    return failure;
                }

                context = results.Aggregate(
                    currentContext, 
                    (current, result) => current.CreateDerivedFor(result.Key, result.Value) /*new TaskContext(current.Results.CreateDerived(result.Key, result.Value), null)*/);
            }

            return new SuccessResult<IResultsStorage>(context.Results);
        }
    }
}