namespace ElementalSpirit.Domain.Spirit
{
    using PlayerEntity = ElementalSpirit.Domain.Player.Player;

    public class WindSpirit : SpiritBase
    {
        private int _extraProjectiles = 1;

        public WindSpirit() : base("spirit_wind", "Zephyr", "Wind", 11f, 6f) { }

        protected override void ApplyEffect(PlayerEntity player)
        {
            player.ActivateWindBarrage(_extraProjectiles);
        }

        protected override void OnExpire(PlayerEntity player)
        {
            player.DeactivateWindBarrage();
        }

        protected override void OnLevelUp()
        {
            _extraProjectiles = Level switch { 2 => 2, 3 => 3, 4 => 5, 5 => 7, _ => 1 };
            SkillDuration = 6f + (Level - 1) * 0.5f;
            CooldownDuration = Math.Max(8f, 11f - (Level - 1) * 0.5f);
        }
    }
}