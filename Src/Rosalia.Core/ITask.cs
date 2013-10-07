namespace Rosalia.Core
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;

    public interface ITask : IExecutable, IIdentifiable
    {
        bool HasChildren { get; }

        IEnumerable<ITask> Children { get; }

        Action<TaskContext> BeforeExecute { get; set; }

        Action<TaskContext> AfterExecute { get; set; }
    }
}