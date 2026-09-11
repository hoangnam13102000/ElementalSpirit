namespace ElementalSpirit.Domain.Equipment
{
    public class Weapon : Equipment
    {
        public Weapon(string id, string name, int priceGold, int bonusDamage)
            : base(id, name, EquipmentSlot.Weapon, priceGold)
        {
            BonusDamage = bonusDamage;
        }

        public override Equipment Clone()
            => new Weapon(Id, Name, PriceGold, BonusDamage);
    }
}