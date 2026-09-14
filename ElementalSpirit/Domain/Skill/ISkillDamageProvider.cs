using System.Collections.Generic;

namespace ElementalSpirit.Domain.Skill
{
    public interface ISkillDamageProvider
    {
        IReadOnlyList<SkillDamageArea> CreateDamageAreas(SkillDamageContext context);
    }
}
