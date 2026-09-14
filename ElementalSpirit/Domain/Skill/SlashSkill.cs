using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.Domain.Skill
{
    public sealed class SlashSkill : ISkill
    {
        private readonly SlashVariant _variant;
        private readonly IProjectileLoadout _projectileLoadout;

        public string Id => _variant.Id;
        public string Name => _variant.Name;
        public string IconAssetKey =>
            _projectileLoadout.CurrentProjectileType == _variant.ProjectileType
                ? _variant.AlternateIconAssetKey
                : _variant.IconAssetKey;
        public float IconScale => _variant.IconScale;
        public bool IsActive => false;
        public float CooldownRemaining => 0f;
        public SkillAnimationState AnimationState => SkillAnimationState.None;

        public SlashSkill(SlashVariant variant, IProjectileLoadout projectileLoadout)
        {
            _variant = variant ?? throw new System.ArgumentNullException(nameof(variant));
            _projectileLoadout = projectileLoadout
                ?? throw new System.ArgumentNullException(nameof(projectileLoadout));
        }

        public bool Activate(SkillContext context)
        {
            context.ProjectileLoadout.SetProjectileType(
                context.ProjectileLoadout.CurrentProjectileType == _variant.ProjectileType
                    ? ProjectileType.Basic
                    : _variant.ProjectileType);
            return true;
        }

        public void Update(float deltaTime)
        {
        }
    }
}
