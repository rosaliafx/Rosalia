namespace Rosalia.Core
{
    using System;
    using System.Collections.Generic;

    public interface IIdentifiable
    {
        /// <summary>
        /// Gets unique instance Id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets friendly name.
        /// </summary>
        string Name { get; }

        IEnumerable<IIdentifiable> IdentifiableChildren { get; }
    }
}