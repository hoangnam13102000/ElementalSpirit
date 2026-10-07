namespace ElementalSpirit.Presentation.Forms.Settings
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components;
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
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _lblTitle = new System.Windows.Forms.Label();
            _lblLanguage = new System.Windows.Forms.Label();
            _cmbLanguage = new System.Windows.Forms.ComboBox();
            _lblResolution = new System.Windows.Forms.Label();
            _lblMusic = new System.Windows.Forms.Label();
            _chkMusic = new System.Windows.Forms.Button();
            _lblSfx = new System.Windows.Forms.Label();
            _chkSfx = new System.Windows.Forms.Button();
            _lblFullscreen = new System.Windows.Forms.Label();
            _chkFullscreen = new System.Windows.Forms.Button();
            _lblSaveGame = new System.Windows.Forms.Label();
            _btnSaveGame = new System.Windows.Forms.Button();
            _btnSave = new System.Windows.Forms.Button();
            _btnCancel = new System.Windows.Forms.Button();
            _btnExitGame = new System.Windows.Forms.Button();
            SuspendLayout();
            //
            // Labels
            //
            _lblTitle.AutoSize = true;
            _lblTitle.Font = new System.Drawing.Font("Georgia", 16F, System.Drawing.FontStyle.Bold);
            _lblTitle.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _lblTitle.Location = new System.Drawing.Point(24, 20);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Text = "Settings";
            _lblLanguage.AutoSize = true;
            _lblLanguage.Font = new System.Drawing.Font("Segoe UI", 11F);
            _lblLanguage.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _lblLanguage.Location = new System.Drawing.Point(24, 70);
            _lblLanguage.Name = "_lblLanguage";
            _lblLanguage.Text = "Language";
            _lblResolution.AutoSize = true;
            _lblResolution.Font = new System.Drawing.Font("Segoe UI", 11F);
            _lblResolution.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _lblResolution.Location = new System.Drawing.Point(24, 120);
            _lblResolution.Name = "_lblResolution";
            _lblResolution.Text = "Resolution";
            _lblMusic.AutoSize = true;
            _lblMusic.Font = new System.Drawing.Font("Segoe UI", 11F);
            _lblMusic.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _lblMusic.Location = new System.Drawing.Point(24, 150);
            _lblMusic.Name = "_lblMusic";
            _lblMusic.Text = "Music";
            _lblSfx.AutoSize = true;
            _lblSfx.Font = new System.Drawing.Font("Segoe UI", 11F);
            _lblSfx.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _lblSfx.Location = new System.Drawing.Point(24, 185);
            _lblSfx.Name = "_lblSfx";
            _lblSfx.Text = "Sound effects";
            _lblFullscreen.AutoSize = true;
            _lblFullscreen.Font = new System.Drawing.Font("Segoe UI", 11F);
            _lblFullscreen.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _lblFullscreen.Location = new System.Drawing.Point(24, 220);
            _lblFullscreen.Name = "_lblFullscreen";
            _lblFullscreen.Text = "Fullscreen";
            _lblSaveGame.AutoSize = true;
            _lblSaveGame.Font = new System.Drawing.Font("Segoe UI", 11F);
            _lblSaveGame.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _lblSaveGame.Location = new System.Drawing.Point(24, 267);
            _lblSaveGame.Name = "_lblSaveGame";
            _lblSaveGame.Text = "Save game";
            //
            // _cmbLanguage
            //
            _cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _cmbLanguage.Location = new System.Drawing.Point(180, 67);
            _cmbLanguage.Name = "_cmbLanguage";
            _cmbLanguage.Size = new System.Drawing.Size(200, 23);
            _cmbLanguage.TabIndex = 1;
            _cmbLanguage.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;
            //
            // Toggle buttons
            //
            _chkMusic.Location = new System.Drawing.Point(230, 145);
            _chkMusic.Name = "_chkMusic";
            _chkMusic.Size = new System.Drawing.Size(36, 30);
            _chkMusic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _chkMusic.Font = new System.Drawing.Font("Segoe UI Emoji", 10F);
            _chkMusic.ForeColor = System.Drawing.Color.White;
            _chkMusic.Cursor = System.Windows.Forms.Cursors.Hand;
            _chkMusic.UseVisualStyleBackColor = false;
            _chkMusic.FlatAppearance.BorderSize = 0;
            _chkMusic.Click += MusicButton_Click;
            _chkSfx.Location = new System.Drawing.Point(230, 180);
            _chkSfx.Name = "_chkSfx";
            _chkSfx.Size = new System.Drawing.Size(36, 30);
            _chkSfx.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _chkSfx.Font = new System.Drawing.Font("Segoe UI Emoji", 10F);
            _chkSfx.ForeColor = System.Drawing.Color.White;
            _chkSfx.Cursor = System.Windows.Forms.Cursors.Hand;
            _chkSfx.UseVisualStyleBackColor = false;
            _chkSfx.FlatAppearance.BorderSize = 0;
            _chkSfx.Click += SfxButton_Click;
            _chkFullscreen.Location = new System.Drawing.Point(230, 215);
            _chkFullscreen.Name = "_chkFullscreen";
            _chkFullscreen.Size = new System.Drawing.Size(36, 30);
            _chkFullscreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _chkFullscreen.Font = new System.Drawing.Font("Segoe UI Emoji", 10F);
            _chkFullscreen.ForeColor = System.Drawing.Color.White;
            _chkFullscreen.Cursor = System.Windows.Forms.Cursors.Hand;
            _chkFullscreen.UseVisualStyleBackColor = false;
            _chkFullscreen.FlatAppearance.BorderSize = 0;
            _chkFullscreen.Click += FullscreenButton_Click;
            //
            // Action buttons
            //
            _btnSaveGame.Bounds = new System.Drawing.Rectangle(180, 263, 200, 36);
            _btnSaveGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnSaveGame.BackColor = System.Drawing.Color.FromArgb(70, 40, 80);
            _btnSaveGame.ForeColor = System.Drawing.Color.White;
            _btnSaveGame.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnSaveGame.UseVisualStyleBackColor = false;
            _btnSaveGame.Name = "_btnSaveGame";
            _btnSaveGame.Text = "Save game";
            _btnSaveGame.Visible = false;
            _btnSaveGame.Click += SaveGameButton_Click;
            _btnSaveGame.Paint += SaveGameButton_Paint;
            _btnSave.Bounds = new System.Drawing.Rectangle(70, 320, 120, 36);
            _btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnSave.BackColor = System.Drawing.Color.FromArgb(70, 40, 80);
            _btnSave.ForeColor = System.Drawing.Color.White;
            _btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnSave.UseVisualStyleBackColor = false;
            _btnSave.Name = "_btnSave";
            _btnSave.Text = "Save";
            _btnSave.Click += SaveButton_Click;
            _btnCancel.Bounds = new System.Drawing.Rectangle(220, 320, 120, 36);
            _btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnCancel.BackColor = System.Drawing.Color.FromArgb(70, 40, 80);
            _btnCancel.ForeColor = System.Drawing.Color.White;
            _btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnCancel.UseVisualStyleBackColor = false;
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Text = "Cancel";
            _btnCancel.Click += CancelButton_Click;
            _btnExitGame.Bounds = new System.Drawing.Rectangle(75, 370, 270, 36);
            _btnExitGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _btnExitGame.BackColor = System.Drawing.Color.FromArgb(70, 40, 80);
            _btnExitGame.ForeColor = System.Drawing.Color.White;
            _btnExitGame.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnExitGame.UseVisualStyleBackColor = false;
            _btnExitGame.Name = "_btnExitGame";
            _btnExitGame.Text = "Exit to menu";
            _btnExitGame.BackColor = System.Drawing.Color.FromArgb(120, 30, 30);
            _btnExitGame.Click += ExitGameButton_Click;
            //
            // SettingsForm
            //
            BackColor = System.Drawing.Color.FromArgb(24, 18, 36);
            ClientSize = new System.Drawing.Size(420, 485);
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
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

    }
}
