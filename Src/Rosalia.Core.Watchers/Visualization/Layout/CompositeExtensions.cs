namespace Rosalia.Core.Watchers.Visualization.Layout
{
    using System.Linq;

    public static class CompositeExtensions
    {
        public static bool IsEmpty(this IComposite composite)
        {
            return composite.ChildrenItems == null || composite.ChildrenItems.Count() == 0;
        }
    }
}