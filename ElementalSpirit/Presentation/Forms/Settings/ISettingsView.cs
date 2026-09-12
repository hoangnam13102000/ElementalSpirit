using System;
using ElementalSpirit.Localization;

namespace ElementalSpirit.Presentation.Forms.Settings
{

    public interface ISettingsView
    {
        SupportedLanguage SelectedLanguage { get; set; }

        event EventHandler? SaveRequested;
        event EventHandler? CancelRequested;
        event EventHandler<SupportedLanguage>? LanguageSelectionChanged;

        void ApplyTranslations(Func<string, string> translate);
        void ShowInfo(string message, string title);
        void CloseView();
    }
}