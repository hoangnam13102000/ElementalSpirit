using ElementalSpirit.Domain.Spirit;

namespace ElementalSpirit.Factories
{
    public enum SpiritType { Earth, Fire, Water, Wind }

    public static class SpiritFactory
    {
        public static ISpirit Create(SpiritType type) => type switch
        {
            SpiritType.Earth => new EarthSpirit(),
            SpiritType.Fire => new FireSpirit(),
            SpiritType.Water => new WaterSpirit(),
            SpiritType.Wind => new WindSpirit(),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}