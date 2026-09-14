using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using ElementalSpirit.Domain.SaveData;

namespace ElementalSpirit.Services
{
    internal static class GameSaveFileStore
    {
        private static string SaveFilePath =>
            Path.Combine(AppContext.BaseDirectory, "Saves", "savegame.json");

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        public static bool SaveFileExists => File.Exists(SaveFilePath);

        public static GameSaveData? Load()
        {
            try
            {
                if (!File.Exists(SaveFilePath))
                    return null;

                string json = File.ReadAllText(SaveFilePath);
                return JsonSerializer.Deserialize<GameSaveData>(json, SerializerOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameSaveFileStore] Load failed: {ex}");
                return null;
            }
        }

        public static void Save(GameSaveData data)
        {
            try
            {
                string? dir = Path.GetDirectoryName(SaveFilePath);
                if (dir != null)
                    Directory.CreateDirectory(dir);

                string json = JsonSerializer.Serialize(data, SerializerOptions);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameSaveFileStore] Save failed: {ex}");
            }
        }

        public static void Delete()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                    File.Delete(SaveFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GameSaveFileStore] Delete failed: {ex}");
            }
        }
    }
}