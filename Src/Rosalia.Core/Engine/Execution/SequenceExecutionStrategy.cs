namespace Rosalia.Core.Engine.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class SequenceExecutionStrategy : IExecutionStrategy
    {
//        public ITaskResult<IResultsStorage> Execute(Layer[] layers, TaskContext initialContext)
//        {
//            var context = initialContext;
//
//            foreach (var layer in layers)
//            {
//                foreach (var item in layer.Items)
//                {
//                    var id = item.Id;
//                    var executable = item.Task;
//                    var result = executable.Execute(context.CreateFor(id));
//
//                    if (!result.IsSuccess)
//                    {
//                        return new FailureResult<IResultsStorage>(result.Error);
//                    }
//
//                    context = context.CreateDerivedFor(id, result.Data);
//                }
//            }
//
//            return new SuccessResult<IResultsStorage>(context.Results);
//        }

        public ITaskResult<IdentityWithResult[]> Execute(Layer layer, Func<Identity, TaskContext> contextFactory)
        {
            IList<IdentityWithResult> resultStorage = new List<IdentityWithResult>();

            foreach (var item in layer.Items)
            {
                var id = item.Id;
                var executable = item.Task;
                var result = executable.Execute(contextFactory(id));

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