using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using ElementalSpirit.Presentation.Assets;
using ElementalSpirit.Presentation.Intro;

namespace ElementalSpirit.Presentation.Forms
{
    public class IntroForm : Form
    {
        private readonly IntroManager _introManager;
        private Image? _currentBackground;
        private string _currentSpeakerName = "";
        private string _currentText = "";
        private string _currentCaption = "";
        private Speaker _currentSpeaker;

        private float _fadeAlpha = 0f;
        private bool _isFadingIn;
        private readonly Timer _fadeTimer;

        public event Action? OnIntroFinished;

        public IntroForm()
        {
            _introManager = new IntroManager();
            _introManager.OnSceneChanged += HandleSceneChanged;
            _introManager.OnLineChanged += HandleLineChanged;
            _introManager.OnIntroCompleted += HandleIntroCompleted;

            Text = "Elemental Spirit — Intro";
            ClientSize = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(8, 10, 18);
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            _fadeTimer = new Timer { Interval = 30 };
            _fadeTimer.Tick += OnFadeTick;

            KeyDown += OnKeyPressed;
            MouseClick += OnMouseClicked;
        }

        public void StartIntro()
        {
            _introManager.Start();
            BeginFadeIn();
        }

        private void HandleSceneChanged(DialogueScene scene)
        {
            if (!string.IsNullOrEmpty(scene.BackgroundImageName))
            {
                try
                {
                    _currentBackground?.Dispose();

                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string fullPath = Path.Combine(baseDir, "Resources", "Images", scene.BackgroundImageName);

                    if (File.Exists(fullPath))
                    {
                        _currentBackground = Image.FromFile(fullPath);
                    }
                    else
                    {
                        string altPath = Path.Combine(baseDir, scene.BackgroundImageName);
                        if (File.Exists(altPath))
                            _currentBackground = Image.FromFile(altPath);
                        else
                            _currentBackground = AssetLoader.Get(scene.BackgroundImageName);
                    }
                }
                catch
                {
                    _currentBackground = null;
                }
            }
            BeginFadeIn();
        }

        private void HandleLineChanged(DialogueLine line)
        {
            _currentSpeaker = line.Speaker;
            _currentSpeakerName = GetSpeakerDisplayName(line.Speaker);
            _currentText = line.Text;
            _currentCaption = line.OnScreenCaption ?? "";
            Invalidate();
        }

        private void HandleIntroCompleted()
        {
            _fadeTimer.Stop();
            _currentBackground?.Dispose();

            OnIntroFinished?.Invoke();
            Close();
        }

        private void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { _introManager.SkipAll(); return; }
            if (e.KeyCode == Keys.Tab) { _introManager.SkipScene(); return; }
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                _introManager.NextLine();
            }
        }

        private void OnMouseClicked(object? sender, MouseEventArgs e)
        {
            _introManager.NextLine();
        }

        private void BeginFadeIn()
        {
            _fadeAlpha = 0f;
            _isFadingIn = true;
            _fadeTimer.Start();
        }

        private void OnFadeTick(object? sender, EventArgs e)
        {
            if (_isFadingIn)
            {
                _fadeAlpha += 0.04f;
                if (_fadeAlpha >= 1f)
                {
                    _fadeAlpha = 1f;
                    _isFadingIn = false;
                    _fadeTimer.Stop();
                }
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (_currentBackground != null)
                g.DrawImage(_currentBackground, 0, 0, ClientSize.Width, ClientSize.Height);
            else
            {
                using var bgBrush = new LinearGradientBrush(
                    ClientRectangle,
                    Color.FromArgb(12, 16, 28),
                    Color.FromArgb(28, 36, 56),
                    90f);
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            if (_fadeAlpha < 1f)
            {
                int alpha = (int)(255 * (1 - _fadeAlpha));
                using var fadeBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
                g.FillRectangle(fadeBrush, 0, 0, ClientSize.Width, ClientSize.Height);
            }

            if (!string.IsNullOrEmpty(_currentCaption))
                DrawCenteredCaption(g);

            if (_currentSpeaker != Speaker.OnScreenText && !string.IsNullOrEmpty(_currentText))
                DrawDialogueBox(g);

            DrawControlHint(g);
        }

        private void DrawCenteredCaption(Graphics g)
        {
            using var captionFont = new Font("Georgia", 34f, FontStyle.Italic);
            using var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0));
            using var captionBrush = new SolidBrush(Color.FromArgb(230, 255, 240, 200));

            var size = g.MeasureString(_currentCaption, captionFont);
            float x = (ClientSize.Width - size.Width) / 2;
            float y = ClientSize.Height * 0.28f;

            g.DrawString(_currentCaption, captionFont, shadowBrush, x + 3, y + 3);
            g.DrawString(_currentCaption, captionFont, captionBrush, x, y);
        }

        private void DrawDialogueBox(Graphics g)
        {
            const float padding = 24f;
            float boxX = padding;
            float boxY = ClientSize.Height - 180;
            float boxW = ClientSize.Width - padding * 2;
            float boxH = 140;

            using var boxBg = new SolidBrush(Color.FromArgb(200, 8, 12, 24));
            g.FillRectangle(boxBg, boxX, boxY, boxW, boxH);

            using var borderPen = new Pen(Color.FromArgb(140, 120, 180, 220), 1.5f);
            g.DrawRectangle(borderPen, boxX, boxY, boxW, boxH);

            if (!string.IsNullOrEmpty(_currentSpeakerName))
            {
                using var nameFont = new Font("Georgia", 15f, FontStyle.Bold);
                using var nameBrush = new SolidBrush(GetSpeakerColor(_currentSpeaker));
                g.DrawString(_currentSpeakerName, nameFont, nameBrush, boxX + padding, boxY + 14);
            }

            using var textFont = new Font("Segoe UI", 12.5f);
            using var textBrush = new SolidBrush(Color.FromArgb(240, 245, 255));
            var textRect = new RectangleF(boxX + padding, boxY + 52, boxW - padding * 2, boxH - 62);
            g.DrawString(_currentText, textFont, textBrush, textRect);
        }

        private void DrawControlHint(Graphics g)
        {
            using var hintFont = new Font("Consolas", 9.5f);
            using var hintBrush = new SolidBrush(Color.FromArgb(130, 180, 210, 255));
            g.DrawString(
                "SPACE / CLICK: Continue    TAB: Skip Scene    ESC: Skip Intro",
                hintFont, hintBrush,
                ClientSize.Width - 450, ClientSize.Height - 24);
        }

        private static string GetSpeakerDisplayName(Speaker speaker)
        {
            return speaker switch
            {
                Speaker.Arin => "Arin",
                Speaker.Terra => "Terra",
                Speaker.VillageElder => "Village Elder",
                _ => ""
            };
        }

        private static Color GetSpeakerColor(Speaker speaker)
        {
            return speaker switch
            {
                Speaker.Arin => Color.FromArgb(160, 200, 255),
                Speaker.Terra => Color.FromArgb(160, 220, 110),
                Speaker.VillageElder => Color.FromArgb(220, 190, 140),
                _ => Color.FromArgb(200, 210, 240)
            };
        }
    }
}