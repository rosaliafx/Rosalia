namespace Rosalia.Core
{
    using System.Collections.Generic;

    public interface ITask<TContext> : IExecutable<TContext>, IIdentifiable
    {
        bool HasChildren { get; }

        IEnumerable<ITask<TContext>> Children { get; }
    }
}