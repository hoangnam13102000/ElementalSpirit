using System;
using System.Drawing;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Domain.Enemy.NormalEnemy
{
    public class Slime : Enemy
    {
        public const string AssetKey = "Slime.png";

        private readonly Random _random = new();
        private readonly float _speed = 70f;
        private float _dirX;
        private float _changeDirectionTimer;
        private float _changeDirectionInterval = 0.8f;

        private const float AttackRange = 55f;
        private const float AttackCooldownTime = 1.4f;
        private float _attackCooldown;
        private bool _isAttacking;
        private bool _attackHitRaised;

        private float _targetX;
        private float _targetY;
        private bool _hasTarget;

        protected override float HurtDuration => 0.65f;
        protected override float DeathDuration => 0.55f;

        private readonly SlimeAnimationController? _animController;
        private SlimeAnimationState _lastAnimState = SlimeAnimationState.Idle;

        public event Action<Slime>? OnAttackHit;

        public Image? CurrentImage => _animController?.CurrentImage;
        public SlimeAnimationState CurrentAnimState =>
            _animController?.CurrentState ?? SlimeAnimationState.Idle;
        public bool IsAttacking => _isAttacking;

        public Slime(float x, float y, SlimeAnimationController? animController = null)
            : base(x, y, maxHealth: 30, damage: 8)
        {
            Width = 40;
            Height = 32;
            _animController = animController;
            ChooseNewDirection();
        }

        /// <summary>Inject mục tiêu combat (DIP: không hardcode Player).</summary>
        public void SetCombatTarget(float centerX, float centerY)
        {
            _targetX = centerX;
            _targetY = centerY;
            _hasTarget = true;
        }

        public override void Update(float deltaTime, float groundY)
        {
            UpdateEffectTimers(deltaTime);
            UpdateAnimation(deltaTime);

            if (IsDying) return;

            if (_attackCooldown > 0f)
                _attackCooldown -= deltaTime;

            if (_isAttacking)
            {
                TryRaiseAttackHit();
                return;
            }

            if (_hasTarget && _attackCooldown <= 0f && IsTargetInAttackRange())
            {
                StartAttack();
                return;
            }

            float speedMultiplier = IsHurt ? 0.6f : 1f;
            _changeDirectionTimer -= deltaTime;
            if (_changeDirectionTimer <= 0f)
                ChooseNewDirection();

            X += _dirX * _speed * speedMultiplier * deltaTime;

            if (_dirX < -0.1f) Facing = FacingDirection.Left;
            else if (_dirX > 0.1f) Facing = FacingDirection.Right;
        }

        public override void ResolveGroundCollision(float groundY)
        {
            Y = groundY - Height;
        }

        private bool IsTargetInAttackRange()
        {
            float cx = X + Width / 2f;
            float cy = Y + Height / 2f;
            float dx = _targetX - cx;
            float dy = _targetY - cy;
            return dx * dx + dy * dy <= AttackRange * AttackRange;
        }

        private void StartAttack()
        {
            _isAttacking = true;
            _attackHitRaised = false;
            _dirX = 0f;

            if (_hasTarget)
            {
                float cx = X + Width / 2f;
                Facing = _targetX < cx ? FacingDirection.Left : FacingDirection.Right;
            }

            _animController?.Play(SlimeAnimationState.Attack);
            _lastAnimState = SlimeAnimationState.Attack;
        }

        private void TryRaiseAttackHit()
        {
            if (_attackHitRaised || _animController == null) return;

            int mid = Math.Max(1, _animController.CurrentClip.FrameCount / 2);
            if (_animController.CurrentFrameIndex >= mid)
            {
                _attackHitRaised = true;
                OnAttackHit?.Invoke(this);
            }
        }

        private void EndAttack()
        {
            _isAttacking = false;
            _attackCooldown = AttackCooldownTime;
            ChooseNewDirection();
        }

        private void UpdateAnimation(float deltaTime)
        {
            if (_animController == null) return;

            var desired = DetermineAnimationState();
            if (desired != _lastAnimState)
            {
                _animController.Play(desired);
                _lastAnimState = desired;
            }

            _animController.Update(deltaTime);

            if (_animController.IsCurrentCompleted)
            {
                if (desired == SlimeAnimationState.Hurt)
                    NotifyHurtAnimationEnded();
                else if (desired == SlimeAnimationState.Dead)
                    NotifyDeathAnimationEnded();
                else if (desired == SlimeAnimationState.Attack)
                    EndAttack();
            }
        }

        private SlimeAnimationState DetermineAnimationState()
        {
            if (IsDying || !IsAlive) return SlimeAnimationState.Dead;
            if (IsHurt) return SlimeAnimationState.Hurt;
            if (_isAttacking) return SlimeAnimationState.Attack;
            if (Math.Abs(_dirX) < 0.05f) return SlimeAnimationState.Idle;
            return SlimeAnimationState.Walk;
        }

        private void ChooseNewDirection()
        {
            int choice = _random.Next(0, 5);
            _dirX = choice switch
            {
                0 or 1 => -1f,
                2 or 3 => 1f,
                _ => 0f
            };

            _changeDirectionInterval = 0.6f + (float)_random.NextDouble() * 0.8f;
            _changeDirectionTimer = _changeDirectionInterval;
        }

        public override void OnDeath()
        {
            OnAttackHit = null;
            _animController?.Dispose();
            base.OnDeath();
        }
    }
}