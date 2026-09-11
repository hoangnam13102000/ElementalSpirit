using System.Drawing;

namespace ElementalSpirit.Domain.Player
{

    public class Player
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public float Speed { get; private set; } = 220f; // pixels per second

        public int Width { get; } = 32;
        public int Height { get; } = 32;

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        public int MaxHp { get; private set; } = 100;
        public int CurrentHp { get; private set; } = 100;
        public int Damage { get; set; } = 10;   

        public bool IsInvulnerable { get; set; } = false;
        public bool ActiveShield { get; set; } = false;
        public bool ActiveFireBoost { get; set; } = false;
        public bool ActiveHealEffect { get; set; } = false;
        public bool ActiveWindBarrage { get; set; } = false;
        public int ExtraProjectiles { get; set; } = 0;

        public Player(float startX, float startY)
        {
            X = startX;
            Y = startY;
        }

        public void Move(float dirX, float dirY, float deltaTime)
        {
            X += dirX * Speed * deltaTime;
            Y += dirY * Speed * deltaTime;
        }

        public void ClampToBounds(float minX, float minY, float maxX, float maxY)
        {
            if (X < minX) X = minX;
            if (Y < minY) Y = minY;
            if (X + Width > maxX) X = maxX - Width;
            if (Y + Height > maxY) Y = maxY - Height;
        }

        public void TakeDamage(int amount)
        {
            CurrentHp = Math.Max(0, CurrentHp - amount);
        }

        public void Heal(int amount)
        {
            CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
        }

        public float AttackCooldown { get; private set; } = 0f;
        public float AttackInterval { get; private set; } = 0.25f; // bắn mỗi 0.25 giây

        public void Update(float deltaTime)
        {
            if (AttackCooldown > 0)
                AttackCooldown -= deltaTime;
        }

        public bool CanAttack() => AttackCooldown <= 0;

        public void ResetAttackCooldown()
        {
            AttackCooldown = AttackInterval;
        }

        public bool IsAlive => CurrentHp > 0;
    }
}