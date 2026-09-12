using System;
using System.Collections.Generic;
using System.IO;

namespace ElementalSpirit.Localization
{

    public sealed class LocalizationManager : ILocalizationService
    {
        private const string BaseBundleName = "messages";

        private static readonly Lazy<LocalizationManager> _instance =
            new(() => new LocalizationManager());

        public static LocalizationManager Instance => _instance.Value;

        private readonly Dictionary<SupportedLanguage, IResourceBundle> _bundleCache = new();

        public SupportedLanguage CurrentLanguage { get; private set; }

        public event EventHandler? LanguageChanged;

        private LocalizationManager()
        {
            CurrentLanguage = LanguagePreferenceStore.LoadSavedLanguage() ?? SupportedLanguage.Vietnamese;
        }

        public void SetLanguage(SupportedLanguage language)
        {
            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;
            LanguagePreferenceStore.Save(language);
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public string Translate(string key) => Translate(key, CurrentLanguage);

        public string Translate(string key, SupportedLanguage language)
        {
            var bundle = GetBundle(language);
            return bundle.GetString(key);
        }

        private IResourceBundle GetBundle(SupportedLanguage language)
        {
            if (_bundleCache.TryGetValue(language, out var cached))
                return cached;

            string suffix = language == SupportedLanguage.Vietnamese ? "vi" : "en";
            string fileName = $"{BaseBundleName}_{suffix}.properties";
            string path = Path.Combine(LangFolder, fileName);

            var bundle = new PropertiesResourceBundle(path);
            _bundleCache[language] = bundle;
            return bundle;
        }

        private static string LangFolder =>
            Path.Combine(AppContext.BaseDirectory, "Resources", "Lang");
    }
}