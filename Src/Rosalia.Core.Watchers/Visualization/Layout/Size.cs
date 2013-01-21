namespace Rosalia.Core.Watchers.Visualization.Layout
{
    public class Size
    {
        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public Size()
        {
        }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}