using System;
using System.Diagnostics;
using System.IO;

namespace ElementalSpirit.Services
{
    internal static class FullscreenPreferenceStore
    {
        private static string PreferenceFilePath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ElementalSpirit",
                "fullscreen.pref");

        public static bool Load()
        {
            try
            {
                return File.Exists(PreferenceFilePath) &&
                       string.Equals(
                           File.ReadAllText(PreferenceFilePath).Trim(),
                           "true",
                           StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException)
            {
                Debug.WriteLine($"[FullscreenPreferenceStore] Load failed: {ex}");
                return false;
            }
        }

        public static void Save(bool enabled)
        {
            try
            {
                string? directory = Path.GetDirectoryName(PreferenceFilePath);
                if (directory != null)
                    Directory.CreateDirectory(directory);

                File.WriteAllText(PreferenceFilePath, enabled ? "true" : "false");
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException)
            {
                Debug.WriteLine($"[FullscreenPreferenceStore] Save failed: {ex}");
            }
        }
    }
}
