using System.Collections.Generic;
using ElementalSpirit.Domain.Skill;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface ISkillManager
    {
        IReadOnlyList<ISkill> Skills { get; }
        IReadOnlyList<string> UnlockedSkillIds { get; }
        SkillAnimationState CurrentAnimationState { get; }
        WaterfallSkill? ActiveWaterfall { get; }
        bool Unlock(string skillId);
        bool TryActivate(string skillId);
        IReadOnlyList<SkillDamageArea> CreateDamageAreas(
            float originX,
            float originY,
            float direction,
            int damage);
        void Update(float deltaTime);
    }
}
