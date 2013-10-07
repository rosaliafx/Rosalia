namespace Rosalia.Core
{
    using System;

    public abstract class GenericWorkflow<TData> : Workflow, IGenericWorkflow<TData>
    {
        private TData _data;

        protected TData Data
        {
            get { return _data; }
        }

        public override ExecutionResult Execute(object inputData)
        {
            if (!(inputData is TData))
            {
                throw new Exception("Generic workflow was executed with wrong input data object");
            }

            _data = (TData)inputData;

            return base.Execute(inputData);
        }
    }
}