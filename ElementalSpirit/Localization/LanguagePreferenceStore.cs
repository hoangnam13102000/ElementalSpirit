using System;
using System.Diagnostics;
using System.IO;

namespace ElementalSpirit.Localization
{

    internal static class LanguagePreferenceStore
    {
        private static string PreferenceFilePath =>
            Path.Combine(AppContext.BaseDirectory, "Resources", "Lang", "language.pref");

        public static SupportedLanguage? LoadSavedLanguage()
        {
            try
            {
                if (!File.Exists(PreferenceFilePath))
                    return null;

                string content = File.ReadAllText(PreferenceFilePath).Trim().ToLowerInvariant();
                return content switch
                {
                    "en" => SupportedLanguage.English,
                    "vi" => SupportedLanguage.Vietnamese,
                    _ => null
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LanguagePreferenceStore] Load failed: {ex}");
                return null;
            }
        }

        public static void Save(SupportedLanguage language)
        {
            try
            {
                string? dir = Path.GetDirectoryName(PreferenceFilePath);
                if (dir != null)
                    Directory.CreateDirectory(dir);

                string code = language == SupportedLanguage.English ? "en" : "vi";
                File.WriteAllText(PreferenceFilePath, code);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LanguagePreferenceStore] Save failed: {ex}");
            }
        }
    }
}