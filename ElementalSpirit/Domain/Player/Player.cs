using System;
using ElementalSpirit.Domain;

namespace ElementalSpirit.Domain.Player
{
    public enum PlayerMovementState
    {
        Idle,
        Running,
        Jumping,
        Falling
    }

    public class Player : Character
    {
        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }
        public float WalkSpeed { get; private set; } = PlayerConstants.WalkSpeed;
        public float RunSpeed { get; private set; } = PlayerConstants.RunSpeed;
        public float CurrentMoveSpeed { get; private set; }
        public bool WantsToRun { get; private set; }
        public float JumpForce { get; private set; } = PlayerConstants.JumpForce;
        public float Gravity { get; private set; } = PlayerConstants.Gravity;
        public bool IsGrounded { get; private set; }
        public PlayerMovementState MovementState { get; private set; }

        public FacingDirection Facing { get; private set; } = FacingDirection.Right;

        public int BaseMaxHp { get; private set; } = PlayerConstants.BaseMaxHp;
        public int CurrentHp => HP;
        public int CoreDamage { get; private set; } = PlayerConstants.BaseDamage;
        public int BaseDamage { get; private set; } = PlayerConstants.BaseDamage;
        public int Defense { get; private set; } = PlayerConstants.BaseDefense;
        private float _damageMultiplier = 1f;

        public bool IsInvulnerable { get; private set; }
        private float _invulnerabilityTimer;
        public float InvulnerabilityDuration { get; private set; } = PlayerConstants.InvulnerabilityDuration;

        public bool ActiveShield { get; private set; }
        public bool ActiveFireBoost { get; private set; }
        public bool ActiveHealEffect { get; private set; }
        public bool ActiveWindBarrage { get; private set; }
        public int ExtraProjectiles { get; private set; }

        public float AttackCooldown { get; private set; }
        public float AttackInterval { get; private set; } = PlayerConstants.AttackInterval;

        public bool IsAttacking { get; private set; }
        public bool IsFiring { get; private set; }

        public event Action? OnAttackHitFrame;
        public event Action? OnFireCastFrame;
        public event Action? OnAttackAnimationEnded;
        public event Action? OnFireAnimationEnded;
        public event Action? OnHurtAnimationEnded;
        public event Action? OnDeathAnimationEnded;

        public Player(float startX, float startY)
            : base(startX, startY, PlayerConstants.Width, PlayerConstants.Height,
                  PlayerConstants.BaseMaxHp, PlayerConstants.BaseDamage)
        {
            VelocityX = 0f;
            VelocityY = 0f;
            IsGrounded = false;
            MovementState = PlayerMovementState.Idle;
            CurrentMoveSpeed = WalkSpeed;
        }

        public void MoveHorizontal(float dirX, float deltaTime)
        {
            dirX = Math.Clamp(dirX, -1f, 1f);
            CurrentMoveSpeed = WantsToRun ? RunSpeed : WalkSpeed;
            VelocityX = dirX * CurrentMoveSpeed;
            if (dirX > 0.1f) Facing = FacingDirection.Right;
            else if (dirX < -0.1f) Facing = FacingDirection.Left;
        }

        public void SetWantsToRun(bool wantsToRun) => WantsToRun = wantsToRun;

        public void ApplyVelocityDamping(float factor) =>
            VelocityX *= Math.Clamp(factor, 0f, 1f);

        public void ActivateFireBoost(float damageMultiplier)
        {
            ActiveFireBoost = true;
            ApplyDamageMultiplier(damageMultiplier);
        }

        public void DeactivateFireBoost()
        {
            ActiveFireBoost = false;
            ResetDamageMultiplier();
        }

        public void ActivateShield()
        {
            ActiveShield = true;
            IsInvulnerable = true;
        }

        public void DeactivateShield()
        {
            ActiveShield = false;
            IsInvulnerable = false;
        }

        public void ActivateHeal(int instantAmount)
        {
            ActiveHealEffect = true;
            Heal(instantAmount);
        }

        public void TickHeal(int amount)
        {
            if (amount > 0) Heal(amount);
        }

        public void DeactivateHeal() => ActiveHealEffect = false;

        public void ActivateWindBarrage(int extraProjectiles)
        {
            ActiveWindBarrage = true;
            ExtraProjectiles = Math.Max(0, extraProjectiles);
        }

        public void DeactivateWindBarrage()
        {
            ActiveWindBarrage = false;
            ExtraProjectiles = 0;
        }

        public void TryJump()
        {
            if (!IsGrounded || IsDead || IsHurt) return;
            VelocityY = -JumpForce;
            IsGrounded = false;
        }

        public void Update(float deltaTime)
        {
            if (AttackCooldown > 0) AttackCooldown -= deltaTime;
            if (_invulnerabilityTimer > 0)
            {
                _invulnerabilityTimer -= deltaTime;
                if (_invulnerabilityTimer <= 0) IsInvulnerable = false;
            }
            if (IsDead)
            {
                VelocityY += Gravity * deltaTime;
                if (VelocityY > PlayerConstants.MaxFallSpeed) VelocityY = PlayerConstants.MaxFallSpeed;
                Y += VelocityY * deltaTime;
                return;
            }
            if (!IsGrounded)
            {
                VelocityY += Gravity * deltaTime;
                if (VelocityY > PlayerConstants.MaxFallSpeed) VelocityY = PlayerConstants.MaxFallSpeed;
            }
            X += VelocityX * deltaTime;
            Y += VelocityY * deltaTime;
            UpdateMovementState();
        }

        public void ResolveGroundCollision(float groundY)
        {
            float playerBottom = Y + Height;
            if (playerBottom >= groundY)
            {
                Y = groundY - Height;
                VelocityY = 0f;
                IsGrounded = true;
            }
            else
            {
                if (VelocityY > 1f || VelocityY < -1f) IsGrounded = false;
            }
        }

        public void ClampHorizontalBounds(float minX, float maxX)
        {
            if (X < minX) X = minX;
            if (X + Width > maxX) X = maxX - Width;
        }

        public override void TakeDamage(int amount)
        {
            if (IsInvulnerable || ActiveShield || IsDead) return;
            int reduced = Math.Max(1, amount - Defense);
            HP = Math.Max(0, HP - reduced);
            if (HP <= 0) StartDeath();
            else StartHurt();
        }

        public void Heal(int amount) => HP = Math.Min(MaxHp, HP + amount);

        public void ApplyEquipmentBonuses(int bonusDamage, int bonusMaxHp, int bonusDefense)
        {
            float hpRatio = MaxHp > 0 ? (float)HP / MaxHp : 1f;
            BaseDamage = CoreDamage + bonusDamage;
            MaxHp = BaseMaxHp + bonusMaxHp;
            Defense = bonusDefense;
            HP = Math.Clamp((int)(MaxHp * hpRatio + 0.5f), 0, MaxHp);
            Damage = Math.Max(1, (int)(BaseDamage * _damageMultiplier));
        }

        public void ApplyDamageMultiplier(float multiplier)
        {
            _damageMultiplier = multiplier;
            Damage = Math.Max(1, (int)(BaseDamage * _damageMultiplier));
        }

        public void ResetDamageMultiplier()
        {
            _damageMultiplier = 1f;
            Damage = BaseDamage;
        }

        public bool CanAttack() => AttackCooldown <= 0 && !IsAttacking && !IsFiring && !IsHurt && !IsDead;
        public bool CanFire() => AttackCooldown <= 0 && !IsAttacking && !IsFiring && !IsHurt && !IsDead;
        public void ResetAttackCooldown() => AttackCooldown = AttackInterval;

        public void StartAttack() { if (!IsDead) IsAttacking = true; }
        public void StartFire() { if (!IsDead) IsFiring = true; }

        private void StartHurt()
        {
            IsHurt = true;
            IsInvulnerable = true;
            _invulnerabilityTimer = InvulnerabilityDuration;
        }

        private void StartDeath()
        {
            IsDead = true;
            IsAttacking = false;
            IsFiring = false;
            IsHurt = false;
            VelocityX = 0f;
        }

        public void NotifyAttackHitFrame() => OnAttackHitFrame?.Invoke();
        public void NotifyFireCastFrame() => OnFireCastFrame?.Invoke();
        public void NotifyAttackAnimationEnded() { IsAttacking = false; OnAttackAnimationEnded?.Invoke(); }
        public void NotifyFireAnimationEnded() { IsFiring = false; OnFireAnimationEnded?.Invoke(); }
        public void NotifyHurtAnimationEnded() { IsHurt = false; OnHurtAnimationEnded?.Invoke(); }
        public void NotifyDeathAnimationEnded() => OnDeathAnimationEnded?.Invoke();

        private void UpdateMovementState()
        {
            if (!IsGrounded)
                MovementState = VelocityY < 0 ? PlayerMovementState.Jumping : PlayerMovementState.Falling;
            else if (Math.Abs(VelocityX) > 1f)
                MovementState = PlayerMovementState.Running;
            else
                MovementState = PlayerMovementState.Idle;
        }
    }
}