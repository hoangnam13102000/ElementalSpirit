using System.Collections.Generic;
using ElementalSpirit.Domain.Equipment;

namespace ElementalSpirit.Data
{
    public static class EquipmentCatalog
    {
        public static IReadOnlyList<Equipment> All { get; } = new List<Equipment>
        {
            new Weapon("wpn_basic_wand", "Basic Wand", 50, 3),
            new Weapon("wpn_crystal_wand", "Crystal Wand", 120, 7),
            new Weapon("wpn_fire_staff", "Fire Staff", 220, 12),
            new Weapon("wpn_storm_staff", "Storm Staff", 350, 18),
            new Weapon("wpn_ancient_staff", "Ancient Staff", 600, 28),

            new Armor("arm_cloth", "Cloth Armor", 40, 15, 1),
            new Armor("arm_leather", "Leather Armor", 100, 30, 3),
            new Armor("arm_elemental_robe", "Elemental Robe", 200, 50, 5),
            new Armor("arm_guardian", "Guardian Armor", 320, 80, 8),
            new Armor("arm_ancient_robe", "Ancient Robe", 500, 120, 12),

            new Accessory("acc_mana_ring", "Mana Ring", 80, 2, 10),
            new Accessory("acc_health_ring", "Health Ring", 90, 0, 25),
            new Accessory("acc_spirit_necklace", "Spirit Necklace", 180, 5, 20),
            new Accessory("acc_element_core", "Element Core", 400, 10, 40)
        };

        public static Equipment? Find(string id)
        {
            foreach (var e in All)
                if (e.Id == id) return e;
            return null;
        }
    }
}