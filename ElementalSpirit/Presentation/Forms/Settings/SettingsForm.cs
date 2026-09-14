using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Presenters;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Presentation.Forms.Settings
{
    public class SettingsForm : Form, ISettingsView
    {
        private readonly Label _lblTitle;
        private readonly Label _lblLanguage;
        private readonly ComboBox _cmbLanguage;
        private readonly Label _lblResolution;
        private readonly Label _lblSound;
        private readonly Label _lblFullscreen;
        private readonly Label _lblSaveGame;
        private readonly Button _btnSaveGame;
        private readonly Button _btnSave;
        private readonly Button _btnCancel;
        private readonly Button _btnExitGame;

        public SettingsForm(
            ILocalizationService localization,
            ISaveGameService saveGameService,
            GameManager? activeGame = null)
        {
            ClientSize = new Size(420, 450);
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
            _lblSound = CreateLabel(155);
            _lblFullscreen = CreateLabel(190);

            _lblSaveGame = CreateLabel(232);

            _btnSaveGame = CreateButton(180, 228, 200);
            _btnSaveGame.Click += (s, e) => SaveGameRequested?.Invoke(this, EventArgs.Empty);

            _btnSave = CreateButton(100, 310);
            _btnSave.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);

            _btnCancel = CreateButton(220, 310);
            _btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);

            _btnExitGame = CreateButton(100, 366, 220);
            _btnExitGame.BackColor = Color.FromArgb(120, 30, 30);
            _btnExitGame.Click += (s, e) => ExitGameRequested?.Invoke(this, EventArgs.Empty);

            Controls.AddRange(new Control[]
            {
                _lblTitle, _lblLanguage, _cmbLanguage,
                _lblResolution, _lblSound, _lblFullscreen,
                _lblSaveGame, _btnSaveGame,
                _btnSave, _btnCancel, _btnExitGame
            });

            _ = new SettingsPresenter(
                this,
                localization ?? throw new ArgumentNullException(nameof(localization)),
                saveGameService ?? throw new ArgumentNullException(nameof(saveGameService)),
                activeGame);
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
            _lblSound.Text = translate("settings.sound.label") + " " + translate("settings.value.on");
            _lblFullscreen.Text = translate("settings.fullscreen.label") + " " + translate("settings.value.off");
            _lblSaveGame.Text = translate("settings.savegame.label");
            _btnSaveGame.Text = translate("settings.button.savegame");
            _btnSave.Text = translate("settings.button.save");
            _btnCancel.Text = translate("settings.button.cancel");
            _btnExitGame.Text = translate("settings.button.exitgame");
        }

        public void ShowInfo(string message, string title) =>
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public bool Confirm(string message, string title) =>
            MessageBox.Show(this, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes;

        public void SetSaveGameAvailable(bool available)
        {
            _btnSaveGame.Enabled = available;
        }

        public bool ExitToMainMenuRequested { get; private set; }

        public void RequestExitToMainMenu()
        {
            ExitToMainMenuRequested = true;
            Close();
        }

        public void CloseView() => Close();

        private static Label CreateLabel(int y, Font? font = null) => new()
        {
            Location = new Point(24, y),
            AutoSize = true,
            ForeColor = Color.FromArgb(230, 220, 255),
            Font = font ?? new Font("Segoe UI", 11f)
        };

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