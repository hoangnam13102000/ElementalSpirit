namespace ElementalSpirit.Presentation.Forms
{
    using ElementalSpirit.Domain.BossEncounter;
    using ElementalSpirit.Localization;
    using ElementalSpirit.Presentation.BossEncounter;
    using System.ComponentModel;
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
    public sealed partial class BossDialogueForm : Form, IBossDialogueView
    {
        private readonly ILocalizationService _localization = null!;
        private readonly string _playerName = "Arin";
        private Panel _dialoguePanel = null!;
        private Label _speakerLabel = null!;
        private Label _textLabel = null!;
        private Label _captionLabel = null!;
        private Label _sceneNameLabel = null!;
        private Button _nextButton = null!;
        private Button _skipButton = null!;

        public event EventHandler? NextLineRequested;
        public event EventHandler? SkipSceneRequested;

        public BossDialogueForm()
        {
            _localization = null!;
            _playerName = "Arin";
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _localization = LocalizationManager.Instance;
            ApplyTranslations();
        }

        public BossDialogueForm(ILocalizationService localization, string playerName = "Arin")
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            if (string.IsNullOrWhiteSpace(playerName))
                throw new ArgumentException("Player name cannot be empty.", nameof(playerName));
            _playerName = playerName.Trim();

            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            ApplyTranslations();
        }

        private void ApplyTranslations()
        {
            Text = _localization.Translate("boss.dialogue.title");
            _nextButton.Text = _localization.Translate("boss.dialogue.next");
            _skipButton.Text = _localization.Translate("boss.dialogue.skip");
        }

        private void NextButton_Click(object? sender, EventArgs e)
        {
            NextLineRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SkipButton_Click(object? sender, EventArgs e)
        {
            SkipSceneRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BossDialogueForm_KeyDown(object? sender, KeyEventArgs e)
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
        }

        public void ShowDialogueLine(BossDialogueLine line)
        {
            if (line.Speaker == BossDialogueSpeaker.OnScreenText)
            {
                // On-screen text: hiển thị ở giữa màn hình, ẩn khung thoại
                _dialoguePanel.Visible = false;
                _captionLabel.Visible = true;
                _captionLabel.Text = ReplacePlayerName(line.OnScreenCaption ?? line.Text);
            }
            else
            {
                // Normal dialogue
                _dialoguePanel.Visible = true;
                _captionLabel.Visible = false;
                _speakerLabel.Text = GetSpeakerDisplayName(line.Speaker);
                _speakerLabel.ForeColor = GetSpeakerColor(line.Speaker);
                _textLabel.Text = ReplacePlayerName(line.Text);
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
                BossDialogueSpeaker.Arin => _playerName,
                BossDialogueSpeaker.Gorgon => _localization.Translate("boss.speaker.gorgon"),
                BossDialogueSpeaker.Terra => _localization.Translate("boss.speaker.terra"),
                _ => ""
            };
        }

        private string ReplacePlayerName(string text) =>
            text.Replace("{playerName}", _playerName, StringComparison.OrdinalIgnoreCase);

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