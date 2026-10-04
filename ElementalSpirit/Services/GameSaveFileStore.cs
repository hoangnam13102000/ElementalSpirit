using System;
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
            if (!File.Exists(SaveFilePath))
                return null;

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                GameSaveData? data = JsonSerializer.Deserialize<GameSaveData>(json, SerializerOptions);
                if (data == null ||
                    data.OwnedEquipmentIds == null ||
                    data.ClearedStages == null ||
                    data.Spirits == null)
                {
                    throw new InvalidDataException("Saved game file contains invalid data.");
                }

                return data;
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("Saved game file contains invalid JSON.", ex);
            }
        }

        public static void Save(GameSaveData data)
        {
            ArgumentNullException.ThrowIfNull(data);

            string? dir = Path.GetDirectoryName(SaveFilePath);
            if (dir == null)
                throw new IOException("Could not determine the save-game directory.");

            Directory.CreateDirectory(dir);
            string json = JsonSerializer.Serialize(data, SerializerOptions);
            File.WriteAllText(SaveFilePath, json);
        }

        public static void Delete()
        {
            if (File.Exists(SaveFilePath))
                File.Delete(SaveFilePath);
        }
    }
}