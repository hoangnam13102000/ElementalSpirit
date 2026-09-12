using System;
using System.Drawing;
using ElementalSpirit.Domain;

namespace ElementalSpirit.Domain.Enemy
{
    public abstract class Enemy : Character
    {
        public int MaxHealth => MaxHp;
        public int Health => HP;
        public bool IsDying { get; protected set; }
        public bool IsDeathAnimationComplete { get; protected set; }
        public Domain.Player.FacingDirection Facing { get; protected set; } = Domain.Player.FacingDirection.Left;

        private float _hurtTimer;
        private float _deathTimer;
        protected virtual float HurtDuration => 0.35f;
        protected virtual float DeathDuration => 0.6f;

        protected Enemy(float x, float y, int maxHealth, int damage)
            : base(x, y, 36, 36, maxHealth, damage)
        {
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

        public override void TakeDamage(int amount)
        {
            if (!IsAlive) return;
            HP = Math.Max(0, HP - amount);
            if (HP <= 0) { IsDying = true; IsDead = true; IsHurt = false; _deathTimer = DeathDuration; }
            else { IsHurt = true; _hurtTimer = HurtDuration; }
        }

        public virtual void NotifyHurtAnimationEnded() { IsHurt = false; _hurtTimer = 0f; }
        public virtual void NotifyDeathAnimationEnded() { IsDeathAnimationComplete = true; _deathTimer = 0f; }
        public virtual void OnDeath() { }
    }
}