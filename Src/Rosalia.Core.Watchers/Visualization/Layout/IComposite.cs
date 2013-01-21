namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System.Collections.Generic;

    public interface IComposite
    {
        IEnumerable<IComposite> ChildrenItems { get; }
    }
}