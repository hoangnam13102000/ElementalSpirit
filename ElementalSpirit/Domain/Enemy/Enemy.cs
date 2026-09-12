using System;
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
        public Image? Image { get; protected set; }
        public bool IsAlive => Health > 0;
        public bool IsHurt { get; protected set; }
        public bool IsDying { get; protected set; }
        public bool IsDeathAnimationComplete { get; protected set; }
        public Domain.Player.FacingDirection Facing { get; protected set; } = Domain.Player.FacingDirection.Left;

        private float _hurtTimer;
        private float _deathTimer;
        protected virtual float HurtDuration => 0.35f;
        protected virtual float DeathDuration => 0.6f;

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        protected Enemy(float x, float y, int maxHealth, int damage)
        {
            X = x; Y = y; MaxHealth = maxHealth; Health = maxHealth; Damage = damage;
        }

        public abstract void Update(float deltaTime, float groundY);

        [Obsolete("Use Update(float deltaTime, float groundY) instead.")]
        public virtual void Update(float deltaTime) => Update(deltaTime, 0f);

        protected void UpdateEffectTimers(float deltaTime)
        {
            if (IsHurt)
            {
                _hurtTimer -= deltaTime;
                if (_hurtTimer <= 0f) NotifyHurtAnimationEnded();
            }
            if (IsDying && !IsDeathAnimationComplete)
            {
                _deathTimer -= deltaTime;
                if (_deathTimer <= 0f) NotifyDeathAnimationEnded();
            }
        }

        public virtual void TakeDamage(int amount)
        {
            if (!IsAlive) return;
            Health = Math.Max(0, Health - amount);
            if (Health <= 0) { IsDying = true; IsHurt = false; _deathTimer = DeathDuration; }
            else { IsHurt = true; _hurtTimer = HurtDuration; }
        }

        public virtual void NotifyHurtAnimationEnded() { IsHurt = false; _hurtTimer = 0f; }
        public virtual void NotifyDeathAnimationEnded() { IsDeathAnimationComplete = true; _deathTimer = 0f; }
        public virtual void OnDeath() { }
    }
}