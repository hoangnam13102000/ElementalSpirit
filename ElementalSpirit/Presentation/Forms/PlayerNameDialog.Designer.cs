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
                components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _promptLabel = new Label();
            _nameTextBox = new TextBox();
            _errorLabel = new Label();
            _startButton = new Button();
            _cancelButton = new Button();
            SuspendLayout();
            // 
            // _promptLabel
            // 
            _promptLabel.Font = new Font("Segoe UI", 10F);
            _promptLabel.ForeColor = Color.FromArgb(230, 220, 255);
            _promptLabel.Location = new Point(24, 20);
            _promptLabel.Name = "_promptLabel";
            _promptLabel.Size = new Size(372, 28);
            _promptLabel.TabIndex = 0;
            _promptLabel.Text = "Enter your character name";
            // 
            // _nameTextBox
            // 
            _nameTextBox.Font = new Font("Segoe UI", 11F);
            _nameTextBox.Location = new Point(24, 54);
            _nameTextBox.MaxLength = 24;
            _nameTextBox.Name = "_nameTextBox";
            _nameTextBox.Size = new Size(372, 37);
            _nameTextBox.TabIndex = 1;
            _nameTextBox.TextChanged += _nameTextBox_TextChanged;
            // 
            // _errorLabel
            // 
            _errorLabel.ForeColor = Color.LightCoral;
            _errorLabel.Location = new Point(24, 85);
            _errorLabel.Name = "_errorLabel";
            _errorLabel.Size = new Size(372, 22);
            _errorLabel.TabIndex = 2;
            _errorLabel.Visible = false;
            // 
            // _startButton
            // 
            _startButton.BackColor = Color.FromArgb(55, 90, 140);
            _startButton.FlatAppearance.BorderColor = Color.FromArgb(150, 190, 240);
            _startButton.FlatStyle = FlatStyle.Flat;
            _startButton.ForeColor = Color.White;
            _startButton.Location = new Point(220, 120);
            _startButton.Name = "_startButton";
            _startButton.Size = new Size(176, 38);
            _startButton.TabIndex = 3;
            _startButton.Text = "Start";
            _startButton.UseVisualStyleBackColor = false;
            _startButton.Click += StartButton_Click;
            // 
            // _cancelButton
            // 
            _cancelButton.BackColor = Color.FromArgb(65, 55, 80);
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.FlatAppearance.BorderColor = Color.FromArgb(150, 140, 170);
            _cancelButton.FlatStyle = FlatStyle.Flat;
            _cancelButton.ForeColor = Color.White;
            _cancelButton.Location = new Point(24, 120);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(176, 38);
            _cancelButton.TabIndex = 4;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = false;
            // 
            // PlayerNameDialog
            // 
            AcceptButton = _startButton;
            BackColor = Color.FromArgb(24, 18, 36);
            CancelButton = _cancelButton;
            ClientSize = new Size(420, 185);
            Controls.Add(_cancelButton);
            Controls.Add(_startButton);
            Controls.Add(_errorLabel);
            Controls.Add(_nameTextBox);
            Controls.Add(_promptLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PlayerNameDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Character name";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
