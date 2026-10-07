using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Presenters;
using ElementalSpirit.Presentation.Dialogs;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Presentation.Forms.Settings
{
    public class SettingsForm : Form, ISettingsView
    {
        private readonly ILocalizationService _localization;
        private readonly Label _lblTitle;
        private readonly Label _lblLanguage;
        private readonly ComboBox _cmbLanguage;
        private readonly Label _lblResolution;
        private readonly Label _lblMusic;
        private readonly Button _chkMusic;           
        private readonly Label _lblSfx;              
        private readonly Button _chkSfx;
        private readonly Label _lblFullscreen;
        private readonly Button _chkFullscreen;
        private readonly Action<bool>? _onFullscreenChanged;
        private bool _isFullscreenOn;
        private readonly Label _lblSaveGame;
        private readonly Button _btnSaveGame;
        private readonly Button _btnSave;
        private readonly Button _btnCancel;
        private readonly Button _btnExitGame;

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
            bool showSaveGame = activeGame != null;
            int actionOffset = showSaveGame ? 0 : -50;
            ClientSize = new Size(420, showSaveGame ? 485 : 420);
            _isFullscreenOn = initialIsFullscreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(24, 18, 36);

            _lblTitle = CreateLabel(20, new Font("Georgia", 16f, FontStyle.Bold));
            _lblLanguage = CreateLabel(70);

            _cmbLanguage = new ComboBox
            {
                Location = new Point(180, 67),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbLanguage.Items.Add(new LanguageItem(SupportedLanguage.Vietnamese, "Tiếng Việt"));
            _cmbLanguage.Items.Add(new LanguageItem(SupportedLanguage.English, "English"));
            _cmbLanguage.SelectedIndexChanged += (s, e) =>
            {
                if (_cmbLanguage.SelectedItem is LanguageItem item)
                    LanguageSelectionChanged?.Invoke(this, item.Language);
            };

            _lblResolution = CreateLabel(120);

            _chkMusic = new Button 
            { 
                Location = new Point(230, 145),
                Size = new Size(36, 30), 
                FlatStyle = FlatStyle.Flat, 
                Font = new Font("Segoe UI Emoji", 10f), 
                ForeColor = Color.White, Cursor = Cursors.Hand 
            };
            _chkMusic.FlatAppearance.BorderSize = 0;
            UpdateSoundToggleButton(_chkMusic, Services.AudioManager.Instance.MusicEnabled); 
            _chkMusic.Click += (s, e) => 
            { 
                Services.AudioManager.Instance.MusicEnabled = !Services.AudioManager.Instance.MusicEnabled; 
                UpdateSoundToggleButton(_chkMusic, Services.AudioManager.Instance.MusicEnabled); 
            };

            _lblMusic = CreateLabel(150);
            _lblFullscreen = CreateLabel(220);
            _lblSfx = CreateLabel(185);

            _chkFullscreen = new Button 
            { 
                Location = new Point(230, 215), 
                Size = new Size(36, 30),
                FlatStyle = FlatStyle.Flat, 
                Font = new Font("Segoe UI Emoji", 10f),
                ForeColor = Color.White, Cursor = Cursors.Hand,
                Text = _isFullscreenOn ? "🗗" : "⛶"
            }; 
            _chkFullscreen.FlatAppearance.BorderSize = 0; 
            _chkFullscreen.Click += (s, e) => 
            {
                _isFullscreenOn = !_isFullscreenOn; 
                _chkFullscreen.Text = _isFullscreenOn ? "🗗" : "⛶"; 
                _onFullscreenChanged?.Invoke(_isFullscreenOn);
                ApplyFullscreenTranslation(key => 
                _localization.Translate(key));
                _chkFullscreen.Focus();
                Application.DoEvents();
            };
            _chkSfx = new Button 
            { 
                Location = new Point(230, 180),
                Size = new Size(36, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Emoji", 10f), 
                ForeColor = Color.White, Cursor = Cursors.Hand
            };
            _chkSfx.FlatAppearance.BorderSize = 0;
            UpdateSoundToggleButton(_chkSfx, Services.AudioManager.Instance.SfxEnabled);
            _chkSfx.Click += (s, e) =>
            { 
                Services.AudioManager.Instance.SfxEnabled = !Services.AudioManager.Instance.SfxEnabled; 
                UpdateSoundToggleButton(_chkSfx, Services.AudioManager.Instance.SfxEnabled);
            };

            _lblSaveGame = CreateLabel(267);
            _lblSaveGame.Visible = showSaveGame;

            _btnSaveGame = CreateButton(180, 263, 200);
            _btnSaveGame.Visible = showSaveGame;
            _btnSaveGame.Click += (s, e) => SaveGameRequested?.Invoke(this, EventArgs.Empty);
            _btnSaveGame.Paint += (s, e) =>
            {
                var btn = (Button)s!;
                // Nếu nút bị Disabled thì vẽ chữ màu trắng mờ hơn nhẹ hoặc trắng tinh tùy thích
                Color textColor = btn.Enabled ? Color.White : Color.FromArgb(220, 255, 255, 255);

                TextRenderer.DrawText(
                    e.Graphics,
                    btn.Text,
                    btn.Font,
                    btn.ClientRectangle,
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            _btnSave = CreateButton(70, 320 + actionOffset,120);
            _btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);

            _btnCancel = CreateButton(220, 320 + actionOffset,120);
            _btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);

            _btnExitGame = CreateButton(100, 370 + actionOffset, 270);
            _btnExitGame.BackColor = Color.FromArgb(120, 30, 30);
            _btnExitGame.Click += (s, e) =>
            {
                
                if (Confirm(_localization.Translate("settings.exitgame.confirm.message"),
                            _localization.Translate("settings.exitgame.confirm.title")))
                {
                    RequestExitToMainMenu(); 
                }
            };

            Controls.AddRange(new Control[]
            {
                _lblTitle, _lblLanguage, _cmbLanguage,
                 _lblResolution, _lblMusic, _chkMusic, _lblSfx, _chkSfx, _lblFullscreen, _chkFullscreen,
                _lblSaveGame, _btnSaveGame,
                _btnSave, _btnCancel, _btnExitGame
            });

            _ = new SettingsPresenter(
                this,
                _localization,
                saveGameService ?? throw new ArgumentNullException(nameof(saveGameService)),
                activeGame,
                onGameSaved);
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

        private static Label CreateLabel(int y, Font? font = null) => new()
        {
            Location = new Point(24, y),
            AutoSize = true,
            ForeColor = Color.FromArgb(230, 220, 255),
            Font = font ?? new Font("Segoe UI", 11f)
        };

        private static void UpdateSoundToggleButton(Button btn, bool isOn) { 
            btn.Text = isOn ? "🔊" : "🔇";
        }

        private static Button CreateButton(int x, int y, int width = 100) => new()
        {
            Bounds = new Rectangle(x, y, width, 36),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(70, 40, 80),
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };

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