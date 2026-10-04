using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ElementalSpirit.Domain.Leaderboard;

namespace ElementalSpirit.Services
{
    public sealed class LeaderboardService
    {
        private readonly string _leaderboardFilePath;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        public LeaderboardService()
            : this(Path.Combine(AppContext.BaseDirectory, "Saves", "leaderboard.json"))
        {
        }

        public LeaderboardService(string leaderboardFilePath)
        {
            if (string.IsNullOrWhiteSpace(leaderboardFilePath))
                throw new ArgumentException("A leaderboard file path is required.", nameof(leaderboardFilePath));

            _leaderboardFilePath = Path.GetFullPath(leaderboardFilePath);
        }

        public IReadOnlyList<LeaderboardEntry> GetEntries()
        {
            if (!File.Exists(_leaderboardFilePath))
                return Array.Empty<LeaderboardEntry>();

            try
            {
                string json = File.ReadAllText(_leaderboardFilePath);
                List<LeaderboardEntry>? entries =
                    JsonSerializer.Deserialize<List<LeaderboardEntry>>(json, SerializerOptions);

                if (entries == null || entries.Any(entry =>
                        entry == null ||
                        string.IsNullOrWhiteSpace(entry.PlayerName) ||
                        entry.ClearedStageCount < 0 ||
                        entry.TotalGoldEarned < 0))
                {
                    throw new InvalidDataException("Leaderboard data contains invalid entries.");
                }

                return SortEntries(entries);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("Leaderboard file contains invalid JSON.", ex);
            }
        }

        public void RecordRun(Guid runId, string playerName, int clearedStageCount, int totalGoldEarned)
        {
            if (runId == Guid.Empty)
                throw new ArgumentException("A run ID is required.", nameof(runId));
            if (string.IsNullOrWhiteSpace(playerName))
                throw new ArgumentException("A player name is required.", nameof(playerName));
            if (clearedStageCount < 0)
                throw new ArgumentOutOfRangeException(nameof(clearedStageCount));
            if (totalGoldEarned < 0)
                throw new ArgumentOutOfRangeException(nameof(totalGoldEarned));

            var entries = GetEntries().ToList();
            entries.RemoveAll(entry => entry.RunId == runId);
            entries.Add(new LeaderboardEntry
            {
                RunId = runId,
                PlayerName = playerName.Trim(),
                ClearedStageCount = clearedStageCount,
                TotalGoldEarned = totalGoldEarned,
                RecordedAtUtc = DateTime.UtcNow
            });

            string? directory = Path.GetDirectoryName(_leaderboardFilePath);
            if (directory == null)
                throw new IOException("Could not determine the leaderboard directory.");

            Directory.CreateDirectory(directory);
            string json = JsonSerializer.Serialize(SortEntries(entries), SerializerOptions);
            File.WriteAllText(_leaderboardFilePath, json);
        }

        public void Clear()
        {
            if (File.Exists(_leaderboardFilePath))
                File.Delete(_leaderboardFilePath);
        }

        private static IReadOnlyList<LeaderboardEntry> SortEntries(IEnumerable<LeaderboardEntry> entries) =>
            entries
                .OrderByDescending(entry => entry.ClearedStageCount)
                .ThenByDescending(entry => entry.TotalGoldEarned)
                .ThenByDescending(entry => entry.RecordedAtUtc)
                .ToArray();
    }
}
