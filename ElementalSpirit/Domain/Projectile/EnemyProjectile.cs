using System;

namespace ElementalSpirit.Domain.Projectile
{
    public enum EnemyProjectileType
    {
        BlueOrb,
        NuclearExplosion
    }

    public sealed class EnemyProjectile : Projectile
    {
        public float Radius { get; }
        public bool IsTrackingPlayer { get; }
        public EnemyProjectileType Type { get; }

        public EnemyProjectile(
            float x,
            float y,
            float vx,
            float vy,
            int damage,
            float lifetime = 3.0f,
            float radius = 10f,
            bool isTrackingPlayer = false,
            EnemyProjectileType type = EnemyProjectileType.BlueOrb)
            : base(x, y, vx, vy, damage, lifetime)
        {
            Radius = radius;
            IsTrackingPlayer = isTrackingPlayer;
            Type = type;
            Width = (int)Math.Ceiling(radius * 2f);
            Height = (int)Math.Ceiling(radius * 2f);
        }

        public override void Update(float deltaTime)
        {
            if (!IsAlive) return;
            X += VelocityX * deltaTime;
            Y += VelocityY * deltaTime;
            Lifetime -= deltaTime;
            if (Lifetime <= 0f)
                IsAlive = false;
        }
    }
}
