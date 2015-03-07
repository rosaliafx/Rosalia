namespace Rosalia.Runner.Console.Startup
{
    using Rosalia.Core.Api;

    public class InitializationResult
    {
        private readonly bool _isSuccess;

        public InitializationResult()
        {
            _isSuccess = false;
        }

        public InitializationResult(IWorkflow workflow)
        {
            Workflow = workflow;
            _isSuccess = true;
        }

        public IWorkflow Workflow { get; private set; }

        public bool IsSuccess
        {
            get { return _isSuccess; }
        }
    }
}