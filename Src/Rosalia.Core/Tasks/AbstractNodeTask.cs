namespace Rosalia.Core.Tasks
{
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public abstract class AbstractNodeTask<T> : AbstractTask<T>
    {
        public override bool HasChildren
        {
            get { return true; }
        }

        protected ExecutionResult ExecuteChild(ITask<T> child, TaskContext<T> context)
        {
            return context.Executer.Execute(child);
        }

        protected void ApplyChildResult(ExecutionResult result, ResultBuilder resultBuilder)
        {
            if (result.ResultType != ResultType.Success)
            {
                resultBuilder.Fail();
            }
        }
    }
}