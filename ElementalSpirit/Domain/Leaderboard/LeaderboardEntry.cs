using System;

namespace ElementalSpirit.Domain.Leaderboard
{
    public sealed class LeaderboardEntry
    {
        public Guid RunId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int ClearedStageCount { get; set; }
        public int TotalGoldEarned { get; set; }
        public DateTime RecordedAtUtc { get; set; }
    }
}
