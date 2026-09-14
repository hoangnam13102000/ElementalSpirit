using ElementalSpirit.Domain;

namespace ElementalSpirit.Domain.Projectile
{
    public abstract class Projectile : Entity
    {
        public float VelocityX { get; protected set; }
        public float VelocityY { get; protected set; }
        public float HorizontalDirection => Math.Sign(VelocityX);
        public int Damage { get; protected set; }
        public float Lifetime { get; protected set; }
        public bool IsAlive { get; protected set; } = true;
        protected Projectile(float x, float y, float vx, float vy, int damage, float lifetime = 2.5f)
            : base(x, y, 8, 8)
        {
            X = x;
            Y = y;
            VelocityX = vx;
            VelocityY = vy;
            Damage = damage;
            Lifetime = lifetime;
        }

        public virtual void Update(float deltaTime)
        {
            if (!IsAlive) return;
            X += VelocityX * deltaTime;
            Y += VelocityY * deltaTime;
            Lifetime -= deltaTime;
            if (Lifetime <= 0)
                IsAlive = false;
        }

        public void Kill()
        {
            IsAlive = false;
        }
    }
}