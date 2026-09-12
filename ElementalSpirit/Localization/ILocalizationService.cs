using System;

namespace ElementalSpirit.Localization
{

    public interface ILocalizationService
    {
        SupportedLanguage CurrentLanguage { get; }

        event EventHandler? LanguageChanged;

        void SetLanguage(SupportedLanguage language);

        string Translate(string key);

        string Translate(string key, SupportedLanguage language);
    }
}