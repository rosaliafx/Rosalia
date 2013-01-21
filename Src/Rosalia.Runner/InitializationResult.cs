namespace Rosalia.Runner
{
    using Rosalia.Core;

    public class InitializationResult
    {
        private readonly bool _isSuccess;

        public InitializationResult()
        {
            _isSuccess = false;
        }

        public InitializationResult(IWorkflow workflow, object context)
        {
            Workflow = workflow;
            Context = context;
            _isSuccess = true;
        }

        public IWorkflow Workflow { get; private set; }

        public object Context { get; private set; }

        public bool IsSuccess
        {
            get { return _isSuccess; }
        }
    }
}