
namespace ElementalSpirit.Domain.Projectile
{
    public class PlayerProjectile : Projectile
    {
        public bool IsFireball { get; }

        public PlayerProjectile(float x, float y, float speed, int damage, bool isFireball = false)
            : base(x, y, speed, 0f, damage, isFireball ? 3.0f : 2.5f)
        {
            IsFireball = isFireball;
            if (isFireball) { Width = 24; Height = 24; }
            else { Width = 10; Height = 6; }
        }
    }
}