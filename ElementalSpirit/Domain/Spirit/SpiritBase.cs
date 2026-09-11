namespace ElementalSpirit.Domain.Spirit
{
    using PlayerEntity = ElementalSpirit.Domain.Player.Player;

    public abstract class SpiritBase : ISpirit
    {
        public string Id { get; }
        public string Name { get; }
        public string Element { get; }
        public int Level { get; private set; } = 1;
        public float CooldownRemaining { get; private set; }
        public float CooldownDuration { get; protected set; }
        public float ActiveRemaining { get; private set; }
        public float SkillDuration { get; protected set; }
        public bool IsActive => ActiveRemaining > 0f;
        public bool CanActivate => !IsActive && CooldownRemaining <= 0f;

        protected SpiritBase(string id, string name, string element, float cooldown, float duration)
        {
            Id = id;
            Name = name;
            Element = element;
            CooldownDuration = cooldown;
            SkillDuration = duration;
        }

        public void Activate(PlayerEntity player)
        {
            if (!CanActivate) return;
            ActiveRemaining = SkillDuration;
            CooldownRemaining = CooldownDuration;
            ApplyEffect(player);
        }

        public void Update(float deltaTime, PlayerEntity player)
        {
            if (CooldownRemaining > 0f)
                CooldownRemaining = Math.Max(0f, CooldownRemaining - deltaTime);

            if (ActiveRemaining > 0f)
            {
                ActiveRemaining = Math.Max(0f, ActiveRemaining - deltaTime);
                OnActiveTick(player, deltaTime);
                if (ActiveRemaining <= 0f)
                    OnExpire(player);
            }
        }

        public virtual void Upgrade()
        {
            if (Level >= 5) return;
            Level++;
            OnLevelUp();
        }

        protected abstract void ApplyEffect(PlayerEntity player);
        protected virtual void OnActiveTick(PlayerEntity player, float deltaTime) { }
        protected abstract void OnExpire(PlayerEntity player);
        protected abstract void OnLevelUp();
    }
}