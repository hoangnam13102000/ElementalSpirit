using System.Drawing;

namespace ElementalSpirit.Domain.Stage
{
    public class TerrainWall
    {
        public string Name { get; }
        public float XRatio { get; }
        public float TopRatio { get; }
        public float BottomRatio { get; }

        public TerrainWall(string name, float xRatio, float topRatio, float bottomRatio)
        {
            Name = name;
            XRatio = xRatio;
            TopRatio = topRatio;
            BottomRatio = bottomRatio;
        }

        public RectangleF GetAbsoluteBounds(RectangleF playArea)
        {
            float x = playArea.Left + playArea.Width * XRatio;
            float top = playArea.Top + playArea.Height * TopRatio;
            float bottom = playArea.Top + playArea.Height * BottomRatio;
            return new RectangleF(x, top, 1f, bottom - top);
        }
    }
}
