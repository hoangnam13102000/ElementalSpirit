using System.Drawing;

namespace ElementalSpirit.Domain.Enemy
{
    public abstract class Enemy
    {
        public float X { get; protected set; }
        public float Y { get; protected set; }

        public int Width { get; protected set; } = 36;
        public int Height { get; protected set; } = 36;

        public int MaxHealth { get; protected set; }
        public int Health { get; protected set; }
        public int Damage { get; protected set; }

        public bool IsAlive => Health > 0;

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        protected Enemy(float x, float y, int maxHealth, int damage)
        {
            X = x;
            Y = y;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Damage = damage;
        }

        public abstract void Update(float deltaTime);

        public virtual void TakeDamage(int amount)
        {
            if (!IsAlive) return;
            Health = Math.Max(0, Health - amount);
        }

        public virtual void OnDeath()
        {
            // Override nếu cần drop item, effect...
        }
    }
}