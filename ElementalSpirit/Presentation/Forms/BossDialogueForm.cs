namespace ElementalSpirit.Presentation.Forms
{
    using ElementalSpirit.Domain.BossEncounter;
    using ElementalSpirit.Localization;
    using ElementalSpirit.Presentation.BossEncounter;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Form hiển thị thoại boss. Implement IBossDialogueView.
    /// 
    /// Model-View-Presenter:
    /// - Đây là View, chỉ chịu trách nhiệm:
    ///   a) Hiển thị dữ liệu do Presenter cung cấp
    ///   b) Nhận input từ người dùng và chuyển tiếp cho Presenter qua event
    /// - KHÔNG chứa logic xử lý luồng thoại
    /// - KHÔNG chứa logic game
    /// </summary>
    public sealed class BossDialogueForm : Form, IBossDialogueView
    {
        private readonly ILocalizationService _localization;
        private readonly Panel _dialoguePanel;
        private readonly Label _speakerLabel;
        private readonly Label _textLabel;
        private readonly Label _captionLabel;
        private readonly Label _sceneNameLabel;
        private readonly Button _nextButton;
        private readonly Button _skipButton;

        public event EventHandler? NextLineRequested;
        public event EventHandler? SkipSceneRequested;

        public BossDialogueForm(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // Form setup
            Text = _localization.Translate("boss.dialogue.title");
            ClientSize = new Size(1280, 720);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            DoubleBuffered = true;
            KeyPreview = true;

            // Scene name label (top)
            _sceneNameLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 40,
                ForeColor = Color.FromArgb(255, 215, 0),
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(180, 0, 0, 0)
            };
            Controls.Add(_sceneNameLabel);

            // Dialogue panel (bottom)
            _dialoguePanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 220,
                BackColor = Color.FromArgb(200, 20, 20, 40),
                Padding = new Padding(30)
            };

            // Speaker label
            _speakerLabel = new Label
            {
                Location = new Point(30, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 200, 255)
            };
            _dialoguePanel.Controls.Add(_speakerLabel);

            // Text label
            _textLabel = new Label
            {
                Location = new Point(30, 60),
                Size = new Size(1000, 100),
                Font = new Font("Segoe UI", 13f),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopLeft
            };
            _dialoguePanel.Controls.Add(_textLabel);

            // Caption label (on-screen text)
            _captionLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18f, FontStyle.Italic),
                ForeColor = Color.FromArgb(255, 230, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            Controls.Add(_captionLabel);

            // Next button
            _nextButton = new Button
            {
                Text = _localization.Translate("boss.dialogue.next"),
                Location = new Point(1080, 160),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(60, 120, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold)
            };
            _nextButton.Click += (s, e) => NextLineRequested?.Invoke(this, EventArgs.Empty);
            _dialoguePanel.Controls.Add(_nextButton);

            // Skip button
            _skipButton = new Button
            {
                Text = _localization.Translate("boss.dialogue.skip"),
                Location = new Point(940, 160),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f)
            };
            _skipButton.Click += (s, e) => SkipSceneRequested?.Invoke(this, EventArgs.Empty);
            _dialoguePanel.Controls.Add(_skipButton);

            Controls.Add(_dialoguePanel);

            // Keyboard support
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    NextLineRequested?.Invoke(this, EventArgs.Empty);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true;
                    SkipSceneRequested?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public void ShowDialogueLine(BossDialogueLine line)
        {
            if (line.Speaker == BossDialogueSpeaker.OnScreenText)
            {
                // On-screen text: hiển thị ở giữa màn hình, ẩn khung thoại
                _dialoguePanel.Visible = false;
                _captionLabel.Visible = true;
                _captionLabel.Text = line.OnScreenCaption ?? line.Text;
            }
            else
            {
                // Normal dialogue
                _dialoguePanel.Visible = true;
                _captionLabel.Visible = false;
                _speakerLabel.Text = GetSpeakerDisplayName(line.Speaker);
                _speakerLabel.ForeColor = GetSpeakerColor(line.Speaker);
                _textLabel.Text = line.Text;
            }
        }

        public void ShowSceneName(string sceneName)
        {
            _sceneNameLabel.Text = sceneName;
        }

        public void SetDialogueVisible(bool visible)
        {
            _dialoguePanel.Visible = visible;
            _sceneNameLabel.Visible = visible;
            if (!visible) _captionLabel.Visible = false;
        }

        public void CloseView()
        {
            Close();
        }

        public void ShowInfo(string message, string title)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetSpeakerDisplayName(BossDialogueSpeaker speaker)
        {
            return speaker switch
            {
                BossDialogueSpeaker.Narrator => _localization.Translate("boss.speaker.narrator"),
                BossDialogueSpeaker.Arin => _localization.Translate("boss.speaker.arin"),
                BossDialogueSpeaker.Gorgon => _localization.Translate("boss.speaker.gorgon"),
                BossDialogueSpeaker.Terra => _localization.Translate("boss.speaker.terra"),
                _ => ""
            };
        }

        private static Color GetSpeakerColor(BossDialogueSpeaker speaker)
        {
            return speaker switch
            {
                BossDialogueSpeaker.Narrator => Color.FromArgb(200, 200, 200),
                BossDialogueSpeaker.Arin => Color.FromArgb(100, 200, 255),
                BossDialogueSpeaker.Gorgon => Color.FromArgb(255, 100, 100),
                BossDialogueSpeaker.Terra => Color.FromArgb(150, 220, 100),
                _ => Color.White
            };
        }
    }
}