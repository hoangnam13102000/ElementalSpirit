using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.Factories
{
    public static class ProjectileFactory
    {
        public static PlayerProjectile CreatePlayerProjectile(
            float x,
            float y,
            float direction,
            int damage,
            ProjectileType type = ProjectileType.Basic)
        {
            float speed = type == ProjectileType.Slash ? 520f * direction : 450f * direction;
            return new PlayerProjectile(x, y, speed, damage, type);
        }

        public static PlayerProjectile CreateFireball(float x, float y, float direction, int damage)
        {
            float speed = 380f * direction;
            return new PlayerProjectile(x, y, speed, damage, ProjectileType.Fireball);
        }
    }
}