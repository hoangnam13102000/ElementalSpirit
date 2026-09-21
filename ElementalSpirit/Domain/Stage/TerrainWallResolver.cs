using System.Collections.Generic;
using System.Drawing;

namespace ElementalSpirit.Domain.Stage
{
    public class TerrainWallResolver
    {
        // Player chỉ bị tường chặn khi chân đã tụt xuống thấp hơn mép trên của tường quá ngưỡng này
        // (tránh việc đứng sát mép lỗ trên mặt đất cũng bị coi là "đang ở trong lỗ").
        private const float PlayerLipTolerance = 2f;

        // Đẩy điểm chân lệch khỏi đúng đường tường 1 chút để platform kế bên không bắt nhầm.
        private const float PlayerWallEpsilon = 0.5f;

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

        /// <summary>
        /// Va chạm ngang của Player với các tường có BlocksPlayer = true (vd: 2 thành của cái lỗ ở màn 3).
        /// Tính theo ĐIỂM CHÂN (tâm thân Player) - cùng quy ước với TerrainCollisionResolver và việc
        /// vẽ sprite (sprite được căn giữa theo X của chân), nên Player rơi vào lỗ dễ dàng,
        /// nhưng không thể đi xuyên qua thành lỗ khi đang ở dưới mặt đất; phải nhảy lên khỏi mép.
        /// Trả về X (cạnh trái) mới của Player.
        /// </summary>
        public float ResolvePlayerHorizontalPosition(
            RectangleF playerBounds,
            float previousX,
            IReadOnlyList<TerrainWall> walls,
            RectangleF playArea)
        {
            float halfWidth = playerBounds.Width / 2f;
            float resolvedX = playerBounds.X;

            foreach (var wall in walls)
            {
                if (!wall.BlocksPlayer) continue;

                RectangleF wallBounds = wall.GetAbsoluteBounds(playArea);

                bool belowLip = playerBounds.Bottom > wallBounds.Top + PlayerLipTolerance;
                bool aboveWallBottom = playerBounds.Top < wallBounds.Bottom;
                if (!belowLip || !aboveWallBottom) continue;

                float wallX = wallBounds.Left;
                bool wasLeftOfWall = previousX + halfWidth < wallX;
                bool isLeftOfWall = resolvedX + halfWidth < wallX;
                if (wasLeftOfWall == isLeftOfWall) continue;

                float blockedFootX = wasLeftOfWall
                    ? wallX - PlayerWallEpsilon
                    : wallX + PlayerWallEpsilon;
                resolvedX = blockedFootX - halfWidth;
            }

            return resolvedX;
        }
    }
}