using System.Collections.Generic;
using System.Drawing;

namespace ElementalSpirit.Domain.Stage
{

    public class TerrainCollisionResolver
    {
        private const float GroundSnapTolerance = 4f;

        public bool TryGetSupportingGroundY(
            IReadOnlyList<TerrainPlatform> platforms,
            RectangleF playArea,
            float footX,
            float previousFootY,
            float currentFootY,
            out float groundY)
        {
            groundY = 0f;
            bool found = false;

            foreach (var platform in platforms)
            {
                if (!platform.ContainsAbsoluteX(playArea, footX)) continue;

                float topY = platform.GetAbsoluteTopY(playArea);

                bool wasAboveOrAtSurface = previousFootY <= topY + GroundSnapTolerance;
                bool nowAtOrBelowSurface = currentFootY >= topY;
                if (!wasAboveOrAtSurface || !nowAtOrBelowSurface) continue;

                if (!found || topY < groundY)
                {
                    groundY = topY;
                    found = true;
                }
            }

            return found;
        }
    }
}