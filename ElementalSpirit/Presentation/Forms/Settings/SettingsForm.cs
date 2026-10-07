using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Presenters;
using ElementalSpirit.Presentation.Dialogs;
using ElementalSpirit.Services;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Presentation.Forms.Settings
{
    public partial class SettingsForm : Form, ISettingsView
    {
        private readonly ILocalizationService _localization = null!;
        private readonly Action<bool>? _onFullscreenChanged;
        private bool _isFullscreenOn;
        private bool _isInitializing;

        public SettingsForm()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _localization = LocalizationManager.Instance;
            _onFullscreenChanged = null;
            InitializeRuntime(new SaveGameService(), null, null, false);
        }

        public SettingsForm(
            ILocalizationService localization,
            ISaveGameService saveGameService,
            GameManager? activeGame = null,
            Action? onGameSaved = null,
            Action<bool>? onFullscreenChanged = null,
            bool initialIsFullscreen = false)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _onFullscreenChanged = onFullscreenChanged;
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            InitializeRuntime(saveGameService, activeGame, onGameSaved, initialIsFullscreen);
        }

        private void InitializeRuntime(
            ISaveGameService saveGameService,
            GameManager? activeGame,
            Action? onGameSaved,
            bool initialIsFullscreen)
        {
            bool showSaveGame = activeGame != null;
            int actionOffset = showSaveGame ? 0 : -50;
            ClientSize = new Size(420, showSaveGame ? 485 : 420);
            _isFullscreenOn = initialIsFullscreen;
            _isInitializing = true;
            _cmbLanguage.Items.Add(new LanguageItem(SupportedLanguage.Vietnamese, "Tiếng Việt"));
            _cmbLanguage.Items.Add(new LanguageItem(SupportedLanguage.English, "English"));
            _isInitializing = false;
            UpdateSoundToggleButton(_chkMusic, Services.AudioManager.Instance.MusicEnabled);

            _chkFullscreen.Text = _isFullscreenOn ? "🗗" : "⛶";
            UpdateSoundToggleButton(_chkSfx, Services.AudioManager.Instance.SfxEnabled);

            _lblSaveGame.Visible = showSaveGame;
            _btnSaveGame.Visible = showSaveGame;
            _btnSaveGame.Enabled = showSaveGame;
            _btnSaveGame.Location = new Point(180, 263);
            _btnSave.Location = new Point(70, 320 + actionOffset);
            _btnCancel.Location = new Point(220, 320 + actionOffset);
            _btnExitGame.BackColor = Color.FromArgb(120, 30, 30);
            _btnExitGame.Location = new Point((ClientSize.Width - _btnExitGame.Width) / 2, 370 + actionOffset);

            _ = new SettingsPresenter(
                this,
                _localization,
                saveGameService ?? throw new ArgumentNullException(nameof(saveGameService)),
                activeGame,
                onGameSaved);
        }

        private void LanguageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!_isInitializing && _cmbLanguage.SelectedItem is LanguageItem item)
                LanguageSelectionChanged?.Invoke(this, item.Language);
        }

        private void MusicButton_Click(object? sender, EventArgs e)
        {
            Services.AudioManager.Instance.MusicEnabled = !Services.AudioManager.Instance.MusicEnabled;
            UpdateSoundToggleButton(_chkMusic, Services.AudioManager.Instance.MusicEnabled);
        }

        private void FullscreenButton_Click(object? sender, EventArgs e)
        {
            _isFullscreenOn = !_isFullscreenOn;
            _chkFullscreen.Text = _isFullscreenOn ? "🗗" : "⛶";
            _onFullscreenChanged?.Invoke(_isFullscreenOn);
            ApplyFullscreenTranslation(_localization.Translate);
            _chkFullscreen.Focus();
            Application.DoEvents();
        }

        private void SfxButton_Click(object? sender, EventArgs e)
        {
            Services.AudioManager.Instance.SfxEnabled = !Services.AudioManager.Instance.SfxEnabled;
            UpdateSoundToggleButton(_chkSfx, Services.AudioManager.Instance.SfxEnabled);
        }

        private void SaveGameButton_Click(object? sender, EventArgs e)
        {
            SaveGameRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SaveGameButton_Paint(object? sender, PaintEventArgs e)
        {
            var button = (Button)sender!;
            Color textColor = button.Enabled ? Color.White : Color.FromArgb(220, 255, 255, 255);

            TextRenderer.DrawText(
                e.Graphics,
                button.Text,
                button.Font,
                button.ClientRectangle,
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            SaveRequested?.Invoke(this, EventArgs.Empty);
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ExitGameButton_Click(object? sender, EventArgs e)
        {
            if (Confirm(
                _localization.Translate("settings.exitgame.confirm.message"),
                _localization.Translate("settings.exitgame.confirm.title")))
            {
                RequestExitToMainMenu();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SupportedLanguage SelectedLanguage
        {
            get => _cmbLanguage.SelectedItem is LanguageItem item
                ? item.Language
                : SupportedLanguage.Vietnamese;
            set
            {
                for (int i = 0; i < _cmbLanguage.Items.Count; i++)
                {
                    if (_cmbLanguage.Items[i] is LanguageItem item && item.Language == value)
                    {
                        _cmbLanguage.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        public event EventHandler? SaveRequested;
        public event EventHandler? CancelRequested;
        public event EventHandler<SupportedLanguage>? LanguageSelectionChanged;
        public event EventHandler? SaveGameRequested;
        public event EventHandler? ExitGameRequested;

        public void ApplyTranslations(Func<string, string> translate)
        {
            Text = translate("settings.title");
            _lblTitle.Text = translate("settings.title");
            _lblLanguage.Text = translate("settings.language.label");
            _lblResolution.Text = translate("settings.resolution.label") + " 1280x720";
            _lblMusic.Text = translate("settings.music.label");
            _lblSfx.Text = translate("settings.sfx.label");
            ApplyFullscreenTranslation(translate);
            _lblSaveGame.Text = translate("settings.savegame.label");
            _btnSaveGame.Text = translate("settings.button.savegame");
            _btnSave.Text = translate("settings.button.save");
            _btnCancel.Text = translate("settings.button.cancel");
            _btnExitGame.Text = translate("settings.button.exitgame");
        }

        public void ShowInfo(string message, string title)
        {
            using var dialog = new InformationDialog(
                title,
                message,
                _localization.Translate("dialog.ok"));
            dialog.ShowDialog(this);
        }

        public bool Confirm(string message, string title)
        {
            using var dialog = new ConfirmationDialog(
                title,
                message,
                _localization.Translate("dialog.yes"),
                _localization.Translate("dialog.no"));
            return dialog.ShowDialog(this) == DialogResult.OK;
        }

        public void SetSaveGameAvailable(bool available)
        {
            _btnSaveGame.Visible = available;
            _lblSaveGame.Visible = available;
            _btnSaveGame.Enabled = available;
            _btnSaveGame.BackColor = available ? Color.FromArgb(90, 50, 110) : Color.FromArgb(50, 35, 65);
            _btnSaveGame.ForeColor = Color.White;
        }

        public bool ExitToMainMenuRequested { get; private set; }

        public void RequestExitToMainMenu()
        {
            ExitToMainMenuRequested = true;
            Close();
        }

        public void CloseView() => Close();

        private void ApplyFullscreenTranslation(Func<string, string> translate)
        {
            _lblFullscreen.Text = translate("settings.fullscreen.label") + " " +
                                  translate(_isFullscreenOn ? "settings.value.on" : "settings.value.off");
        }

        private static void UpdateSoundToggleButton(Button btn, bool isOn) { 
            btn.Text = isOn ? "🔊" : "🔇";
        }

        private sealed class LanguageItem
        {
            public SupportedLanguage Language { get; }
            private readonly string _display;

            public LanguageItem(SupportedLanguage language, string display)
            {
                Language = language;
                _display = display;
            }

            public override string ToString() => _display;
        }
    }
}