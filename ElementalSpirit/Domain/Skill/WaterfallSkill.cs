using System;
using System.Collections.Generic;
using System.Drawing;

namespace ElementalSpirit.Domain.Skill
{
    public sealed class WaterfallSkill : ISkill, ISkillDamageProvider
    {
        public const int ColumnCount = 5;
        public const float ColumnSpacing = 72f;
        public const float ColumnWidth = 96f;
        public const float ColumnHeight = 180f;
        public const float ColumnInterval = 0.14f;
        public const float CooldownDuration = 4f;
        public const float DamageMultiplier = 2f;
        private const float AnimationDuration = ColumnCount * ColumnInterval + 0.35f;
        private float _remainingTime;
        private float _cooldownRemaining;
        private float _elapsedTime;

        public string Id => "waterfall";
        public string Name => "Waterfall";
        public string IconAssetKey => "Characters/Skill/Watermagic/WaterFall/water60002.png";
        public float IconScale => 0.62f;
        public bool IsActive => _remainingTime > 0f;
        public float CooldownRemaining => _cooldownRemaining;
        public int ActiveColumnCount =>
            IsActive
                ? System.Math.Min(
                    ColumnCount,
                    (int)(_elapsedTime / ColumnInterval) + 1)
                : 0;
        public SkillAnimationState AnimationState =>
            IsActive ? SkillAnimationState.Waterfall : SkillAnimationState.None;

        public IReadOnlyList<SkillDamageArea> CreateDamageAreas(SkillDamageContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var areas = new List<SkillDamageArea>(ColumnCount);
            float baseX = context.OriginX + context.Direction * ColumnSpacing;
            float columnY = context.OriginY - ColumnHeight;

            for (int column = 0; column < ColumnCount; column++)
            {
                float columnX = baseX + context.Direction * column * ColumnSpacing;
                areas.Add(new SkillDamageArea(
                    new RectangleF(
                        columnX - ColumnWidth / 2f,
                        columnY,
                        ColumnWidth,
                        ColumnHeight),
                    Math.Max(
                        context.Damage + 1,
                        (int)MathF.Round(context.Damage * DamageMultiplier))));
            }

            return areas;
        }

        public bool Activate(SkillContext context)
        {
            if (_cooldownRemaining > 0f) return false;
            _remainingTime = AnimationDuration;
            _cooldownRemaining = CooldownDuration;
            _elapsedTime = 0f;
            return true;
        }

        public void Update(float deltaTime)
        {
            if (_remainingTime > 0f)
            {
                _remainingTime = System.Math.Max(0f, _remainingTime - deltaTime);
                _elapsedTime += deltaTime;
            }
            else
            {
                _elapsedTime = 0f;
            }
            _cooldownRemaining = System.Math.Max(0f, _cooldownRemaining - deltaTime);
        }
    }
}
