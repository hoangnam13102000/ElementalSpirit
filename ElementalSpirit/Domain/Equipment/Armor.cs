namespace ElementalSpirit.Domain.Equipment
{
    public class Armor : Equipment
    {
        public Armor(string id, string name, int priceGold, int bonusMaxHp, int bonusDefense)
            : base(id, name, EquipmentSlot.Armor, priceGold)
        {
            BonusMaxHp = bonusMaxHp;
            BonusDefense = bonusDefense;
        }

        public override Equipment Clone()
            => new Armor(Id, Name, PriceGold, BonusMaxHp, BonusDefense);
    }
}