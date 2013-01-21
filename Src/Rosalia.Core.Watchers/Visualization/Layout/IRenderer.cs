namespace Rosalia.Core.Watchers.Visualization.Layout
{
    public interface IRenderer
    {
        Size GetItemSize(LayoutItem item);

        LayoutType GetChildrenLayoutType(LayoutItem item);
    }
}