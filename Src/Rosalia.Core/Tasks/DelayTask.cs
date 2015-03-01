namespace Rosalia.Core.Tasks
{
    using System;
    using System.Threading;
    using Rosalia.Core.Tasks.Results;

    public class DelayTask : AbstractTask<Nothing>
    {
        private readonly TimeSpan _interval;

        public DelayTask(TimeSpan interval)
        {
            _interval = interval;
        }

        protected override ITaskResult<Nothing> SafeExecute(TaskContext context)
        {
            Thread.Sleep(_interval);
            return null;
        }
    }
}