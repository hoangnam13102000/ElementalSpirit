using System.Collections.Generic;
using ElementalSpirit.Domain.Equipment;

namespace ElementalSpirit.Domain.Inventory
{
    public class Inventory
    {
        private readonly List<Equipment.Equipment> _items = new();
        private readonly Dictionary<EquipmentSlot, Equipment.Equipment?> _equipped = new()
        {
            { EquipmentSlot.Weapon, null },
            { EquipmentSlot.Armor, null },
            { EquipmentSlot.Accessory, null }
        };

        public IReadOnlyList<Equipment.Equipment> Items => _items;
        public IReadOnlyDictionary<EquipmentSlot, Equipment.Equipment?> Equipped => _equipped;

        public void Add(Equipment.Equipment item)
        {
            if (_items.Exists(i => i.Id == item.Id))
                return;
            _items.Add(item);
        }

        public bool Owns(string itemId) => _items.Exists(i => i.Id == itemId);

        public bool TryEquip(string itemId)
        {
            var item = _items.Find(i => i.Id == itemId);
            if (item == null) return false;
            _equipped[item.Slot] = item;
            return true;
        }

        public void Unequip(EquipmentSlot slot)
        {
            _equipped[slot] = null;
        }

        public Equipment.Equipment? GetEquipped(EquipmentSlot slot)
            => _equipped.TryGetValue(slot, out var item) ? item : null;

        public int TotalBonusDamage
        {
            get
            {
                int sum = 0;
                foreach (var kv in _equipped)
                    if (kv.Value != null) sum += kv.Value.BonusDamage;
                return sum;
            }
        }

        public int TotalBonusMaxHp
        {
            get
            {
                int sum = 0;
                foreach (var kv in _equipped)
                    if (kv.Value != null) sum += kv.Value.BonusMaxHp;
                return sum;
            }
        }

        public int TotalBonusDefense
        {
            get
            {
                int sum = 0;
                foreach (var kv in _equipped)
                    if (kv.Value != null) sum += kv.Value.BonusDefense;
                return sum;
            }
        }
    }
}