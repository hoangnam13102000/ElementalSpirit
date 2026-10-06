using System;
using System.Drawing;
using ElementalSpirit.Domain.Enemy.AI;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Domain.Enemy.NormalEnemy
{
    public class Slime : Enemy
    {
        public const string AssetKey = "Slime.png";

        private readonly float _speed = 70f;
        private float _dirX;

        private const float AttackRange = 55f;
        private const float DetectionRange = 280f;
        private const float AttackCooldownTime = 1.4f;
        private float _attackCooldown;
        private bool _isAttacking;
        private bool _attackHitRaised;

        private readonly IEnemyBehavior _behavior = new SimpleEnemyBehavior(DetectionRange, AttackRange);
        private IEnemyTarget? _target;

        protected override float HurtDuration => 0.65f;
        protected override float DeathDuration => 0.55f;

        private readonly SlimeAnimationController? _animController;
        private SlimeAnimationState _lastAnimState = SlimeAnimationState.Idle;

        public event Action<Slime>? OnAttackHit;

        public Image? CurrentImage => _animController?.CurrentImage;
        public SlimeAnimationState CurrentAnimState =>
            _animController?.CurrentState ?? SlimeAnimationState.Idle;
        public bool IsAttacking => _isAttacking;

        public bool CanHitTarget(IEnemyTarget target)
        {
            float slimeCenterX = X + Width / 2f;
            float slimeCenterY = Y + Height / 2f;
            float targetCenterX = target.X + target.Width / 2f;
            float targetCenterY = target.Y + target.Height / 2f;
            float deltaX = targetCenterX - slimeCenterX;
            float deltaY = targetCenterY - slimeCenterY;
            return deltaX * deltaX + deltaY * deltaY <= AttackRange * AttackRange;
        }

        public Slime(float x, float y, SlimeAnimationController? animController = null)
            : base(x, y, maxHealth: 30, damage: 8)
        {
            Width = 40;
            Height = 32;
            _animController = animController;
        }

        /// <summary>Inject mục tiêu combat (DIP: không hardcode Player).</summary>
        public void SetCombatTarget(float centerX, float centerY)
        {
            _target = new CoordinateEnemyTarget(centerX, centerY);
        }

        public void SetCombatTarget(IEnemyTarget? target) => _target = target;

        public override void Update(float deltaTime, float groundY, IEnemyTarget? target = null)
        {
            UpdateEffectTimers(deltaTime);
            UpdateAnimation(deltaTime);

            if (IsDying)
            {
                BehaviorState = EnemyBehaviorState.Dead;
                return;
            }

            if (_attackCooldown > 0f)
                _attackCooldown -= deltaTime;

            if (_isAttacking)
            {
                TryRaiseAttackHit();
                return;
            }

            if (target != null)
                _target = target;

            var decision = _behavior.Update(
                deltaTime,
                X + Width / 2f,
                Y + Height / 2f,
                _target);
            BehaviorState = decision.State;

            if (decision.State == EnemyBehaviorState.Attack && _attackCooldown <= 0f)
            {
                StartAttack();
                return;
            }

            float speedMultiplier = IsHurt ? 0.6f : 1f;
            _dirX = decision.DirectionX;
            X += _dirX * _speed * speedMultiplier * deltaTime;

            if (_dirX < -0.1f) Facing = FacingDirection.Left;
            else if (_dirX > 0.1f) Facing = FacingDirection.Right;
        }

        public override void ResolveGroundCollision(float groundY)
        {
            Y = groundY - Height;
        }

        private void StartAttack()
        {
            _isAttacking = true;
            _attackHitRaised = false;
            _dirX = 0f;

            if (_target != null)
            {
                float cx = X + Width / 2f;
                Facing = _target.X + _target.Width / 2f < cx
                    ? FacingDirection.Left
                    : FacingDirection.Right;
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

        public override void OnDeath()
        {
            OnAttackHit = null;
            _animController?.Dispose();
            base.OnDeath();
        }

        private sealed class CoordinateEnemyTarget : IEnemyTarget
        {
            public float X { get; }
            public float Y { get; }
            public int Width { get; }
            public int Height { get; }
            public bool IsAlive => true;

            public CoordinateEnemyTarget(float centerX, float centerY)
            {
                X = centerX;
                Y = centerY;
                Width = 0;
                Height = 0;
            }
        }
    }
}