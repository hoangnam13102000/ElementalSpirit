using System.Collections.Generic;
using System.Drawing;

namespace ElementalSpirit.Domain.Stage
{
    public class TerrainWallResolver
    {
        public float ResolveHorizontalPosition(
            RectangleF entityBounds,
            float previousX,
            IReadOnlyList<TerrainWall> walls,
            RectangleF playArea)
        {
            float resolvedX = entityBounds.X;

            foreach (var wall in walls)
            {
                RectangleF wallBounds = wall.GetAbsoluteBounds(playArea);
                bool overlapsVertically =
                    entityBounds.Bottom >= wallBounds.Top &&
                    entityBounds.Top < wallBounds.Bottom;
                if (!overlapsVertically) continue;

                if (previousX + entityBounds.Width <= wallBounds.Left &&
                    entityBounds.Right > wallBounds.Left)
                {
                    resolvedX = wallBounds.Left - entityBounds.Width;
                }
                else if (previousX >= wallBounds.Right &&
                         entityBounds.Left < wallBounds.Right)
                {
                    resolvedX = wallBounds.Right;
                }
            }

            return resolvedX;
        }
    }
}
