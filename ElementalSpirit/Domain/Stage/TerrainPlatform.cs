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

        // Tra ve vi tri/kich thuoc thuc te (pixel) cua nen da, dua tren vung choi hien tai
        public RectangleF GetAbsoluteBounds(RectangleF playArea)
        {
            float x = playArea.Left + playArea.Width * MinXRatio;
            float width = playArea.Width * (MaxXRatio - MinXRatio);
            float y = playArea.Top + playArea.Height * TopRatio;
            float height = playArea.Height * (BottomRatio - TopRatio);
            return new RectangleF(x, y, width, height);
        }
    }
    // =====================================================================
}