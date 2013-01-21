namespace Rosalia.Core.Watchers.Visualization.Layout
{
    public interface ICoordTransformer
    {
        void Init(LayoutData data);

        void Transform(Point point);
    }
}