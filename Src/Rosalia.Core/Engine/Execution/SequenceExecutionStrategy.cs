namespace Rosalia.Core.Engine.Execution
{
    using System;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class SequenceExecutionStrategy : IExecutionStrategy
    {
        public ITaskResult<IResultsStorage> Execute(Layer[] layers, ILogRenderer logRenderer)
        {
            var context = new TaskContext(this, logRenderer);

            foreach (var layer in layers)
            {
                foreach (var item in layer.Items)
                {
                    var id = item.Id;
                    var executable = item.Task;

//                    executable.MessagePosted += (sender, args) => messageListener.Invoke(
//                        new MessageEventArgs(
//                            args.Message,
//                            new Identities(id) + args.Identities));

                    var result = executable.Execute(context.CreateFor(id));
                    if (!result.IsSuccess)
                    {
                        return new FailureResult<IResultsStorage>(result.Error);
                    }

//                    context = new TaskContext(context.Results.CreateDerived(item.Id, result.Data), null);
                    context = context.CreateDerivedFor(id, result.Data);
                }
            }

            return new SuccessResult<IResultsStorage>(context.Results);
        }
    }
}