using System;
using System.Drawing;

namespace ElementalSpirit.Domain.Stage
{
    public sealed class Portal
    {
        public PortalType Type { get; }
        public int TargetStageIndex { get; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; }
        public float Height { get; }
        public bool IsVisible { get; set; }
        public bool IsActive { get; set; } = true;

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        public Portal(
            PortalType type,
            int targetStageIndex,
            float x,
            float y,
            float width = 80f,
            float height = 140f)
        {
            Type = type;
            TargetStageIndex = targetStageIndex;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            IsVisible = type == PortalType.BackPortal; // Cổng về mặc định hiện
        }

        public void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool Intersects(RectangleF bounds)
        {
            return IsVisible && IsActive && Bounds.IntersectsWith(bounds);
        }
    }
}