using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;
using ElementalSpirit.Presentation.Forms.Settings;

namespace ElementalSpirit.Presentation.Forms
{
    public class MainMenuForm : Form
    {
        private readonly Button _btnStart;
        private readonly Button _btnSettings;
        private readonly Button _btnExit;
        private Image? _menuBackground;

        private readonly ILocalizationService _localization = LocalizationManager.Instance;

        public MainMenuForm()
        {
            ClientSize = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(6, 8, 15);
            DoubleBuffered = true;

            try
            {
                _menuBackground = AssetLoader.Get("Menu/MainMenu_Background.png");
            }
            catch { _menuBackground = null; }

            _btnStart = CreateMenuButton(300);
            _btnStart.Click += BtnStart_Click;

            _btnSettings = CreateMenuButton(385);
            _btnSettings.Click += BtnSettings_Click;

            _btnExit = CreateMenuButton(470);
            _btnExit.Click += (s, e) => Application.Exit();

            Controls.Add(_btnStart);
            Controls.Add(_btnSettings);
            Controls.Add(_btnExit);

            // Khi ngôn ngữ đổi ở màn Settings (kể cả lần sau mở lại từ nơi khác),
            // MainMenuForm tự cập nhật lại chữ trên UI.
            _localization.LanguageChanged += (s, e) => ApplyTranslations();
            ApplyTranslations();
        }

        private void ApplyTranslations()
        {
            Text = _localization.Translate("menu.windowTitle");
            _btnStart.Text = _localization.Translate("menu.start");
            _btnSettings.Text = _localization.Translate("menu.settings");
            _btnExit.Text = _localization.Translate("menu.exit");
            Invalidate(); // vẽ lại tiêu đề game trong OnPaint
        }

        private Button CreateMenuButton(int y)
        {
            var btn = new Button
            {
                Bounds = new Rectangle(440, y, 400, 65),
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

            var introForm = new IntroForm();
            introForm.StartIntro();

            introForm.OnIntroFinished += () =>
            {
                var gameForm = new GameForm(Program.CreateGameManager());
                gameForm.ShowDialog();
                this.Close();
            };

            introForm.ShowDialog();
        }

        private void BtnSettings_Click(object? sender, EventArgs e)
        {
            using var settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

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