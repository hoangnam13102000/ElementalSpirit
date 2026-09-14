using System;
using System.Collections.Generic;

namespace ElementalSpirit.Domain.SaveData
{
    public class SpiritSaveData
    {
        public string Id { get; set; } = string.Empty;
        public int Level { get; set; }
    }

    public class GameSaveData
    {
        public int StageIndex { get; set; }
        public string StageName { get; set; } = string.Empty;
        public int PlayerHp { get; set; }
        public int Gold { get; set; }
        public int SpiritShards { get; set; }
        public int Crystals { get; set; }
        public List<string> OwnedEquipmentIds { get; set; } = new();
        public string? EquippedWeaponId { get; set; }
        public string? EquippedArmorId { get; set; }
        public string? EquippedAccessoryId { get; set; }
        public List<SpiritSaveData> Spirits { get; set; } = new();
        public string? EquippedSpiritSlot1Id { get; set; }
        public string? EquippedSpiritSlot2Id { get; set; }
        public DateTime SavedAtUtc { get; set; }
    }
}