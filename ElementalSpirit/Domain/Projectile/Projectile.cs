using System.Drawing;

namespace ElementalSpirit.Domain.Projectile
{
    public abstract class Projectile
    {
        public float X { get; protected set; }
        public float Y { get; protected set; }
        public float VelocityX { get; protected set; }
        public float VelocityY { get; protected set; }
        public int Damage { get; protected set; }
        public float Lifetime { get; protected set; }
        public bool IsAlive { get; protected set; } = true;
        public int Width { get; set; } = 8;
        public int Height { get; set; } = 8;
        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        protected Projectile(float x, float y, float vx, float vy, int damage, float lifetime = 2.5f)
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