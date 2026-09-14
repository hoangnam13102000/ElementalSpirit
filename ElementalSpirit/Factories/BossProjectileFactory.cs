using System;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.Factories
{
    public static class BossProjectileFactory
    {
        public static EnemyProjectile CreateGorgonOrb(float originX, float originY, float targetX, float targetY, int damage)
        {
            float dx = targetX - originX;
            float dy = targetY - originY;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dist < 1f)
                dist = 1f;

            float speed = 240f;
            float vx = (dx / dist) * speed;
            float vy = (dy / dist) * speed;

            return new EnemyProjectile(originX, originY, vx, vy, damage, lifetime: 4.5f, radius: 12f, isTrackingPlayer: false);
        }

        public static EnemyProjectile CreateGorgonNuclearExplosion(
            float originX,
            float originY,
            float targetX,
            float targetY,
            int damage)
        {
            float dx = targetX - originX;
            float dy = targetY - originY;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            if (distance < 1f)
                distance = 1f;

            const float speed = 300f;
            return new EnemyProjectile(
                originX,
                originY,
                dx / distance * speed,
                dy / distance * speed,
                damage,
                lifetime: 4.0f,
                radius: 18f,
                isTrackingPlayer: false,
                type: EnemyProjectileType.NuclearExplosion);
        }

        public static EnemyProjectile[] CreateGorgonSpread(float originX, float originY, float targetX, float targetY, int damage)
        {
            float dx = targetX - originX;
            float dy = targetY - originY;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dist < 1f)
                dist = 1f;

            var list = new EnemyProjectile[5];
            float baseAngle = (float)Math.Atan2(dy, dx);
            for (int i = 0; i < 5; i++)
            {
                float angle = baseAngle + (i - 2) * 0.22f;
                float speed = 235f;
                float vx = (float)Math.Cos(angle) * speed;
                float vy = (float)Math.Sin(angle) * speed;
                list[i] = new EnemyProjectile(originX, originY, vx, vy, damage, lifetime: 4.0f, radius: 12f, isTrackingPlayer: false);
            }

            return list;
        }
    }
}
