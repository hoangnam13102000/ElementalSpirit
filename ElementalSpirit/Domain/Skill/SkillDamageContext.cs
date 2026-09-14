namespace ElementalSpirit.Domain.Skill
{
    public sealed class SkillDamageContext
    {
        public float OriginX { get; }
        public float OriginY { get; }
        public float Direction { get; }
        public int Damage { get; }

        public SkillDamageContext(float originX, float originY, float direction, int damage)
        {
            OriginX = originX;
            OriginY = originY;
            Direction = direction;
            Damage = damage;
        }
    }
}
