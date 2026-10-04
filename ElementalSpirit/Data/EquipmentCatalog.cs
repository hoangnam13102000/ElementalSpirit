using System.Collections.Generic;
using ElementalSpirit.Domain.Equipment;

namespace ElementalSpirit.Data
{
    public static class EquipmentCatalog
    {
        public static IReadOnlyList<Equipment> All { get; } = new List<Equipment>
        {
            new Accessory("acc_mana_ring", "Mana Ring", 80, 2, 10),
            new Accessory("acc_health_ring", "Health Ring", 90, 0, 25),
            new Accessory("acc_spirit_necklace", "Spirit Necklace", 180, 5, 20),
            new Accessory("acc_element_core", "Element Core", 400, 10, 40),
            new Accessory("acc_swift_boots", "Đôi Giày Tốc Hành", 60, 0, 12)
        };

        public static Equipment? Find(string id)
        {
            foreach (var e in All)
                if (e.Id == id) return e;
            return null;
        }
    }
}