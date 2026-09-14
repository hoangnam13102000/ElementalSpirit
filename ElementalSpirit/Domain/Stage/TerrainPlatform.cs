using System.Drawing;

namespace ElementalSpirit.Domain.Stage
{
    public class TerrainPlatform
    {
        public string Name { get; }
        public float MinXRatio { get; }
        public float MaxXRatio { get; }
        public float TopRatio { get; }
        public float BottomRatio { get; }

        public TerrainPlatform(string name, float minXRatio, float maxXRatio, float topRatio, float bottomRatio)
        {
            Name = name;
            MinXRatio = minXRatio;
            MaxXRatio = maxXRatio;
            TopRatio = topRatio;
            BottomRatio = bottomRatio;
        }

        public RectangleF GetAbsoluteBounds(RectangleF playArea)
        {
            float x = playArea.Left + playArea.Width * MinXRatio;
            float width = playArea.Width * (MaxXRatio - MinXRatio);
            float y = playArea.Top + playArea.Height * TopRatio;
            float height = playArea.Height * (BottomRatio - TopRatio);
            return new RectangleF(x, y, width, height);
        }

        public float GetAbsoluteTopY(RectangleF playArea)
        {
            return playArea.Top + playArea.Height * TopRatio;
        }

        public bool ContainsAbsoluteX(RectangleF playArea, float worldX)
        {
            float minX = playArea.Left + playArea.Width * MinXRatio;
            float maxX = playArea.Left + playArea.Width * MaxXRatio;
            return worldX >= minX && worldX <= maxX;
        }
    }
    // =====================================================================
}