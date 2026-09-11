namespace ElementalSpirit.Domain.Projectile
{
    public class PlayerProjectile : Projectile
    {
        public PlayerProjectile(float x, float y, float speed, int damage)
            : base(x, y, speed, 0f, damage, 2.5f)
        {
            Width = 10;
            Height = 6;
        }
    }
}