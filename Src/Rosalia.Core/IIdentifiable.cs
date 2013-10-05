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
        /// Gets or sets friendly name.
        /// </summary>
        string Name { get; set; }

        IEnumerable<IIdentifiable> IdentifiableChildren { get; }
    }
}