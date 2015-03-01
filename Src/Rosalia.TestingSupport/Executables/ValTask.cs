namespace Rosalia.TestingSupport.Executables
{
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class ValTask<T> : AbstractTask<T> where T : class
    {
        private readonly T _value;

        public ValTask(T value)
        {
            _value = value;
        }

        protected override ITaskResult<T> SafeExecute(TaskContext context)
        {
            return new SuccessResult<T>(_value);
        }
    }
}