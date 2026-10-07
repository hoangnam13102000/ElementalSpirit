using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;
using ElementalSpirit.Presentation.Forms.Settings;
using ElementalSpirit.Presentation.Dialogs;
using ElementalSpirit.Services;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Presentation.Forms
{
    public partial class MainMenuForm : Form
    {
        private Button _btnContinue = null!;
        private Button _btnStart = null!;
        private Button _btnLeaderboard = null!;
        private Button _btnGuide = null!;
        private Button _btnSettings = null!;
        private Button _btnExit = null!;
        private Image? _menuBackground;
        private Font? _responsiveTitleFont;
        private IntroForm? _activeIntro;
        private GameForm? _activeGame;

        private readonly ILocalizationService _localization = null!;
        private readonly ISaveGameService _saveGameService = null!;

        public MainMenuForm()
        {
            _localization = null!;
            _saveGameService = null!;
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _localization = LocalizationManager.Instance;
            _saveGameService = new SaveGameService();
            InitializeRuntime();
        }

        public MainMenuForm(ILocalizationService localization, ISaveGameService saveGameService)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _saveGameService = saveGameService ?? throw new ArgumentNullException(nameof(saveGameService));
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            InitializeRuntime();
        }

        private void InitializeRuntime()
        {
            AppIcon.ApplyTo(this);

            try
            {
                _menuBackground = AssetLoader.Get("Menu/MainMenu_Background.png");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainMenuForm] Menu background unavailable: {ex}");
                _menuBackground = null;
            }

            _localization.LanguageChanged += OnLanguageChanged;

            ApplyTranslations();
            LayoutMenuButtons();
            if (System.ComponentModel.LicenseManager.UsageMode !=
                System.ComponentModel.LicenseUsageMode.Designtime)
                Services.AudioManager.Instance.PlayMusic("menu_theme.wav", loop: true);
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            ApplyTranslations();
            LayoutMenuButtons();
        }

        private void MainMenuForm_Resize(object? sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                _saveGameService is null)
                return;

            LayoutMenuButtons();
            Invalidate();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

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
            _titleLabel.Text = _localization.Translate("menu.gameTitle");
        }

        private void LayoutMenuButtons()
        {
            float verticalScale = ClientSize.Height / 720f;
            bool hasSave = _saveGameService.HasSavedGame;
            int titleTop = (int)(70 * verticalScale);
            int titleHeight = Math.Max(44, (int)(80 * verticalScale));
            _titleLabel.SetBounds(
                0,
                titleTop,
                ClientSize.Width,
                titleHeight);
            FitTitleFont(titleHeight);

            _btnContinue.Visible = hasSave;
            _btnContinue.Enabled = hasSave;

            int buttonHeight = Math.Clamp((int)(58 * verticalScale), 42, 58);
            int spacing = buttonHeight + Math.Max(8, (int)(12 * verticalScale));
            int buttonWidth = Math.Max(1, Math.Min(400, ClientSize.Width - 48));
            int left = (ClientSize.Width - buttonWidth) / 2;
            var buttons = hasSave
                ? new[] { _btnContinue, _btnStart, _btnLeaderboard, _btnGuide, _btnSettings, _btnExit }
                : new[] { _btnStart, _btnLeaderboard, _btnGuide, _btnSettings, _btnExit };
            int totalHeight = buttons.Length * buttonHeight + (buttons.Length - 1) * (spacing - buttonHeight);
            int minButtonTop = titleTop + titleHeight + (int)(12 * verticalScale);
            int y = Math.Max(
                minButtonTop,
                (ClientSize.Height - totalHeight) / 2 + (int)(18 * verticalScale));

            foreach (Button button in buttons)
            {
                button.SetBounds(left, y, buttonWidth, buttonHeight);
                button.Visible = true;
                y += spacing;
            }
        }

        private void FitTitleFont(int titleHeight)
        {
            const float minimumFontSize = 14f;
            float low = minimumFontSize;
            float high = 52f;
            float fittedSize = low;
            int availableWidth = Math.Max(1, ClientSize.Width - 48);
            int availableHeight = Math.Max(1, titleHeight - 6);
            string title = _titleLabel.Text;

            for (int i = 0; i < 12; i++)
            {
                float candidateSize = (low + high) / 2f;
                using var candidateFont = new Font("Georgia", candidateSize, FontStyle.Bold);
                Size measured = TextRenderer.MeasureText(
                    title,
                    candidateFont,
                    Size.Empty,
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

                if (measured.Width <= availableWidth && measured.Height <= availableHeight)
                {
                    fittedSize = candidateSize;
                    low = candidateSize;
                }
                else
                {
                    high = candidateSize;
                }
            }

            if (_responsiveTitleFont is not null &&
                Math.Abs(_responsiveTitleFont.Size - fittedSize) < 0.5f)
                return;

            _responsiveTitleFont?.Dispose();
            _responsiveTitleFont = new Font("Georgia", fittedSize, FontStyle.Bold);
            _titleLabel.Font = _responsiveTitleFont;
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
            SetMenuButtonsVisible(false);

            var introForm = new IntroForm(_localization, playerName);
            _activeIntro = introForm;
            introForm.OnIntroFinished += () =>
            {
                if (!ReferenceEquals(_activeIntro, introForm))
                    return;

                 _activeIntro = null;
                 OpenGame(gameManager);
       
            };
            introForm.FormClosed += (_, _) =>
            {
                this.Hide();
                if (ReferenceEquals(_activeIntro, introForm))
                {
                    _activeIntro = null;
                    gameManager.Dispose();
                    ReturnToMenu();
                }
            };
            introForm.Shown += (_, _) => introForm.StartIntro();
            introForm.ShowDialog();

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

            Services.AudioManager.Instance.StopMusic();

            var gameManager = Program.CreateGameManager();
            gameManager.ApplySaveData(saveData);
            OpenGame(gameManager);
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
            _menuBackground = AssetLoader.Get("Menu/MainMenu_Background.png");
            LayoutMenuButtons();
            _btnStart.Enabled = true;
            this.Show();
            this.BringToFront();
            this.Activate();
            Services.AudioManager.Instance.PlayMusic("menu_theme.wav", loop: true);
        }

        private void OpenGame(ElementalSpirit.GameEngine.GameManager gameManager)
        {
            SetMenuButtonsVisible(false);
            this.Hide();
            var gameForm = new GameForm(gameManager, _localization, _saveGameService);
            _activeGame = gameForm;
            gameForm.FormClosed += (_, _) =>
            {
                Controls.Remove(gameForm);
                if (ReferenceEquals(_activeGame, gameForm))
                {
                    _activeGame = null;
                    ReturnToMenu();
                }
            };
            gameForm.ShowDialog();
        }

        private void ShowEmbeddedForm(Form childForm)
        {
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            Controls.Add(childForm);
            childForm.BringToFront();
            childForm.Show();
            childForm.Focus();
        }

        private void SetMenuButtonsVisible(bool visible)
        {
            _btnContinue.Visible = visible && _saveGameService.HasSavedGame;
            _btnStart.Visible = visible;
            _btnLeaderboard.Visible = visible;
            _btnGuide.Visible = visible;
            _btnSettings.Visible = visible;
            _btnExit.Visible = visible;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_menuBackground != null)
                g.DrawImage(_menuBackground, 0, 0, ClientSize.Width, ClientSize.Height);
            else if (BackgroundImage == null)
            {
                using var gradient = new LinearGradientBrush(
                    ClientRectangle,
                    Color.FromArgb(18, 12, 30),
                    Color.FromArgb(35, 22, 55),
                    135f);
                g.FillRectangle(gradient, ClientRectangle);
            }
        }
    }
}