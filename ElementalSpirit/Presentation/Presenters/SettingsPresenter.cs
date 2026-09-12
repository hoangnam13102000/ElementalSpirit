using System;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Forms.Settings;

namespace ElementalSpirit.Presentation.Presenters
{

    public sealed class SettingsPresenter
    {
        private readonly ISettingsView _view;
        private readonly ILocalizationService _localization;

        public SettingsPresenter(ISettingsView view, ILocalizationService localization)
        {
            _view = view;
            _localization = localization;

            _view.SelectedLanguage = _localization.CurrentLanguage;
            _view.ApplyTranslations(key => _localization.Translate(key));

            _view.LanguageSelectionChanged += OnLanguageSelectionChanged;
            _view.SaveRequested += OnSaveRequested;
            _view.CancelRequested += OnCancelRequested;
        }

        private void OnLanguageSelectionChanged(object? sender, SupportedLanguage previewLanguage)
        {

            _view.ApplyTranslations(key => _localization.Translate(key, previewLanguage));
        }

        private void OnSaveRequested(object? sender, EventArgs e)
        {
            _localization.SetLanguage(_view.SelectedLanguage);
            _view.ApplyTranslations(key => _localization.Translate(key));
            _view.ShowInfo(
                _localization.Translate("settings.saved.message"),
                _localization.Translate("settings.saved.title"));
            _view.CloseView();
        }

        private void OnCancelRequested(object? sender, EventArgs e)
        {

            _view.CloseView();
        }
    }
}