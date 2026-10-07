namespace ElementalSpirit.Presentation.Forms
{
    partial class BossDialogueForm
    {
        private System.ComponentModel.IContainer components;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _sceneNameLabel = new System.Windows.Forms.Label();
            _dialoguePanel = new System.Windows.Forms.Panel();
            _speakerLabel = new System.Windows.Forms.Label();
            _textLabel = new System.Windows.Forms.Label();
            _nextButton = new System.Windows.Forms.Button();
            _skipButton = new System.Windows.Forms.Button();
            _captionLabel = new System.Windows.Forms.Label();
            _dialoguePanel.SuspendLayout();
            SuspendLayout();
            //
            // _sceneNameLabel
            //
            _sceneNameLabel.BackColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
            _sceneNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            _sceneNameLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            _sceneNameLabel.ForeColor = System.Drawing.Color.FromArgb(255, 215, 0);
            _sceneNameLabel.Height = 40;
            _sceneNameLabel.Name = "_sceneNameLabel";
            _sceneNameLabel.Text = "Scene";
            _sceneNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // _dialoguePanel
            //
            _dialoguePanel.BackColor = System.Drawing.Color.FromArgb(200, 20, 20, 40);
            _dialoguePanel.Controls.Add(_speakerLabel);
            _dialoguePanel.Controls.Add(_textLabel);
            _dialoguePanel.Controls.Add(_skipButton);
            _dialoguePanel.Controls.Add(_nextButton);
            _dialoguePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            _dialoguePanel.Height = 220;
            _dialoguePanel.Name = "_dialoguePanel";
            _dialoguePanel.Padding = new System.Windows.Forms.Padding(30);
            //
            // _speakerLabel
            //
            _speakerLabel.AutoSize = true;
            _speakerLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            _speakerLabel.ForeColor = System.Drawing.Color.FromArgb(100, 200, 255);
            _speakerLabel.Location = new System.Drawing.Point(30, 20);
            _speakerLabel.Name = "_speakerLabel";
            _speakerLabel.Text = "Speaker";
            //
            // _textLabel
            //
            _textLabel.Font = new System.Drawing.Font("Segoe UI", 13F);
            _textLabel.ForeColor = System.Drawing.Color.White;
            _textLabel.Location = new System.Drawing.Point(30, 60);
            _textLabel.Name = "_textLabel";
            _textLabel.Size = new System.Drawing.Size(1000, 100);
            _textLabel.Text = "Dialogue text";
            _textLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // _nextButton
            //
            _nextButton.BackColor = System.Drawing.Color.FromArgb(60, 120, 200);
            _nextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _nextButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            _nextButton.ForeColor = System.Drawing.Color.White;
            _nextButton.Location = new System.Drawing.Point(1080, 160);
            _nextButton.Name = "_nextButton";
            _nextButton.Size = new System.Drawing.Size(120, 40);
            _nextButton.Text = "Next";
            _nextButton.Click += NextButton_Click;
            //
            // _skipButton
            //
            _skipButton.BackColor = System.Drawing.Color.FromArgb(80, 80, 100);
            _skipButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _skipButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            _skipButton.ForeColor = System.Drawing.Color.White;
            _skipButton.Location = new System.Drawing.Point(940, 160);
            _skipButton.Name = "_skipButton";
            _skipButton.Size = new System.Drawing.Size(120, 40);
            _skipButton.Text = "Skip";
            _skipButton.Click += SkipButton_Click;
            //
            // _captionLabel
            //
            _captionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            _captionLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Italic);
            _captionLabel.ForeColor = System.Drawing.Color.FromArgb(255, 230, 150);
            _captionLabel.Name = "_captionLabel";
            _captionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _captionLabel.Visible = false;
            //
            // BossDialogueForm
            //
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(1280, 720);
            Controls.Add(_captionLabel);
            Controls.Add(_sceneNameLabel);
            Controls.Add(_dialoguePanel);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            KeyPreview = true;
            Name = "BossDialogueForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            KeyDown += BossDialogueForm_KeyDown;
            _dialoguePanel.ResumeLayout(false);
            _dialoguePanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}
