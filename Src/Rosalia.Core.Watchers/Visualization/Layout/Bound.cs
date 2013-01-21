namespace Rosalia.Core.Watchers.Visualization.Layout
{
    public class Bound
    {
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }

        public double Width
        {
            get
            {
                return Left + Right;
            }
        }

        public double Height
        {
            get
            {
                return Top + Bottom;
            }
        }
    }
}