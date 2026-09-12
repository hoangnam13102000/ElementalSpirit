using System;
using System.Drawing;

namespace ElementalSpirit.Domain.Player
{
    /// <summary>
    /// Simple movement states to prepare for future animation system.
    /// Physics does not depend on these; they are derived from velocity.
    /// </summary>
    public enum PlayerMovementState
    {
        Idle,
        Running,
        Jumping,
        Falling
    }

    public class Player
    {
        // ===== Position & size =====
        public float X { get; private set; }
        public float Y { get; private set; }
        public int Width { get; } = 32;
        public int Height { get; } = 32;
        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        // ===== Platformer physics =====
        /// <summary>Horizontal velocity in pixels/second.</summary>
        public float VelocityX { get; private set; }

        /// <summary>Vertical velocity in pixels/second (positive = downward).</summary>
        public float VelocityY { get; private set; }

        /// <summary>Horizontal movement speed.</summary>
        public float MoveSpeed { get; private set; } = 260f;

        /// <summary>Initial upward velocity when jumping.</summary>
        public float JumpForce { get; private set; } = 560f;

        /// <summary>Gravitational acceleration (pixels/second²).</summary>
        public float Gravity { get; private set; } = 1500f;

        /// <summary>True when the player is standing on solid ground.</summary>
        public bool IsGrounded { get; private set; }

        /// <summary>Derived movement state for future animation support.</summary>
        public PlayerMovementState MovementState { get; private set; }

        // ===== Legacy speed property kept for compatibility =====
        [Obsolete("Use MoveSpeed instead. Kept for any external references.")]
        public float Speed { get; private set; } = 220f;

        // ===== Combat stats =====
        public int BaseMaxHp { get; private set; } = 100;
        public int MaxHp { get; private set; } = 100;
        public int CurrentHp { get; private set; } = 100;
        public int CoreDamage { get; private set; } = 10;
        public int BaseDamage { get; private set; } = 10;
        public int Damage { get; private set; } = 10;
        public int Defense { get; private set; } = 0;
        private float _damageMultiplier = 1f;

        public bool IsInvulnerable { get; set; }
        public bool ActiveShield { get; set; }
        public bool ActiveFireBoost { get; set; }
        public bool ActiveHealEffect { get; set; }
        public bool ActiveWindBarrage { get; set; }
        public int ExtraProjectiles { get; set; }

        public float AttackCooldown { get; private set; }
        public float AttackInterval { get; private set; } = 0.25f;

        public bool IsAlive => CurrentHp > 0;

        public Player(float startX, float startY)
        {
            X = startX;
            Y = startY;
            VelocityX = 0f;
            VelocityY = 0f;
            IsGrounded = false;
            MovementState = PlayerMovementState.Idle;
        }

        /// <summary>
        /// Apply horizontal movement input. dirX should be -1, 0, or 1 (normalized).
        /// Sets VelocityX directly; actual position is updated in Update().
        /// </summary>
        public void MoveHorizontal(float dirX, float deltaTime)
        {
            dirX = Math.Clamp(dirX, -1f, 1f);
            VelocityX = dirX * MoveSpeed;
        }

        /// <summary>
        /// Attempt to jump. Only succeeds when IsGrounded is true.
        /// Sets an upward velocity (negative Y) and clears grounded state.
        /// </summary>
        public void TryJump()
        {
            if (!IsGrounded) return;

            VelocityY = -JumpForce;
            IsGrounded = false;
        }

        /// <summary>
        /// Core physics update: applies gravity, integrates velocity into position,
        /// and derives the current movement state.
        /// Ground collision is resolved separately by GameManager via ResolveGroundCollision.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (AttackCooldown > 0)
                AttackCooldown -= deltaTime;

            if (!IsGrounded)
            {
                VelocityY += Gravity * deltaTime;
                if (VelocityY > 1200f)
                    VelocityY = 1200f;
            }

            X += VelocityX * deltaTime;
            Y += VelocityY * deltaTime;

            UpdateMovementState();
        }

        /// <summary>
        /// Resolve collision with a flat ground plane at the given Y coordinate.
        /// If the player's bottom is at or below groundY, snap to ground and stop falling.
        /// </summary>
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
                if (VelocityY > 1f || VelocityY < -1f)
                    IsGrounded = false;
            }
        }

        /// <summary>
        /// Clamp horizontal position to stay within the play area.
        /// Vertical bounds are handled by gravity + ground collision.
        /// </summary>
        public void ClampHorizontalBounds(float minX, float maxX)
        {
            if (X < minX) X = minX;
            if (X + Width > maxX) X = maxX - Width;
        }

        [Obsolete("Use MoveHorizontal() and TryJump() for platformer movement.")]
        public void Move(float dirX, float dirY, float deltaTime)
        {
            MoveHorizontal(dirX, deltaTime);
        }

        [Obsolete("Use ClampHorizontalBounds() for platformer movement.")]
        public void ClampToBounds(float minX, float minY, float maxX, float maxY)
        {
            ClampHorizontalBounds(minX, maxX);
        }

        // ===== Combat methods (unchanged) =====
        public void TakeDamage(int amount)
        {
            if (IsInvulnerable || ActiveShield) return;
            int reduced = Math.Max(1, amount - Defense);
            CurrentHp = Math.Max(0, CurrentHp - reduced);
        }

        public void Heal(int amount)
        {
            CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
        }

        public void ApplyEquipmentBonuses(int bonusDamage, int bonusMaxHp, int bonusDefense)
        {
            float hpRatio = MaxHp > 0 ? (float)CurrentHp / MaxHp : 1f;
            BaseDamage = CoreDamage + bonusDamage;
            MaxHp = BaseMaxHp + bonusMaxHp;
            Defense = bonusDefense;
            CurrentHp = Math.Clamp((int)(MaxHp * hpRatio + 0.5f), 0, MaxHp);
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

        public bool CanAttack() => AttackCooldown <= 0;

        public void ResetAttackCooldown() => AttackCooldown = AttackInterval;

        private void UpdateMovementState()
        {
            if (!IsGrounded)
            {
                MovementState = VelocityY < 0
                    ? PlayerMovementState.Jumping
                    : PlayerMovementState.Falling;
            }
            else if (Math.Abs(VelocityX) > 1f)
            {
                MovementState = PlayerMovementState.Running;
            }
            else
            {
                MovementState = PlayerMovementState.Idle;
            }
        }
    }
}