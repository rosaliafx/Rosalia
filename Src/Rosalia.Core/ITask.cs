namespace Rosalia.Core
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public interface ITask : IExecutable, IIdentifiable
    {
        bool HasChildren { get; }

        IEnumerable<ITask> Children { get; }

        Action<TaskContext, ResultBuilder> BeforeExecute { get; set; }

        Action<TaskContext> AfterExecute { get; set; }
    }
}