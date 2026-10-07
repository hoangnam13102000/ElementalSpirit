namespace ElementalSpirit.Presentation.Dialogs
{
    partial class BaseDialog
    {
        private System.ComponentModel.IContainer components;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _headerPanel = new System.Windows.Forms.Panel();
            _titleLabel = new System.Windows.Forms.Label();
            _iconPanel = new System.Windows.Forms.Panel();
            MessageLabel = new System.Windows.Forms.Label();
            ButtonPanel = new System.Windows.Forms.Panel();
            SecondaryButton = new System.Windows.Forms.Button();
            PrimaryButton = new System.Windows.Forms.Button();
            _headerPanel.SuspendLayout();
            ButtonPanel.SuspendLayout();
            SuspendLayout();
            //
            // _headerPanel
            //
            _headerPanel.BackColor = System.Drawing.Color.White;
            _headerPanel.Controls.Add(_titleLabel);
            _headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            _headerPanel.Height = 72;
            _headerPanel.Name = "_headerPanel";
            _headerPanel.Paint += HeaderPanel_Paint;
            //
            // _titleLabel
            //
            _titleLabel.AutoSize = true;
            _titleLabel.Font = new System.Drawing.Font("Segoe UI", 16F);
            _titleLabel.ForeColor = System.Drawing.Color.FromArgb(20, 25, 35);
            _titleLabel.Location = new System.Drawing.Point(68, 18);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Text = "Dialog title";
            //
            // _iconPanel
            //
            _iconPanel.BackColor = System.Drawing.Color.FromArgb(255, 242, 242);
            _iconPanel.Location = new System.Drawing.Point(28, 90);
            _iconPanel.Name = "_iconPanel";
            _iconPanel.Size = new System.Drawing.Size(72, 72);
            _iconPanel.Paint += IconPanel_Paint;
            //
            // MessageLabel
            //
            MessageLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            MessageLabel.ForeColor = System.Drawing.Color.FromArgb(25, 30, 40);
            MessageLabel.Location = new System.Drawing.Point(126, 108);
            MessageLabel.Name = "MessageLabel";
            MessageLabel.Size = new System.Drawing.Size(340, 48);
            MessageLabel.Text = "Dialog message";
            //
            // ButtonPanel
            //
            ButtonPanel.Controls.Add(SecondaryButton);
            ButtonPanel.Controls.Add(PrimaryButton);
            ButtonPanel.Location = new System.Drawing.Point(28, 194);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Size = new System.Drawing.Size(444, 52);
            ButtonPanel.TabIndex = 0;
            //
            // SecondaryButton
            //
            SecondaryButton.BackColor = System.Drawing.Color.FromArgb(241, 245, 250);
            SecondaryButton.Cursor = System.Windows.Forms.Cursors.Hand;
            SecondaryButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            SecondaryButton.FlatAppearance.BorderSize = 0;
            SecondaryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            SecondaryButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            SecondaryButton.ForeColor = System.Drawing.Color.FromArgb(35, 45, 60);
            SecondaryButton.Location = new System.Drawing.Point(188, 6);
            SecondaryButton.Name = "SecondaryButton";
            SecondaryButton.Size = new System.Drawing.Size(110, 40);
            SecondaryButton.TabIndex = 0;
            SecondaryButton.Text = "Cancel";
            SecondaryButton.UseVisualStyleBackColor = false;
            //
            // PrimaryButton
            //
            PrimaryButton.BackColor = System.Drawing.Color.FromArgb(45, 105, 190);
            PrimaryButton.Cursor = System.Windows.Forms.Cursors.Hand;
            PrimaryButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            PrimaryButton.FlatAppearance.BorderSize = 0;
            PrimaryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            PrimaryButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            PrimaryButton.ForeColor = System.Drawing.Color.White;
            PrimaryButton.Location = new System.Drawing.Point(310, 6);
            PrimaryButton.Name = "PrimaryButton";
            PrimaryButton.Size = new System.Drawing.Size(110, 40);
            PrimaryButton.TabIndex = 1;
            PrimaryButton.Text = "OK";
            PrimaryButton.UseVisualStyleBackColor = false;
            //
            // BaseDialog
            //
            BackColor = System.Drawing.Color.White;
            AcceptButton = PrimaryButton;
            CancelButton = SecondaryButton;
            ClientSize = new System.Drawing.Size(500, 260);
            Controls.Add(ButtonPanel);
            Controls.Add(MessageLabel);
            Controls.Add(_iconPanel);
            Controls.Add(_headerPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BaseDialog";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Dialog";
            Shown += BaseDialog_Shown;
            _headerPanel.ResumeLayout(false);
            _headerPanel.PerformLayout();
            ButtonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
