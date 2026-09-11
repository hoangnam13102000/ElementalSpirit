namespace ElementalSpirit.Domain.Equipment
{
    public enum EquipmentSlot
    {
        Weapon,
        Armor,
        Accessory
    }

    public abstract class Equipment
    {
        public string Id { get; }
        public string Name { get; }
        public EquipmentSlot Slot { get; }
        public int PriceGold { get; }

        public int BonusDamage { get; protected set; }
        public int BonusMaxHp { get; protected set; }
        public int BonusDefense { get; protected set; }

        protected Equipment(string id, string name, EquipmentSlot slot, int priceGold)
        {
            Id = id;
            Name = name;
            Slot = slot;
            PriceGold = priceGold;
        }

        public abstract Equipment Clone();
    }
}