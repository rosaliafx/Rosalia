namespace Rosalia.Core.Engine.Composing
{
    using Rosalia.Core.Tasks;

    public class ExecutableWithIdentity
    {
        private readonly Identity _id;
        private readonly ITask<object> _task;

        public ExecutableWithIdentity(ITask<object> task, Identity id)
        {
            _id = id;
            _task = task;
        }

        public Identity Id
        {
            get { return _id; }
        }

        public ITask<object> Task
        {
            get { return _task; }
        }
    }
}