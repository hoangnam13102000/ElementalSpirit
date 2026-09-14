namespace ElementalSpirit.Domain.Skill
{
    public sealed class SkillContext
    {
        public IProjectileLoadout ProjectileLoadout { get; }

        public SkillContext(IProjectileLoadout projectileLoadout)
        {
            ProjectileLoadout = projectileLoadout;
        }
    }
}
