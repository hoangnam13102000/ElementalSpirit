using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.Factories
{
    public static class ProjectileFactory
    {
        public static PlayerProjectile CreatePlayerProjectile(float x, float y, float direction, int damage)
        {
            // direction: 1 = right, -1 = left
            float speed = 450f * direction;
            return new PlayerProjectile(x, y, speed, damage);
        }
    }
}