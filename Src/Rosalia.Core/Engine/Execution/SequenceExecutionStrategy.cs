namespace Rosalia.Core.Engine.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Interception;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class SequenceExecutionStrategy : IExecutionStrategy
    {
        public ITaskResult<IdentityWithResult[]> Execute(Layer layer, Func<Identity, TaskContext> contextFactory, ITaskInterceptor interceptor)
        {
            IList<IdentityWithResult> resultStorage = new List<IdentityWithResult>();

            foreach (var item in layer.Items)
            {
                Identity id = item.Id;
                ITask<object> executable = item.Task;
                TaskContext context = contextFactory(id);

                if (interceptor != null)
                {
                     interceptor.BeforeTaskExecute(id, executable, context);
                }
                
                ITaskResult<object> result = executable.Execute(context);
                if (interceptor != null)
                {
                    interceptor.AfterTaskExecute(id, executable, context, result);
                }

                if (!result.IsSuccess)
                {
                    return new FailureResult<IdentityWithResult[]>(result.Error);
                }

                resultStorage.Add(new IdentityWithResult(id, result.Data));
            }

            return new SuccessResult<IdentityWithResult[]>(resultStorage.ToArray());
        }
    }
}