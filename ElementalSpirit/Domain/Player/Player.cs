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
        public float VelocityX { get; set; }
        public float VelocityY { get; private set; }
        public float WalkSpeed { get; private set; } = 140f;
        public float RunSpeed { get; private set; } = 280f;
        public float CurrentMoveSpeed { get; private set; }
        public bool WantsToRun { get; set; }
        public float JumpForce { get; private set; } = 560f;
        public float Gravity { get; private set; } = 1500f;
        public bool IsGrounded { get; private set; }
        public PlayerMovementState MovementState { get; private set; }

        [Obsolete("Use WalkSpeed/RunSpeed instead.")]
        public float Speed { get; private set; } = 220f;
        [Obsolete("Use WalkSpeed/RunSpeed instead.")]
        public float MoveSpeed { get; private set; } = 260f;

        public FacingDirection Facing { get; set; } = FacingDirection.Right;

        public int BaseMaxHp { get; private set; } = 100;
        public int CurrentHp => HP;
        public int CoreDamage { get; private set; } = 10;
        public int BaseDamage { get; private set; } = 10;
        public int Defense { get; private set; } = 0;
        private float _damageMultiplier = 1f;

        public bool IsInvulnerable { get; set; }
        private float _invulnerabilityTimer;
        public float InvulnerabilityDuration { get; private set; } = 0.8f;

        public bool ActiveShield { get; set; }
        public bool ActiveFireBoost { get; set; }
        public bool ActiveHealEffect { get; set; }
        public bool ActiveWindBarrage { get; set; }
        public int ExtraProjectiles { get; set; }

        public float AttackCooldown { get; private set; }
        public float AttackInterval { get; private set; } = 0.45f;

        public bool IsAttacking { get; private set; }
        public bool IsFiring { get; private set; }

        public event Action? OnAttackHitFrame;
        public event Action? OnFireCastFrame;
        public event Action? OnAttackAnimationEnded;
        public event Action? OnFireAnimationEnded;
        public event Action? OnHurtAnimationEnded;
        public event Action? OnDeathAnimationEnded;

        public Player(float startX, float startY)
            : base(startX, startY, 32, 32, 100, 10)
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
                if (VelocityY > 1200f) VelocityY = 1200f;
                Y += VelocityY * deltaTime;
                return;
            }
            if (!IsGrounded)
            {
                VelocityY += Gravity * deltaTime;
                if (VelocityY > 1200f) VelocityY = 1200f;
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

        [Obsolete("Use MoveHorizontal() and TryJump().")]
        public void Move(float dirX, float dirY, float deltaTime) => MoveHorizontal(dirX, deltaTime);

        [Obsolete("Use ClampHorizontalBounds().")]
        public void ClampToBounds(float minX, float minY, float maxX, float maxY) => ClampHorizontalBounds(minX, maxX);

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