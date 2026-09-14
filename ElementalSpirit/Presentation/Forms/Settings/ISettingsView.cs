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
        event EventHandler? SaveGameRequested;
        event EventHandler? ExitGameRequested;

        void ApplyTranslations(Func<string, string> translate);
        void ShowInfo(string message, string title);

        bool Confirm(string message, string title);

        void SetSaveGameAvailable(bool available);

        void RequestExitToMainMenu();

        void CloseView();
    }
}