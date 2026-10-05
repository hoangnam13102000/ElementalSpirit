using System;
using System.Collections.Generic;

namespace ElementalSpirit.Domain.SaveData
{
    public class GameSaveData
    {
        public Guid RunId { get; set; }
        public string PlayerName { get; set; } = "Arin";
        public int StageIndex { get; set; }
        public string StageName { get; set; } = string.Empty;
        public int PlayerHp { get; set; }
        public int Gold { get; set; }
        public int TotalGoldEarned { get; set; }
        public List<int> ClearedStages { get; set; } = new();
        public List<string> UnlockedSkillIds { get; set; } = new();
        public bool BossEncounterStarted { get; set; }
        public bool BossEncounterCompleted { get; set; }
        public int BossHealth { get; set; }
        public bool HasDoubleJumpBoots { get; set; }
        public DateTime SavedAtUtc { get; set; }
        public int RemainingEnemyCount { get; set; } = -1; 
    }
}