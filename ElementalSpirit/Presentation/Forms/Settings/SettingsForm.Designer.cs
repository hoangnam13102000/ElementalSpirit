namespace ElementalSpirit.Presentation.Forms.Settings
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null!;
        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.Label _lblLanguage;
        private System.Windows.Forms.ComboBox _cmbLanguage;
        private System.Windows.Forms.Label _lblResolution;
        private System.Windows.Forms.Label _lblMusic;
        private System.Windows.Forms.Button _chkMusic;
        private System.Windows.Forms.Label _lblSfx;
        private System.Windows.Forms.Button _chkSfx;
        private System.Windows.Forms.Label _lblFullscreen;
        private System.Windows.Forms.Button _chkFullscreen;
        private System.Windows.Forms.Label _lblSaveGame;
        private System.Windows.Forms.Button _btnSaveGame;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnExitGame;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _lblTitle = new Label();
            _lblLanguage = new Label();
            _cmbLanguage = new ComboBox();
            _lblResolution = new Label();
            _lblMusic = new Label();
            _chkMusic = new Button();
            _lblSfx = new Label();
            _chkSfx = new Button();
            _lblFullscreen = new Label();
            _chkFullscreen = new Button();
            _lblSaveGame = new Label();
            _btnSaveGame = new Button();
            _btnSave = new Button();
            _btnCancel = new Button();
            _btnExitGame = new Button();
            SuspendLayout();
            // 
            // _lblTitle
            // 
            _lblTitle.AutoSize = true;
            _lblTitle.Font = new Font("Georgia", 16F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.FromArgb(230, 220, 255);
            _lblTitle.Location = new Point(24, 20);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(149, 38);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Settings";
            // 
            // _lblLanguage
            // 
            _lblLanguage.AutoSize = true;
            _lblLanguage.Font = new Font("Segoe UI", 11F);
            _lblLanguage.ForeColor = Color.FromArgb(230, 220, 255);
            _lblLanguage.Location = new Point(24, 70);
            _lblLanguage.Name = "_lblLanguage";
            _lblLanguage.Size = new Size(107, 30);
            _lblLanguage.TabIndex = 1;
            _lblLanguage.Text = "Language";
            // 
            // _cmbLanguage
            // 
            _cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbLanguage.Location = new Point(180, 67);
            _cmbLanguage.Name = "_cmbLanguage";
            _cmbLanguage.Size = new Size(200, 33);
            _cmbLanguage.TabIndex = 1;
            _cmbLanguage.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;
            // 
            // _lblResolution
            // 
            _lblResolution.AutoSize = true;
            _lblResolution.Font = new Font("Segoe UI", 11F);
            _lblResolution.ForeColor = Color.FromArgb(230, 220, 255);
            _lblResolution.Location = new Point(24, 120);
            _lblResolution.Name = "_lblResolution";
            _lblResolution.Size = new Size(113, 30);
            _lblResolution.TabIndex = 2;
            _lblResolution.Text = "Resolution";
            // 
            // _lblMusic
            // 
            _lblMusic.AutoSize = true;
            _lblMusic.Font = new Font("Segoe UI", 11F);
            _lblMusic.ForeColor = Color.FromArgb(230, 220, 255);
            _lblMusic.Location = new Point(24, 150);
            _lblMusic.Name = "_lblMusic";
            _lblMusic.Size = new Size(69, 30);
            _lblMusic.TabIndex = 3;
            _lblMusic.Text = "Music";
            // 
            // _chkMusic
            // 
            _chkMusic.Cursor = Cursors.Hand;
            _chkMusic.FlatAppearance.BorderSize = 0;
            _chkMusic.FlatStyle = FlatStyle.Flat;
            _chkMusic.Font = new Font("Segoe UI Emoji", 10F);
            _chkMusic.ForeColor = Color.White;
            _chkMusic.Location = new Point(230, 145);
            _chkMusic.Name = "_chkMusic";
            _chkMusic.Size = new Size(36, 30);
            _chkMusic.TabIndex = 4;
            _chkMusic.UseVisualStyleBackColor = false;
            _chkMusic.Click += MusicButton_Click;
            // 
            // _lblSfx
            // 
            _lblSfx.AutoSize = true;
            _lblSfx.Font = new Font("Segoe UI", 11F);
            _lblSfx.ForeColor = Color.FromArgb(230, 220, 255);
            _lblSfx.Location = new Point(24, 185);
            _lblSfx.Name = "_lblSfx";
            _lblSfx.Size = new Size(145, 30);
            _lblSfx.TabIndex = 5;
            _lblSfx.Text = "Sound effects";
            // 
            // _chkSfx
            // 
            _chkSfx.Cursor = Cursors.Hand;
            _chkSfx.FlatAppearance.BorderSize = 0;
            _chkSfx.FlatStyle = FlatStyle.Flat;
            _chkSfx.Font = new Font("Segoe UI Emoji", 10F);
            _chkSfx.ForeColor = Color.White;
            _chkSfx.Location = new Point(230, 180);
            _chkSfx.Name = "_chkSfx";
            _chkSfx.Size = new Size(36, 30);
            _chkSfx.TabIndex = 6;
            _chkSfx.UseVisualStyleBackColor = false;
            _chkSfx.Click += SfxButton_Click;
            // 
            // _lblFullscreen
            // 
            _lblFullscreen.AutoSize = true;
            _lblFullscreen.Font = new Font("Segoe UI", 11F);
            _lblFullscreen.ForeColor = Color.FromArgb(230, 220, 255);
            _lblFullscreen.Location = new Point(24, 220);
            _lblFullscreen.Name = "_lblFullscreen";
            _lblFullscreen.Size = new Size(109, 30);
            _lblFullscreen.TabIndex = 7;
            _lblFullscreen.Text = "Fullscreen";
            // 
            // _chkFullscreen
            // 
            _chkFullscreen.Cursor = Cursors.Hand;
            _chkFullscreen.FlatAppearance.BorderSize = 0;
            _chkFullscreen.FlatStyle = FlatStyle.Flat;
            _chkFullscreen.Font = new Font("Segoe UI Emoji", 10F);
            _chkFullscreen.ForeColor = Color.White;
            _chkFullscreen.Location = new Point(230, 215);
            _chkFullscreen.Name = "_chkFullscreen";
            _chkFullscreen.Size = new Size(36, 30);
            _chkFullscreen.TabIndex = 8;
            _chkFullscreen.UseVisualStyleBackColor = false;
            _chkFullscreen.Click += FullscreenButton_Click;
            // 
            // _lblSaveGame
            // 
            _lblSaveGame.AutoSize = true;
            _lblSaveGame.Font = new Font("Segoe UI", 11F);
            _lblSaveGame.ForeColor = Color.FromArgb(230, 220, 255);
            _lblSaveGame.Location = new Point(24, 267);
            _lblSaveGame.Name = "_lblSaveGame";
            _lblSaveGame.Size = new Size(120, 30);
            _lblSaveGame.TabIndex = 9;
            _lblSaveGame.Text = "Save game";
            // 
            // _btnSaveGame
            // 
            _btnSaveGame.BackColor = Color.FromArgb(70, 40, 80);
            _btnSaveGame.Cursor = Cursors.Hand;
            _btnSaveGame.FlatStyle = FlatStyle.Flat;
            _btnSaveGame.ForeColor = Color.White;
            _btnSaveGame.Location = new Point(180, 263);
            _btnSaveGame.Name = "_btnSaveGame";
            _btnSaveGame.Size = new Size(200, 36);
            _btnSaveGame.TabIndex = 10;
            _btnSaveGame.Text = "Save game";
            _btnSaveGame.UseVisualStyleBackColor = false;
            _btnSaveGame.Visible = false;
            _btnSaveGame.Click += SaveGameButton_Click;
            _btnSaveGame.Paint += SaveGameButton_Paint;
            // 
            // _btnSave
            // 
            _btnSave.BackColor = Color.FromArgb(70, 40, 80);
            _btnSave.Cursor = Cursors.Hand;
            _btnSave.FlatStyle = FlatStyle.Flat;
            _btnSave.ForeColor = Color.White;
            _btnSave.Location = new Point(70, 320);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(120, 36);
            _btnSave.TabIndex = 11;
            _btnSave.Text = "Save";
            _btnSave.UseVisualStyleBackColor = false;
            _btnSave.Click += SaveButton_Click;
            // 
            // _btnCancel
            // 
            _btnCancel.BackColor = Color.FromArgb(70, 40, 80);
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.ForeColor = Color.White;
            _btnCancel.Location = new Point(220, 320);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(120, 36);
            _btnCancel.TabIndex = 12;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            _btnCancel.Click += CancelButton_Click;
            // 
            // _btnExitGame
            // 
            _btnExitGame.BackColor = Color.FromArgb(120, 30, 30);
            _btnExitGame.Cursor = Cursors.Hand;
            _btnExitGame.FlatStyle = FlatStyle.Flat;
            _btnExitGame.ForeColor = Color.White;
            _btnExitGame.Location = new Point(70, 386);
            _btnExitGame.Name = "_btnExitGame";
            _btnExitGame.Size = new Size(270, 36);
            _btnExitGame.TabIndex = 13;
            _btnExitGame.Text = "Exit to menu";
            _btnExitGame.UseVisualStyleBackColor = false;
            _btnExitGame.Click += ExitGameButton_Click;
            // 
            // SettingsForm
            // 
            BackColor = Color.FromArgb(24, 18, 36);
            ClientSize = new Size(420, 485);
            Controls.Add(_lblTitle);
            Controls.Add(_lblLanguage);
            Controls.Add(_cmbLanguage);
            Controls.Add(_lblResolution);
            Controls.Add(_lblMusic);
            Controls.Add(_chkMusic);
            Controls.Add(_lblSfx);
            Controls.Add(_chkSfx);
            Controls.Add(_lblFullscreen);
            Controls.Add(_chkFullscreen);
            Controls.Add(_lblSaveGame);
            Controls.Add(_btnSaveGame);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);
            Controls.Add(_btnExitGame);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

    }
}
