using SystemTask = System.Threading.Tasks.Task;

namespace Rosalia.Core.Engine.Execution
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Interception;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class ParallelExecutionStrategy : IExecutionStrategy
    {
        public ITaskResult<IdentityWithResult[]> Execute(Layer layer, Func<Identity, TaskContext> contextFactory, ITaskInterceptor interceptor)
        {
            ITaskResult<IdentityWithResult[]> failure = null;

            var results = new ConcurrentBag<IdentityWithResult>();
            var layerTasks = layer.Items.Select(item => SystemTask.Factory.StartNew(() =>
            {
                Identity id = item.Id;
                TaskContext taskContext = contextFactory(id);

                if (interceptor != null)
                {
                    interceptor.BeforeTaskExecute(id, item.Task, taskContext);
                }
                
                ITaskResult<object> result = item.Task.Execute(taskContext);
                if (interceptor != null)
                {
                    interceptor.AfterTaskExecute(id, item.Task, taskContext, result);
                }

                if (result.IsSuccess)
                {
                    results.Add(new IdentityWithResult(id, result.Data));
                }
                else
                {
                    failure = new FailureResult<IdentityWithResult[]>(result.Error);
                }
            })).ToArray();

            SystemTask.WaitAll(layerTasks);

            if (failure != null)
            {
                return failure;
            }

            return new SuccessResult<IdentityWithResult[]>(results.ToArray());
        }
    }
}