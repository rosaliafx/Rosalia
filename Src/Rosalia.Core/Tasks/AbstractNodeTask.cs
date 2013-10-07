namespace Rosalia.Core.Tasks
{
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public abstract class AbstractNodeTask : AbstractTask
    {
        public override bool HasChildren
        {
            get { return true; }
        }

        protected ExecutionResult ExecuteChild(ITask child, TaskContext context)
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