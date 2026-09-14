using System.Drawing;

namespace ElementalSpirit.Domain.Skill
{
    public sealed class SkillDamageArea
    {
        public RectangleF Bounds { get; }
        public int Damage { get; }

        public SkillDamageArea(RectangleF bounds, int damage)
        {
            Bounds = bounds;
            Damage = damage;
        }
    }
}
