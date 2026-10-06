using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;
using ElementalSpirit.Presentation.Forms.Settings;
using ElementalSpirit.Presentation.Dialogs;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Presentation.Forms
{
    public class MainMenuForm : Form
    {
        private readonly Button _btnContinue;
        private readonly Button _btnStart;
        private readonly Button _btnLeaderboard;
        private readonly Button _btnGuide;
        private readonly Button _btnSettings;
        private readonly Button _btnExit;
        private Image? _menuBackground;

        private readonly ILocalizationService _localization;
        private readonly ISaveGameService _saveGameService;

        private static Image? LoadMenuBackground()
        {
            var source = AssetLoader.Get("Menu/MainMenu_Background.png");
            return source == null ? null : new Bitmap(source);
        }

        public MainMenuForm(ILocalizationService localization, ISaveGameService saveGameService)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _saveGameService = saveGameService ?? throw new ArgumentNullException(nameof(saveGameService));
            AppIcon.ApplyTo(this);
            ClientSize = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(6, 8, 15);
            DoubleBuffered = true;

            try
            {
                _menuBackground = LoadMenuBackground();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainMenuForm] Menu background unavailable: {ex}");
                _menuBackground = null;
            }

            _btnContinue = CreateMenuButton();
            _btnContinue.Click += BtnContinue_Click;

            _btnStart = CreateMenuButton();
            _btnStart.Click += BtnStart_Click;

            _btnLeaderboard = CreateMenuButton();
            _btnLeaderboard.Click += BtnLeaderboard_Click;

            _btnGuide = CreateMenuButton();
            _btnGuide.Click += BtnGuide_Click;

            _btnSettings = CreateMenuButton();
            _btnSettings.Click += BtnSettings_Click;

            _btnExit = CreateMenuButton();
            _btnExit.Click += BtnExit_Click;

            Controls.Add(_btnContinue);
            Controls.Add(_btnStart);
            Controls.Add(_btnLeaderboard);
            Controls.Add(_btnGuide);
            Controls.Add(_btnSettings);
            Controls.Add(_btnExit);

            Resize += (_, _) =>
            {
                LayoutMenuButtons();
                Invalidate();
            };
            _localization.LanguageChanged += (s, e) => ApplyTranslations();

            ApplyTranslations();
            LayoutMenuButtons();
            Services.AudioManager.Instance.PlayMusic("menu_theme.wav", loop: true);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            WindowDisplayMode.SetFullscreen(this, Services.FullscreenPreferenceStore.Load());
        }

        private void ApplyTranslations()
        {
            Text = _localization.Translate("menu.windowTitle");
            _btnContinue.Text = _localization.Translate("menu.continue");
            _btnStart.Text = _localization.Translate("menu.start");
            _btnLeaderboard.Text = _localization.Translate("menu.leaderboard");
            _btnGuide.Text = _localization.Translate("menu.guide");
            _btnSettings.Text = _localization.Translate("menu.settings");
            _btnExit.Text = _localization.Translate("menu.exit");
            Invalidate(); // vẽ lại tiêu đề game trong OnPaint
        }

        private void LayoutMenuButtons()
        {
            float verticalScale = ClientSize.Height / 720f;
            bool hasSave = _saveGameService.HasSavedGame;

            _btnContinue.Visible = hasSave;
            _btnContinue.Enabled = hasSave;

            int buttonHeight = Math.Clamp((int)(58 * verticalScale), 42, 58);
            int spacing = buttonHeight + Math.Max(8, (int)(12 * verticalScale));
            int buttonWidth = Math.Min(400, ClientSize.Width - 48);
            int left = (ClientSize.Width - buttonWidth) / 2;
            var buttons = hasSave
                ? new[] { _btnContinue, _btnStart, _btnLeaderboard, _btnGuide, _btnSettings, _btnExit }
                : new[] { _btnStart, _btnLeaderboard, _btnGuide, _btnSettings, _btnExit };
            int totalHeight = buttons.Length * buttonHeight + (buttons.Length - 1) * (spacing - buttonHeight);
            int y = Math.Max((int)(120 * verticalScale), (ClientSize.Height - totalHeight) / 2 + (int)(18 * verticalScale));

            foreach (Button button in buttons)
            {
                button.SetBounds(left, y, buttonWidth, buttonHeight);
                button.Visible = true;
                y += spacing;
            }
        }

        private Button CreateMenuButton()
        {
            var btn = new Button
            {
                Bounds = new Rectangle(0, 0, 400, 58),
                Font = new Font("Georgia", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 220, 255),
                BackColor = Color.FromArgb(70, 40, 80, 150),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.FromArgb(150, 180, 220);
            return btn;
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            using var nameDialog = new PlayerNameDialog(_localization);
            if (nameDialog.ShowDialog(this) != DialogResult.OK)
                return;

            string playerName = nameDialog.PlayerName;
            var gameManager = Program.CreateGameManager();
            gameManager.SetPlayerName(playerName);

            _btnStart.Enabled = false;
            Services.AudioManager.Instance.StopMusic();
            this.Hide();

            using var introForm = new IntroForm(_localization, playerName);
            bool introCompleted = false;
            introForm.OnIntroFinished += () => introCompleted = true;
            introForm.Shown += (_, _) => introForm.StartIntro();
            introForm.ShowDialog();

            if (introCompleted)
            {
                using var gameForm = new GameForm(gameManager, _localization, _saveGameService);
                gameForm.ShowDialog();
            }
            else
            {
                gameManager.Dispose();
            }

            ReturnToMenu();
            _btnStart.Enabled = true;
        }

        private void BtnContinue_Click(object? sender, EventArgs e)
        {
            ElementalSpirit.Domain.SaveData.GameSaveData? saveData;
            try
            {
                saveData = _saveGameService.Load();
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException ||
                ex is InvalidDataException)
            {
                System.Diagnostics.Debug.WriteLine($"[MainMenuForm] Could not load saved game: {ex}");
                MessageBox.Show(
                    this,
                    _localization.Translate("menu.continue.loadError"),
                    _localization.Translate("menu.windowTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (saveData == null)
            {
                using var dialog = new InformationDialog(
                    _localization.Translate("menu.windowTitle"),
                    _localization.Translate("menu.continue.noSave"),
                    _localization.Translate("dialog.ok"));
                dialog.ShowDialog(this);
                return;
            }

            this.Hide();
            Services.AudioManager.Instance.StopMusic();

            var gameManager = Program.CreateGameManager();
            gameManager.ApplySaveData(saveData);

            var gameForm = new GameForm(gameManager, _localization, _saveGameService);
            gameForm.ShowDialog();
            ReturnToMenu();
        }

        private void BtnExit_Click(object? sender, EventArgs e)
        {
            using var dialog = new ConfirmationDialog(
                _localization.Translate("settings.exitgame.confirm.title"),
                _localization.Translate("settings.exitgame.confirm.message"),
                _localization.Translate("dialog.yes"),
                _localization.Translate("dialog.no"));
            if (dialog.ShowDialog(this) == DialogResult.OK)
                Application.Exit();
        }

        private void BtnSettings_Click(object? sender, EventArgs e)
        {
            using var settingsForm = new SettingsForm(
                _localization,
                _saveGameService,
                onFullscreenChanged: enabled => WindowDisplayMode.SetFullscreen(this, enabled));
            settingsForm.ShowDialog(this);
        }

        private void BtnLeaderboard_Click(object? sender, EventArgs e)
        {
            using var leaderboard = new LeaderboardForm(_localization);
            leaderboard.ShowDialog(this);
        }

        private void BtnGuide_Click(object? sender, EventArgs e)
        {
            using var guide = new GuideAndTeamForm(_localization);
            guide.ShowDialog(this);
        }

        private void ReturnToMenu()
        {
            WindowDisplayMode.SetFullscreen(this, Services.FullscreenPreferenceStore.Load());
            _menuBackground?.Dispose();
            _menuBackground = LoadMenuBackground();
            LayoutMenuButtons();
            this.Show();
            this.Activate();
            Services.AudioManager.Instance.PlayMusic("menu_theme.wav", loop: true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_menuBackground == null)
            {
                try
                {
                    _menuBackground = LoadMenuBackground();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainMenuForm] Menu background unavailable: {ex}");
                    _menuBackground = null;
                }
            }

            if (_menuBackground != null)
                g.DrawImage(_menuBackground, 0, 0, ClientSize.Width, ClientSize.Height);
            else
            {
                using var gradient = new LinearGradientBrush(
                    ClientRectangle,
                    Color.FromArgb(18, 12, 30),
                    Color.FromArgb(35, 22, 55),
                    135f);
                g.FillRectangle(gradient, ClientRectangle);
            }

            using var titleFont = new Font("Georgia", 52f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(245, 235, 255));

            string title = _localization.Translate("menu.gameTitle");
            var size = g.MeasureString(title, titleFont);
            float x = (ClientSize.Width - size.Width) / 2;
            float y = 70;

            g.DrawString(title, titleFont, titleBrush, x, y);
        }
    }
}