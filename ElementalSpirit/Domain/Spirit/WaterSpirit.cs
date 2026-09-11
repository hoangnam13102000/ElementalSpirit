namespace ElementalSpirit.Domain.Spirit
{
    using PlayerEntity = ElementalSpirit.Domain.Player.Player;

    public class WaterSpirit : SpiritBase
    {
        private int _instantHeal = 25;
        private float _regenPerSecond = 5f;

        public WaterSpirit() : base("spirit_water", "Aqua", "Water", 14f, 4f) { }

        protected override void ApplyEffect(PlayerEntity player)
        {
            player.ActiveHealEffect = true;
            player.Heal(_instantHeal);
        }

        protected override void OnActiveTick(PlayerEntity player, float deltaTime)
        {
            int amount = (int)(_regenPerSecond * deltaTime + 0.5f);
            if (amount > 0) player.Heal(amount);
        }

        protected override void OnExpire(PlayerEntity player)
        {
            player.ActiveHealEffect = false;
        }

        protected override void OnLevelUp()
        {
            _instantHeal = Level switch { 2 => 30, 3 => 40, 4 => 50, 5 => 70, _ => 25 };
            _regenPerSecond = 5f + (Level - 1) * 2f;
            SkillDuration = 4f + (Level - 1) * 0.5f;
            CooldownDuration = Math.Max(9f, 14f - (Level - 1));
        }
    }
}