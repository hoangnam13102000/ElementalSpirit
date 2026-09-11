namespace ElementalSpirit.Domain.Spirit
{
    using PlayerEntity = ElementalSpirit.Domain.Player.Player;

    public class FireSpirit : SpiritBase
    {
        private int _damageBonusPercent = 50;

        public FireSpirit() : base("spirit_fire", "Ignis", "Fire", 10f, 5f) { }

        protected override void ApplyEffect(PlayerEntity player)
        {
            player.ActiveFireBoost = true;
            player.ApplyDamageMultiplier(1f + _damageBonusPercent / 100f);
        }

        protected override void OnExpire(PlayerEntity player)
        {
            player.ActiveFireBoost = false;
            player.ResetDamageMultiplier();
        }

        protected override void OnLevelUp()
        {
            _damageBonusPercent = Level switch { 2 => 65, 3 => 80, 4 => 100, 5 => 130, _ => 50 };
            SkillDuration = 5f + (Level - 1) * 0.5f;
            CooldownDuration = Math.Max(7f, 10f - (Level - 1) * 0.5f);
        }
    }
}