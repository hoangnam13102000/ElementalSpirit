using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;
using ElementalSpirit.Presentation.Forms.Settings;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Presentation.Forms
{
    public class MainMenuForm : Form
    {
        private const int ButtonSpacingY = 85;
        private const int ButtonStartY = 300;

        private readonly Button _btnContinue;
        private readonly Button _btnStart;
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
            ClientSize = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(6, 8, 15);
            DoubleBuffered = true;

            try
            {
                _menuBackground = LoadMenuBackground();
            }
            catch { _menuBackground = null; }

            _btnContinue = CreateMenuButton();
            _btnContinue.Click += BtnContinue_Click;

            _btnStart = CreateMenuButton();
            _btnStart.Click += BtnStart_Click;

            _btnSettings = CreateMenuButton();
            _btnSettings.Click += BtnSettings_Click;

            _btnExit = CreateMenuButton();
            _btnExit.Click += (s, e) => Application.Exit();

            Controls.Add(_btnContinue);
            Controls.Add(_btnStart);
            Controls.Add(_btnSettings);
            Controls.Add(_btnExit);

            _localization.LanguageChanged += (s, e) => ApplyTranslations();

            ApplyTranslations();
            LayoutMenuButtons();
        }

        private void ApplyTranslations()
        {
            Text = _localization.Translate("menu.windowTitle");
            _btnContinue.Text = _localization.Translate("menu.continue");
            _btnStart.Text = _localization.Translate("menu.start");
            _btnSettings.Text = _localization.Translate("menu.settings");
            _btnExit.Text = _localization.Translate("menu.exit");
            Invalidate(); // vẽ lại tiêu đề game trong OnPaint
        }

        private void LayoutMenuButtons()
        {
            bool hasSave = _saveGameService.HasSavedGame;
            _btnContinue.Visible = hasSave;

            int y = ButtonStartY;
            if (hasSave)
            {
                _btnContinue.Top = y;
                y += ButtonSpacingY;
            }
            _btnStart.Top = y; y += ButtonSpacingY;
            _btnSettings.Top = y; y += ButtonSpacingY;
            _btnExit.Top = y;
        }

        private Button CreateMenuButton()
        {
            var btn = new Button
            {
                Bounds = new Rectangle(440, ButtonStartY, 400, 65),
                Font = new Font("Georgia", 18f, FontStyle.Bold),
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
            this.Hide();

            var introForm = new IntroForm(_localization);
            introForm.StartIntro();

            introForm.OnIntroFinished += () =>
            {
                var gameForm = new GameForm(Program.CreateGameManager(), _localization, _saveGameService);
                gameForm.ShowDialog();
                ReturnToMenu();
            };

            introForm.ShowDialog();
        }

        private void BtnContinue_Click(object? sender, EventArgs e)
        {
            var saveData = _saveGameService.Load();
            if (saveData == null)
            {
                MessageBox.Show(
                    this,
                    _localization.Translate("menu.continue.noSave"),
                    _localization.Translate("menu.windowTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            this.Hide();

            var gameManager = Program.CreateGameManager();
            gameManager.ApplySaveData(saveData);

            var gameForm = new GameForm(gameManager, _localization, _saveGameService);
            gameForm.ShowDialog();
            ReturnToMenu();
        }

        private void BtnSettings_Click(object? sender, EventArgs e)
        {
            using var settingsForm = new SettingsForm(_localization, _saveGameService);
            settingsForm.ShowDialog(this);
        }

        private void ReturnToMenu()
        {
            _menuBackground?.Dispose();
            _menuBackground = LoadMenuBackground();
            LayoutMenuButtons();
            this.Show();
            this.Activate();
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
                catch
                {
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