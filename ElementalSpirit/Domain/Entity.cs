using System.Drawing;

namespace ElementalSpirit.Domain
{
    public abstract class Entity
    {
        public float X { get; protected set; }
        public float Y { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        protected Entity(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
