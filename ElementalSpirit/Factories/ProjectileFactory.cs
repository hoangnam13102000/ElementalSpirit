using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.Factories
{
    public static class ProjectileFactory
    {
        public static PlayerProjectile CreatePlayerProjectile(float x, float y, float direction, int damage)
        {
            float speed = 450f * direction;
            return new PlayerProjectile(x, y, speed, damage, isFireball: false);
        }

        public static PlayerProjectile CreateFireball(float x, float y, float direction, int damage)
        {
            float speed = 380f * direction;
            return new PlayerProjectile(x, y, speed, damage, isFireball: true);
        }
    }
}