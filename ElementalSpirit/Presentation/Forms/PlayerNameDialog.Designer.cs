namespace ElementalSpirit.Presentation.Forms
{
    partial class PlayerNameDialog
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label _promptLabel;
        private System.Windows.Forms.TextBox _nameTextBox;
        private System.Windows.Forms.Label _errorLabel;
        private System.Windows.Forms.Button _startButton;
        private System.Windows.Forms.Button _cancelButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _promptLabel = new System.Windows.Forms.Label();
            _nameTextBox = new System.Windows.Forms.TextBox();
            _errorLabel = new System.Windows.Forms.Label();
            _startButton = new System.Windows.Forms.Button();
            _cancelButton = new System.Windows.Forms.Button();
            SuspendLayout();
            //
            // _promptLabel
            //
            _promptLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            _promptLabel.ForeColor = System.Drawing.Color.FromArgb(230, 220, 255);
            _promptLabel.Location = new System.Drawing.Point(24, 20);
            _promptLabel.Name = "_promptLabel";
            _promptLabel.Size = new System.Drawing.Size(372, 28);
            _promptLabel.TabIndex = 0;
            _promptLabel.Text = "Enter your character name";
            //
            // _nameTextBox
            //
            _nameTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            _nameTextBox.Location = new System.Drawing.Point(24, 54);
            _nameTextBox.MaxLength = 24;
            _nameTextBox.Name = "_nameTextBox";
            _nameTextBox.Size = new System.Drawing.Size(372, 27);
            _nameTextBox.TabIndex = 1;
            //
            // _errorLabel
            //
            _errorLabel.ForeColor = System.Drawing.Color.LightCoral;
            _errorLabel.Location = new System.Drawing.Point(24, 85);
            _errorLabel.Name = "_errorLabel";
            _errorLabel.Size = new System.Drawing.Size(372, 22);
            _errorLabel.TabIndex = 2;
            _errorLabel.Visible = false;
            //
            // _startButton
            //
            _startButton.BackColor = System.Drawing.Color.FromArgb(55, 90, 140);
            _startButton.DialogResult = System.Windows.Forms.DialogResult.None;
            _startButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _startButton.ForeColor = System.Drawing.Color.White;
            _startButton.Location = new System.Drawing.Point(220, 120);
            _startButton.Name = "_startButton";
            _startButton.Size = new System.Drawing.Size(176, 38);
            _startButton.TabIndex = 3;
            _startButton.Text = "Start";
            _startButton.UseVisualStyleBackColor = false;
            _startButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 190, 240);
            _startButton.Click += StartButton_Click;
            //
            // _cancelButton
            //
            _cancelButton.BackColor = System.Drawing.Color.FromArgb(65, 55, 80);
            _cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _cancelButton.ForeColor = System.Drawing.Color.White;
            _cancelButton.Location = new System.Drawing.Point(24, 120);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new System.Drawing.Size(176, 38);
            _cancelButton.TabIndex = 4;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = false;
            _cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 140, 170);
            //
            // PlayerNameDialog
            //
            AcceptButton = _startButton;
            CancelButton = _cancelButton;
            BackColor = System.Drawing.Color.FromArgb(24, 18, 36);
            ClientSize = new System.Drawing.Size(420, 185);
            Controls.Add(_cancelButton);
            Controls.Add(_startButton);
            Controls.Add(_errorLabel);
            Controls.Add(_nameTextBox);
            Controls.Add(_promptLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PlayerNameDialog";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Character name";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
