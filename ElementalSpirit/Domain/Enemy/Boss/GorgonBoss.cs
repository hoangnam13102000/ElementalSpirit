using System;
using System.Drawing;
using ElementalSpirit.Domain.Enemy.AI;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Presentation.Assets;

namespace ElementalSpirit.Domain.Enemy
{
    public enum GorgonBossSkill
    {
        NuclearExplosion,
        BlueSpread
    }

    public sealed class GorgonBoss : Enemy
    {
        public const string AssetKey = "Enemies/Boss/Gorgon_1/Idle.png";
        private const float AttackRange = 300f;
        private const float MoveSpeed = 90f;
        private const float SkillCooldown = 2.2f;
        private const float SkillCastDuration = 0.7f;

        private IEnemyTarget? _target;
        private float _skillCooldown;
        private float _directionX = -1f;
        private readonly Random _random = new();
        private readonly GorgonBossAnimationController? _animController;

        private bool _isCastingSkill;
        private float _skillCastTimer;
        private bool _projectileFired;
        private GorgonBossSkill _currentSkill = GorgonBossSkill.BlueSpread;

        public event Action<GorgonBoss>? OnBossProjectileCast;
        public Image? CurrentImage => _animController?.CurrentImage;
        public GorgonAnimationState CurrentAnimState => _animController?.CurrentState ?? GorgonAnimationState.Idle;
        public GorgonBossSkill CurrentSkill => _currentSkill;

        public GorgonBoss(float x, float y, GorgonBossAnimationController? animController = null)
            : base(x, y, 1000, 18)
        {
            Width = 180;
            Height = 170;
            _skillCooldown = 1.2f;
            Facing = FacingDirection.Left;
            _animController = animController;
        }

        public override void Update(float deltaTime, float groundY, IEnemyTarget? target = null)
        {
            // Luôn cập nhật timer hiệu ứng gốc
            UpdateEffectTimers(deltaTime);

            // Animation luôn cập nhật
            _animController?.Update(deltaTime);

            // === TRẠNG THÁI CHẾT ===
            if (IsDying)
            {
                BehaviorState = EnemyBehaviorState.Dead;
                _animController?.Play(GorgonAnimationState.Dead);
                _isCastingSkill = false;
                _projectileFired = false;
                return;
            }

            if (IsHurt)
            {
                BehaviorState = EnemyBehaviorState.Idle;
                _isCastingSkill = false;
                _projectileFired = false;
                if (_animController?.CurrentState != GorgonAnimationState.Hurt)
                    _animController?.Play(GorgonAnimationState.Hurt, forceReset: true);
                ClampHorizontalBounds(0f, 1280f);
                ResolveGroundCollision(groundY);
                return;
            }

            // === KHÔNG CÓ MỤC TIÊU → IDLE ===
            if (target == null)
            {
                _target = null;
                if (!_isCastingSkill)
                {
                    BehaviorState = EnemyBehaviorState.Idle;
                    if (_animController?.CurrentState != GorgonAnimationState.Idle)
                        _animController?.Play(GorgonAnimationState.Idle);
                }
                return;
            }

            _target = target;

            // === Cooldown skill ===
            if (_skillCooldown > 0f)
                _skillCooldown -= deltaTime;

            // === TRẠNG THÁI ĐANG CAST SKILL ===
            if (_isCastingSkill)
            {
                _skillCastTimer -= deltaTime;

                if (!_projectileFired)
                {
                    var controller = _animController;
                    bool shouldFire = controller != null && controller.CurrentState == GorgonAnimationState.Special && controller.CurrentFrameIndex >= 1;

                    if (_skillCastTimer <= SkillCastDuration * 0.5f)
                        shouldFire = true;

                    if (shouldFire)
                    {
                        _projectileFired = true;
                        OnBossProjectileCast?.Invoke(this);
                    }
                }

                bool animDone = _animController?.IsCurrentCompleted ?? true;
                if (animDone && _skillCastTimer <= 0f)
                {
                    _isCastingSkill = false;
                    _projectileFired = false;
                }

                BehaviorState = EnemyBehaviorState.Attack;
                ClampHorizontalBounds(0f, 1280f);
                ResolveGroundCollision(groundY);
                return;
            }

            // === TÍNH KHOẢNG CÁCH ===
            float targetCenterX = _target.X + _target.Width / 2f;
            float bossCenterX = X + Width / 2f;
            float deltaX = targetCenterX - bossCenterX;
            float distanceX = Math.Abs(deltaX);
            bool inAttackRange = distanceX <= AttackRange;
            bool canCastSkill = inAttackRange && _skillCooldown <= 0f;

            // Boss keeps closing the distance while its skill is cooling down.
            // It stops only when a cast is ready or it has reached contact range.
            if (!canCastSkill && distanceX > 12f)
            {
                _directionX = deltaX >= 0f ? 1f : -1f;
                float movement = Math.Min(MoveSpeed * deltaTime, distanceX - 12f);
                X += _directionX * movement;
                BehaviorState = EnemyBehaviorState.Chase;
                Facing = deltaX >= 0f ? FacingDirection.Right : FacingDirection.Left;
                if (_animController?.CurrentState != GorgonAnimationState.Run)
                    _animController?.Play(GorgonAnimationState.Run);
            }
            else if (canCastSkill)
            {
                Facing = deltaX >= 0f ? FacingDirection.Right : FacingDirection.Left;
                BehaviorState = EnemyBehaviorState.Idle;
                if (_animController?.CurrentState != GorgonAnimationState.Idle)
                    _animController?.Play(GorgonAnimationState.Idle);
            }
            // === QUÁ GẦN → ĐỨNG YÊN ===
            else if (distanceX <= 12f)
            {
                BehaviorState = EnemyBehaviorState.Idle;
                if (_animController?.CurrentState != GorgonAnimationState.Idle)
                    _animController?.Play(GorgonAnimationState.Idle);
            }

            // === BẮT ĐẦU CAST SKILL TẦM XA ===
            if (canCastSkill)
            {
                _skillCooldown = SkillCooldown;
                _currentSkill = _currentSkill == GorgonBossSkill.NuclearExplosion
                    ? GorgonBossSkill.BlueSpread
                    : GorgonBossSkill.NuclearExplosion;
                _isCastingSkill = true;
                _skillCastTimer = SkillCastDuration;
                _projectileFired = false;
                BehaviorState = EnemyBehaviorState.Attack;
                _animController?.Play(GorgonAnimationState.Special, forceReset: true);
            }

            ClampHorizontalBounds(0f, 1280f);
            ResolveGroundCollision(groundY);
        }

        public override void ResolveGroundCollision(float groundY)
        {
            if (Y + Height > groundY)
                Y = groundY - Height;
        }

        // === CHỈ MẤT MÁU KHI ĐƯỢC GỌI TỪ BÊN NGOÀI ===
        // KHÔNG có logic nào tự giảm HP trong Update
        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);

            if (!IsAlive)
            {
                IsDying = true;
                IsDead = true;
                IsHurt = false;
            }
            else if (IsHurt)
            {
                _isCastingSkill = false;
                _projectileFired = false;
                _animController?.Play(GorgonAnimationState.Hurt, forceReset: true);
            }
        }

        public override void NotifyHurtAnimationEnded()
        {
            IsHurt = false;
        }

        public override void NotifyDeathAnimationEnded()
        {
            IsDeathAnimationComplete = true;
        }
    }
}