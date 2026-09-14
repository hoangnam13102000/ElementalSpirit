namespace ElementalSpirit.Domain.Skill
{
    public interface ISkill
    {
        string Id { get; }
        string Name { get; }
        string IconAssetKey { get; }
        float IconScale { get; }
        bool IsActive { get; }
        float CooldownRemaining { get; }
        SkillAnimationState AnimationState { get; }
        bool Activate(SkillContext context);
        void Update(float deltaTime);
    }
}
