using System;
using System.Collections.Generic;
using System.Linq;
using ElementalSpirit.Domain.Skill;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.GameEngine
{
    public sealed class SkillManager : ISkillManager
    {
        private readonly List<ISkill> _skills;
        private readonly SkillContext _context;

        public IReadOnlyList<ISkill> Skills => _skills;
        public SkillAnimationState CurrentAnimationState =>
            _skills.FirstOrDefault(skill => skill.IsActive)?.AnimationState
            ?? SkillAnimationState.None;

        public WaterfallSkill? ActiveWaterfall =>
            _skills.OfType<WaterfallSkill>().FirstOrDefault(skill => skill.IsActive);

        public SkillManager(IProjectileLoadout projectileLoadout)
        {
            if (projectileLoadout == null) throw new ArgumentNullException(nameof(projectileLoadout));
            _context = new SkillContext(projectileLoadout);
            _skills = new List<ISkill>
            {
                new SlashSkill(SlashVariant.Wind(), projectileLoadout),
                new WaterfallSkill()
            };
        }

        public bool TryActivate(string skillId)
        {
            var skill = _skills.FirstOrDefault(
                candidate => string.Equals(candidate.Id, skillId, StringComparison.OrdinalIgnoreCase)
                    || candidate.Id.StartsWith(
                        skillId + ".",
                        StringComparison.OrdinalIgnoreCase));
            if (skill == null) return false;

            return skill.Activate(_context);
        }

        public IReadOnlyList<SkillDamageArea> CreateDamageAreas(
            float originX,
            float originY,
            float direction,
            int damage)
        {
            var context = new SkillDamageContext(originX, originY, direction, damage);
            return _skills
                .Where(skill => skill.IsActive && skill is ISkillDamageProvider)
                .Cast<ISkillDamageProvider>()
                .SelectMany(provider => provider.CreateDamageAreas(context))
                .ToArray();
        }

        public void Update(float deltaTime)
        {
            foreach (var skill in _skills)
                skill.Update(deltaTime);
        }
    }
}
