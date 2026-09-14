
namespace ElementalSpirit.Domain.Projectile
{
    public enum ProjectileType
    {
        Basic,
        Fireball,
        Slash
    }

    public class PlayerProjectile : Projectile
    {
        private const float SlashDamageMultiplier = 1.5f;
        private const int BasicWidth = 10;
        private const int BasicHeight = 6;
        private const int FireballSize = 24;
        private const int SlashWidth = 42;
        private const int SlashHeight = 44;

        public ProjectileType Type { get; }
        public bool IsFireball => Type == ProjectileType.Fireball;

        public PlayerProjectile(
            float x,
            float y,
            float speed,
            int damage,
            ProjectileType type = ProjectileType.Basic)
            : base(x, y, speed, 0f, damage, type == ProjectileType.Fireball ? 3.0f : 2.5f)
        {
            Type = type;
            if (type == ProjectileType.Slash)
                Damage = Math.Max(Damage + 1, (int)MathF.Round(Damage * SlashDamageMultiplier));

            if (type == ProjectileType.Fireball)
            {
                Width = FireballSize;
                Height = FireballSize;
            }
            else if (type == ProjectileType.Slash)
            {
                Width = SlashWidth;
                Height = SlashHeight;
            }
            else
            {
                Width = BasicWidth;
                Height = BasicHeight;
            }
        }
    }
}