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
            float speed = type switch
            {
                ProjectileType.Slash => 520f * direction,
                ProjectileType.Fireball => 380f * direction,
                _ => 450f * direction
            };
            return new PlayerProjectile(x, y, speed, damage, type);
        }
    }
}