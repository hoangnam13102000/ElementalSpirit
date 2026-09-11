namespace ElementalSpirit.Domain.Spirit
{
    using PlayerEntity = ElementalSpirit.Domain.Player.Player;

    public class EarthSpirit : SpiritBase
    {
        public EarthSpirit() : base("spirit_earth", "Terra", "Earth", 12f, 3f) { }

        protected override void ApplyEffect(PlayerEntity player)
        {
            player.ActiveShield = true;
            player.IsInvulnerable = true;
        }

        protected override void OnExpire(PlayerEntity player)
        {
            player.ActiveShield = false;
            player.IsInvulnerable = false;
        }

        protected override void OnLevelUp()
        {
            SkillDuration = Level switch { 2 => 4f, 3 => 5f, 4 => 6f, 5 => 8f, _ => 3f };
            CooldownDuration = Math.Max(8f, 12f - (Level - 1));
        }
    }
}