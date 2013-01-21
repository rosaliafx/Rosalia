namespace Rosalia.Core.Watchers.Visualization
{
    using System;
    using System.Drawing;
    using Rosalia.Core.Watchers.Visualization.Layout;
    using Size = Rosalia.Core.Watchers.Visualization.Layout.Size;

    public class GrapthRenderer : IRenderer
    {
        private const int MarginX = 10;
        private const int MarginY = 10;

        private readonly Graphics _graphics;
        private readonly Font _font;

        public GrapthRenderer(Graphics graphics, Font font)
        {
            _graphics = graphics;
            _font = font;
        }

        public Size GetItemSize(LayoutItem item)
        {
            var textSize = _graphics.MeasureString(((Composite) item.Data).Task.Name, _font);

            return new Size(textSize.Width + MarginX * 2, textSize.Height * 2 + MarginY * 2);
        }

        public LayoutType GetChildrenLayoutType(LayoutItem item)
        {
            var task = ((Composite) item.Data).Task;

            if (IsInstanceOfGenericType(typeof(SequenceTask<>), task))
            {
                return LayoutType.Vertical;
            }

            return LayoutType.Horizontal;
        }

        private static bool IsInstanceOfGenericType(Type genericType, object instance)
        {
            Type type = instance.GetType();
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}