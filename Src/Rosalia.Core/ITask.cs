namespace Rosalia.Core
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;

    public interface ITask<TContext> : IExecutable<TContext>, IIdentifiable
    {
        bool HasChildren { get; }

        IEnumerable<ITask<TContext>> Children { get; }

        Action<TaskContext<TContext>> BeforeExecute { get; set; }

        Action<TaskContext<TContext>> AfterExecute { get; set; }
    }
}