namespace ElementalSpirit.Presentation.Forms
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label _titleLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(MainMenuForm));
            components = new System.ComponentModel.Container();
            _titleLabel = new System.Windows.Forms.Label();
            _btnContinue = new System.Windows.Forms.Button();
            _btnStart = new System.Windows.Forms.Button();
            _btnLeaderboard = new System.Windows.Forms.Button();
            _btnGuide = new System.Windows.Forms.Button();
            _btnSettings = new System.Windows.Forms.Button();
            _btnExit = new System.Windows.Forms.Button();
            _titleLabel.AutoEllipsis = true;
            _titleLabel.Font = new System.Drawing.Font("Georgia", 52F, System.Drawing.FontStyle.Bold);
            _titleLabel.ForeColor = System.Drawing.Color.FromArgb(245, 235, 255);
            _titleLabel.Location = new System.Drawing.Point(0, 60);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new System.Drawing.Size(1280, 80);
            _titleLabel.TabIndex = 0;
            _titleLabel.Text = "Elemental Spirit";
            _titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _titleLabel.BackColor = System.Drawing.Color.Transparent;
            SuspendLayout();
            //
            // Menu buttons
            //
            _btnContinue.BackColor = System.Drawing.Color.FromArgb(70, 40, 80, 150);
            _btnContinue.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnContinue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnContinue.Font = new System.Drawing.Font("Georgia", 16F, System.Drawing.FontStyle.Bold);
            _btnContinue.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _btnContinue.Location = new System.Drawing.Point(440, 190);
            _btnContinue.Name = "_btnContinue";
            _btnContinue.Size = new System.Drawing.Size(400, 58);
            _btnContinue.Text = "Continue";
            _btnContinue.UseVisualStyleBackColor = false;
            _btnContinue.FlatAppearance.BorderSize = 2;
            _btnContinue.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 180, 220);

            _btnStart.BackColor = System.Drawing.Color.FromArgb(70, 40, 80, 150);
            _btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnStart.Font = new System.Drawing.Font("Georgia", 16F, System.Drawing.FontStyle.Bold);
            _btnStart.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _btnStart.Location = new System.Drawing.Point(440, 260);
            _btnStart.Name = "_btnStart";
            _btnStart.Size = new System.Drawing.Size(400, 58);
            _btnStart.Text = "Start";
            _btnStart.UseVisualStyleBackColor = false;
            _btnStart.FlatAppearance.BorderSize = 2;
            _btnStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 180, 220);

            _btnLeaderboard.BackColor = System.Drawing.Color.FromArgb(70, 40, 80, 150);
            _btnLeaderboard.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnLeaderboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnLeaderboard.Font = new System.Drawing.Font("Georgia", 16F, System.Drawing.FontStyle.Bold);
            _btnLeaderboard.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _btnLeaderboard.Location = new System.Drawing.Point(440, 330);
            _btnLeaderboard.Name = "_btnLeaderboard";
            _btnLeaderboard.Size = new System.Drawing.Size(400, 58);
            _btnLeaderboard.Text = "Leaderboard";
            _btnLeaderboard.UseVisualStyleBackColor = false;
            _btnLeaderboard.FlatAppearance.BorderSize = 2;
            _btnLeaderboard.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 180, 220);

            _btnGuide.BackColor = System.Drawing.Color.FromArgb(70, 40, 80, 150);
            _btnGuide.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnGuide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnGuide.Font = new System.Drawing.Font("Georgia", 16F, System.Drawing.FontStyle.Bold);
            _btnGuide.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _btnGuide.Location = new System.Drawing.Point(440, 400);
            _btnGuide.Name = "_btnGuide";
            _btnGuide.Size = new System.Drawing.Size(400, 58);
            _btnGuide.Text = "Guide";
            _btnGuide.UseVisualStyleBackColor = false;
            _btnGuide.FlatAppearance.BorderSize = 2;
            _btnGuide.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 180, 220);

            _btnSettings.BackColor = System.Drawing.Color.FromArgb(70, 40, 80, 150);
            _btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnSettings.Font = new System.Drawing.Font("Georgia", 16F, System.Drawing.FontStyle.Bold);
            _btnSettings.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _btnSettings.Location = new System.Drawing.Point(440, 470);
            _btnSettings.Name = "_btnSettings";
            _btnSettings.Size = new System.Drawing.Size(400, 58);
            _btnSettings.Text = "Settings";
            _btnSettings.UseVisualStyleBackColor = false;
            _btnSettings.FlatAppearance.BorderSize = 2;
            _btnSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 180, 220);

            _btnExit.BackColor = System.Drawing.Color.FromArgb(70, 40, 80, 150);
            _btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnExit.Font = new System.Drawing.Font("Georgia", 16F, System.Drawing.FontStyle.Bold);
            _btnExit.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _btnExit.Location = new System.Drawing.Point(440, 540);
            _btnExit.Name = "_btnExit";
            _btnExit.Size = new System.Drawing.Size(400, 58);
            _btnExit.Text = "Exit";
            _btnExit.UseVisualStyleBackColor = false;
            _btnExit.FlatAppearance.BorderSize = 2;
            _btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 180, 220);
            _btnContinue.Click += BtnContinue_Click;
            _btnStart.Click += BtnStart_Click;
            _btnLeaderboard.Click += BtnLeaderboard_Click;
            _btnGuide.Click += BtnGuide_Click;
            _btnSettings.Click += BtnSettings_Click;
            _btnExit.Click += BtnExit_Click;
            Controls.Add(_btnContinue);
            Controls.Add(_btnStart);
            Controls.Add(_btnLeaderboard);
            Controls.Add(_btnGuide);
            Controls.Add(_btnSettings);
            Controls.Add(_btnExit);
            Controls.Add(_titleLabel);
            //
            // MainMenuForm
            //
            BackColor = System.Drawing.Color.FromArgb(6, 8, 15);
            BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(1280, 720);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "MainMenuForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Resize += MainMenuForm_Resize;
            ResumeLayout(false);
        }

    }
}
