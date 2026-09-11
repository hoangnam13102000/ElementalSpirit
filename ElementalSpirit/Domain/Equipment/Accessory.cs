namespace ElementalSpirit.Domain.Equipment
{
    public class Accessory : Equipment
    {
        public Accessory(string id, string name, int priceGold, int bonusDamage, int bonusMaxHp)
            : base(id, name, EquipmentSlot.Accessory, priceGold)
        {
            BonusDamage = bonusDamage;
            BonusMaxHp = bonusMaxHp;
        }

        public override Equipment Clone()
            => new Accessory(Id, Name, PriceGold, BonusDamage, BonusMaxHp);
    }
}