using System;
using ElementalSpirit.Domain;
using ElementalSpirit.Domain.Enemy.AI;
using ElementalSpirit.Domain.Projectile;
using ElementalSpirit.Domain.Skill;

namespace ElementalSpirit.Domain.Player
{
    public enum PlayerMovementState
    {
        Idle,
        Running,
        Jumping,
        Falling
    }

    public class Player : Character, IEnemyTarget, IProjectileLoadout
    {
        public string Name { get; private set; } = "Arin";
        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }
        public float WalkSpeed { get; private set; } = PlayerConstants.WalkSpeed;
        public float RunSpeed { get; private set; } = PlayerConstants.RunSpeed;
        public float CurrentMoveSpeed { get; private set; }
        public bool WantsToRun { get; private set; }
        public float JumpForce { get; private set; } = PlayerConstants.JumpForce;
        public float Gravity { get; private set; } = PlayerConstants.Gravity;
        public bool IsGrounded { get; private set; }
        public bool HasDoubleJumpBoots { get; private set; }
        private bool _doubleJumpConsumed;
        public PlayerMovementState MovementState { get; private set; }

        public FacingDirection Facing { get; private set; } = FacingDirection.Right;
        public void SetFacing(FacingDirection facing) => Facing = facing;

        public int CurrentHp => HP;
        public int Defense { get; private set; } = PlayerConstants.BaseDefense;

        public bool IsInvulnerable { get; private set; }
        private float _invulnerabilityTimer;
        public float InvulnerabilityDuration { get; private set; } = PlayerConstants.InvulnerabilityDuration;

        public float AttackCooldown { get; private set; }
        public float AttackInterval { get; private set; } = PlayerConstants.AttackInterval;

        public bool IsAttacking { get; private set; }
        public bool IsCastingSkill { get; private set; }
        public ProjectileType CurrentProjectileType { get; private set; } = ProjectileType.Fireball;

        public event Action? OnAttackHitFrame;
        public event Action? OnDeath;

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

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Player name cannot be empty.", nameof(name));

            Name = name.Trim();
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

        public bool TryJump()
        {
            if (IsDead || IsHurt) return false;

            if (IsGrounded)
            {
                VelocityY = -JumpForce;
                IsGrounded = false;
                _doubleJumpConsumed = false;
                return true;
            }

            if (HasDoubleJumpBoots && !_doubleJumpConsumed)
            {
                VelocityY = -JumpForce * 0.9f;
                _doubleJumpConsumed = true;
                return true;
            }

            return false;
        }

        public void GrantDoubleJumpBoots()
        {
            HasDoubleJumpBoots = true;
            _doubleJumpConsumed = false;
        }

        public void RestoreDoubleJumpBoots(bool hasBoots)
        {
            HasDoubleJumpBoots = hasBoots;
            _doubleJumpConsumed = false;
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
                _doubleJumpConsumed = false;
            }
            else
            {
                if (VelocityY > 1f || VelocityY < -1f) IsGrounded = false;
            }
        }

        public void LeaveGround() => IsGrounded = false;

        public void ClampHorizontalBounds(float minX, float maxX)
        {
            if (X < minX) X = minX;
            if (X + Width > maxX) X = maxX - Width;
        }

        public void RestoreHorizontalPosition(float x) => X = x;
        public void ResetPosition(float x, float y)
        {
            X = x;
            Y = y;
            VelocityX = 0f;
            VelocityY = 0f;
        }

        public override void TakeDamage(int amount)
        {
            if (IsInvulnerable || IsDead) return;
            int reduced = Math.Max(1, amount - Defense);
            HP = Math.Max(0, HP - reduced);
            if (HP <= 0) StartDeath();
            else StartHurt();
        }

        public void Die() => StartDeath();

        public void Heal(int amount) => HP = Math.Min(MaxHp, HP + amount);

        public void RestoreHp(int hp)
        {
            HP = Math.Clamp(hp, 1, MaxHp);
            IsDead = false;
            IsHurt = false;
        }

        public void SetProjectileType(ProjectileType projectileType) =>
            CurrentProjectileType = projectileType;

        public bool CanAttack() => AttackCooldown <= 0 && !IsAttacking && !IsHurt && !IsDead;
        public void ResetAttackCooldown() => AttackCooldown = AttackInterval;

        public void StartAttack() { if (!IsDead) IsAttacking = true; }
        public void StartSkillCast() { if (!IsDead) IsCastingSkill = true; }

        private void StartHurt()
        {
            IsHurt = true;
            IsInvulnerable = true;
            _invulnerabilityTimer = InvulnerabilityDuration;
        }

        private void StartDeath()
        {
            try
            {
                string savePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves", "savegame.json");
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                }
            }
            catch (Exception ex) when (
                ex is System.IO.IOException ||
                ex is UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine($"[Player] Could not delete save after death: {ex}");
            }

            IsDead = true;
            IsAttacking = false;
            IsCastingSkill = false;
            IsHurt = false;
            VelocityX = 0f;
            OnDeath?.Invoke();
        }

        public void NotifyAttackHitFrame() => OnAttackHitFrame?.Invoke();
        public void NotifyAttackAnimationEnded() { IsAttacking = false; }
        public void NotifySkillAnimationEnded() => IsCastingSkill = false;
        public void NotifyHurtAnimationEnded() { IsHurt = false; }
        public void NotifyDeathAnimationEnded() { }

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