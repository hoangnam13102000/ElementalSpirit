namespace ElementalSpirit.Domain
{
    public abstract class Character : Entity
    {
        public int MaxHp { get; protected set; }
        public int HP { get; protected set; }
        public int Damage { get; protected set; }
        public bool IsHurt { get; protected set; }
        public bool IsDead { get; protected set; }
        public bool IsAlive => HP > 0;

        protected Character(float x, float y, int width, int height, int maxHp, int damage)
            : base(x, y, width, height)
        {
            MaxHp = maxHp;
            HP = maxHp;
            Damage = damage;
        }

        public abstract void TakeDamage(int amount);
    }
}
