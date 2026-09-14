using System;
using System.Windows.Forms;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Forms.Settings;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Presentation.Presenters
{

    public sealed class SettingsPresenter
    {
        private readonly ISettingsView _view;
        private readonly ILocalizationService _localization;
        private readonly ISaveGameService _saveGameService;
        private readonly GameManager? _activeGame;

        public SettingsPresenter(
            ISettingsView view,
            ILocalizationService localization,
            ISaveGameService saveGameService,
            GameManager? activeGame = null)
        {
            _view = view;
            _localization = localization;
            _saveGameService = saveGameService;
            _activeGame = activeGame;

            _view.SelectedLanguage = _localization.CurrentLanguage;
            _view.ApplyTranslations(key => _localization.Translate(key));
            _view.SetSaveGameAvailable(_activeGame != null);

            _view.LanguageSelectionChanged += OnLanguageSelectionChanged;
            _view.SaveRequested += OnSaveRequested;
            _view.CancelRequested += OnCancelRequested;
            _view.SaveGameRequested += OnSaveGameRequested;
            _view.ExitGameRequested += OnExitGameRequested;
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

        private void OnSaveGameRequested(object? sender, EventArgs e)
        {
            if (_activeGame == null)
            {
                _view.ShowInfo(
                    _localization.Translate("settings.savegame.unavailable"),
                    _localization.Translate("settings.title"));
                return;
            }

            var snapshot = _activeGame.CaptureSaveData();
            _saveGameService.Save(snapshot);

            _view.ShowInfo(
                _localization.Translate("settings.savegame.saved.message"),
                _localization.Translate("settings.savegame.saved.title"));
        }

        private void OnExitGameRequested(object? sender, EventArgs e)
        {
            bool confirmed = _view.Confirm(
                _localization.Translate("settings.exitgame.confirm.message"),
                _localization.Translate("settings.exitgame.confirm.title"));

            if (!confirmed) return;

            if (_activeGame != null)
                _view.RequestExitToMainMenu();
            else
                Application.Exit();
        }
    }
}