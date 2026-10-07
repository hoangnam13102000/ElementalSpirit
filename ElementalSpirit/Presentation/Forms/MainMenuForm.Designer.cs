namespace ElementalSpirit.Presentation.Forms
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label _titleLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _responsiveTitleFont?.Dispose();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenuForm));
            components = new System.ComponentModel.Container();
            _titleLabel = new Label();
            _btnContinue = new Button();
            _btnStart = new Button();
            _btnLeaderboard = new Button();
            _btnGuide = new Button();
            _btnSettings = new Button();
            _btnExit = new Button();
            SuspendLayout();
            // 
            // _titleLabel
            // 
            _titleLabel.AutoEllipsis = true;
            _titleLabel.BackColor = Color.Transparent;
            _titleLabel.Font = new Font("Georgia", 52F, FontStyle.Bold);
            _titleLabel.ForeColor = Color.FromArgb(245, 235, 255);
            _titleLabel.Location = new Point(0, 60);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new Size(1280, 115);
            _titleLabel.TabIndex = 0;
            _titleLabel.Text = "Elemental Spirit";
            _titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _btnContinue
            // 
            _btnContinue.BackColor = Color.FromArgb(70, 40, 80, 150);
            _btnContinue.Cursor = Cursors.Hand;
            _btnContinue.FlatAppearance.BorderColor = Color.FromArgb(150, 180, 220);
            _btnContinue.FlatAppearance.BorderSize = 2;
            _btnContinue.FlatStyle = FlatStyle.Flat;
            _btnContinue.Font = new Font("Georgia", 16F, FontStyle.Bold);
            _btnContinue.ForeColor = Color.FromArgb(230, 220, 255);
            _btnContinue.Location = new Point(440, 190);
            _btnContinue.Name = "_btnContinue";
            _btnContinue.Size = new Size(400, 58);
            _btnContinue.TabIndex = 0;
            _btnContinue.Text = "Continue";
            _btnContinue.UseVisualStyleBackColor = false;
            _btnContinue.Click += BtnContinue_Click;
            // 
            // _btnStart
            // 
            _btnStart.BackColor = Color.FromArgb(70, 40, 80, 150);
            _btnStart.Cursor = Cursors.Hand;
            _btnStart.FlatAppearance.BorderColor = Color.FromArgb(150, 180, 220);
            _btnStart.FlatAppearance.BorderSize = 2;
            _btnStart.FlatStyle = FlatStyle.Flat;
            _btnStart.Font = new Font("Georgia", 16F, FontStyle.Bold);
            _btnStart.ForeColor = Color.FromArgb(230, 220, 255);
            _btnStart.Location = new Point(440, 260);
            _btnStart.Name = "_btnStart";
            _btnStart.Size = new Size(400, 58);
            _btnStart.TabIndex = 1;
            _btnStart.Text = "Start";
            _btnStart.UseVisualStyleBackColor = false;
            _btnStart.Click += BtnStart_Click;
            // 
            // _btnLeaderboard
            // 
            _btnLeaderboard.BackColor = Color.FromArgb(70, 40, 80, 150);
            _btnLeaderboard.Cursor = Cursors.Hand;
            _btnLeaderboard.FlatAppearance.BorderColor = Color.FromArgb(150, 180, 220);
            _btnLeaderboard.FlatAppearance.BorderSize = 2;
            _btnLeaderboard.FlatStyle = FlatStyle.Flat;
            _btnLeaderboard.Font = new Font("Georgia", 16F, FontStyle.Bold);
            _btnLeaderboard.ForeColor = Color.FromArgb(230, 220, 255);
            _btnLeaderboard.Location = new Point(440, 330);
            _btnLeaderboard.Name = "_btnLeaderboard";
            _btnLeaderboard.Size = new Size(400, 58);
            _btnLeaderboard.TabIndex = 2;
            _btnLeaderboard.Text = "Leaderboard";
            _btnLeaderboard.UseVisualStyleBackColor = false;
            _btnLeaderboard.Click += BtnLeaderboard_Click;
            // 
            // _btnGuide
            // 
            _btnGuide.BackColor = Color.FromArgb(70, 40, 80, 150);
            _btnGuide.Cursor = Cursors.Hand;
            _btnGuide.FlatAppearance.BorderColor = Color.FromArgb(150, 180, 220);
            _btnGuide.FlatAppearance.BorderSize = 2;
            _btnGuide.FlatStyle = FlatStyle.Flat;
            _btnGuide.Font = new Font("Georgia", 16F, FontStyle.Bold);
            _btnGuide.ForeColor = Color.FromArgb(230, 220, 255);
            _btnGuide.Location = new Point(440, 400);
            _btnGuide.Name = "_btnGuide";
            _btnGuide.Size = new Size(400, 58);
            _btnGuide.TabIndex = 3;
            _btnGuide.Text = "Guide";
            _btnGuide.UseVisualStyleBackColor = false;
            _btnGuide.Click += BtnGuide_Click;
            // 
            // _btnSettings
            // 
            _btnSettings.BackColor = Color.FromArgb(70, 40, 80, 150);
            _btnSettings.Cursor = Cursors.Hand;
            _btnSettings.FlatAppearance.BorderColor = Color.FromArgb(150, 180, 220);
            _btnSettings.FlatAppearance.BorderSize = 2;
            _btnSettings.FlatStyle = FlatStyle.Flat;
            _btnSettings.Font = new Font("Georgia", 16F, FontStyle.Bold);
            _btnSettings.ForeColor = Color.FromArgb(230, 220, 255);
            _btnSettings.Location = new Point(440, 470);
            _btnSettings.Name = "_btnSettings";
            _btnSettings.Size = new Size(400, 58);
            _btnSettings.TabIndex = 4;
            _btnSettings.Text = "Settings";
            _btnSettings.UseVisualStyleBackColor = false;
            _btnSettings.Click += BtnSettings_Click;
            // 
            // _btnExit
            // 
            _btnExit.BackColor = Color.FromArgb(70, 40, 80, 150);
            _btnExit.Cursor = Cursors.Hand;
            _btnExit.FlatAppearance.BorderColor = Color.FromArgb(150, 180, 220);
            _btnExit.FlatAppearance.BorderSize = 2;
            _btnExit.FlatStyle = FlatStyle.Flat;
            _btnExit.Font = new Font("Georgia", 16F, FontStyle.Bold);
            _btnExit.ForeColor = Color.FromArgb(230, 220, 255);
            _btnExit.Location = new Point(440, 540);
            _btnExit.Name = "_btnExit";
            _btnExit.Size = new Size(400, 58);
            _btnExit.TabIndex = 5;
            _btnExit.Text = "Exit";
            _btnExit.UseVisualStyleBackColor = false;
            _btnExit.Click += BtnExit_Click;
            // 
            // MainMenuForm
            // 
            BackColor = Color.FromArgb(6, 8, 15);
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1280, 720);
            Controls.Add(_btnContinue);
            Controls.Add(_btnStart);
            Controls.Add(_btnLeaderboard);
            Controls.Add(_btnGuide);
            Controls.Add(_btnSettings);
            Controls.Add(_btnExit);
            Controls.Add(_titleLabel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "MainMenuForm";
            StartPosition = FormStartPosition.CenterScreen;
            Resize += MainMenuForm_Resize;
            ResumeLayout(false);
        }

    }
}
