namespace ElementalSpirit.Presentation.Forms
{
    partial class GuideAndTeamForm
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Panel _divider;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _titleLabel = new System.Windows.Forms.Label();
            _subtitleLabel = new System.Windows.Forms.Label();
            _divider = new System.Windows.Forms.Panel();
            _instructionsHeading = new System.Windows.Forms.Label();
            _instructionsLabel = new System.Windows.Forms.Label();
            _teamHeading = new System.Windows.Forms.Label();
            _teamGrid = new System.Windows.Forms.DataGridView();
            _closeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)_teamGrid).BeginInit();
            SuspendLayout();
            //
            // _titleLabel
            //
            _titleLabel.AutoSize = true;
            _titleLabel.Font = new System.Drawing.Font("Georgia", 20F, System.Drawing.FontStyle.Bold);
            _titleLabel.ForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _titleLabel.Location = new System.Drawing.Point(28, 20);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new System.Drawing.Size(216, 31);
            _titleLabel.TabIndex = 0;
            _titleLabel.Text = "Guide and team";
            //
            // _subtitleLabel
            //
            _subtitleLabel.AutoSize = true;
            _subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            _subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _subtitleLabel.Location = new System.Drawing.Point(30, 61);
            _subtitleLabel.Name = "_subtitleLabel";
            _subtitleLabel.Size = new System.Drawing.Size(216, 17);
            _subtitleLabel.TabIndex = 1;
            _subtitleLabel.Text = "Controls, instructions, and contributors";
            //
            // _divider
            //
            _divider.BackColor = System.Drawing.Color.FromArgb(221, 177, 83);
            _divider.Location = new System.Drawing.Point(28, 94);
            _divider.Name = "_divider";
            _divider.Size = new System.Drawing.Size(704, 2);
            _divider.TabIndex = 2;
            //
            // _instructionsHeading
            //
            _instructionsHeading.AutoSize = true;
            _instructionsHeading.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            _instructionsHeading.ForeColor = System.Drawing.Color.FromArgb(221, 177, 83);
            _instructionsHeading.Location = new System.Drawing.Point(28, 112);
            _instructionsHeading.Name = "_instructionsHeading";
            _instructionsHeading.Size = new System.Drawing.Size(105, 20);
            _instructionsHeading.TabIndex = 3;
            _instructionsHeading.Text = "How to play";
            //
            // _instructionsLabel
            //
            _instructionsLabel.BackColor = System.Drawing.Color.FromArgb(25, 30, 44);
            _instructionsLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            _instructionsLabel.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            _instructionsLabel.ForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _instructionsLabel.Location = new System.Drawing.Point(30, 146);
            _instructionsLabel.Name = "_instructionsLabel";
            _instructionsLabel.Padding = new System.Windows.Forms.Padding(16);
            _instructionsLabel.Size = new System.Drawing.Size(700, 238);
            _instructionsLabel.TabIndex = 4;
            _instructionsLabel.Text = "Instructions";
            //
            // _teamHeading
            //
            _teamHeading.AutoSize = true;
            _teamHeading.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            _teamHeading.ForeColor = System.Drawing.Color.FromArgb(221, 177, 83);
            _teamHeading.Location = new System.Drawing.Point(28, 402);
            _teamHeading.Name = "_teamHeading";
            _teamHeading.Size = new System.Drawing.Size(42, 20);
            _teamHeading.TabIndex = 5;
            _teamHeading.Text = "Team";
            //
            // _teamGrid
            //
            _teamGrid.AllowUserToAddRows = false;
            _teamGrid.AllowUserToDeleteRows = false;
            _teamGrid.AllowUserToResizeColumns = false;
            _teamGrid.AllowUserToResizeRows = false;
            _teamGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            _teamGrid.BackgroundColor = System.Drawing.Color.FromArgb(25, 30, 44);
            _teamGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            _teamGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            _teamGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            _teamGrid.ColumnHeadersHeight = 36;
            _teamGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _teamGrid.EnableHeadersVisualStyles = false;
            _teamGrid.GridColor = System.Drawing.Color.FromArgb(44, 52, 68);
            _teamGrid.Location = new System.Drawing.Point(28, 438);
            _teamGrid.MultiSelect = false;
            _teamGrid.Name = "_teamGrid";
            _teamGrid.ReadOnly = true;
            _teamGrid.RowHeadersVisible = false;
            _teamGrid.RowTemplate.Height = 28;
            _teamGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            _teamGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _teamGrid.Size = new System.Drawing.Size(704, 150);
            _teamGrid.TabIndex = 6;
            _teamGrid.Columns.Add("name", "Member");
            _teamGrid.Columns.Add("studentId", "Student ID");
            _teamGrid.Columns.Add("className", "Class");
            _teamGrid.Columns[0].FillWeight = 125F;
            _teamGrid.Columns[1].FillWeight = 100F;
            _teamGrid.Columns[2].FillWeight = 110F;
            _teamGrid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(33, 41, 58);
            _teamGrid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _teamGrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _teamGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(33, 41, 58);
            _teamGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _teamGrid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(25, 30, 44);
            _teamGrid.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _teamGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(25, 30, 44);
            _teamGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _teamGrid.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            _teamGrid.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(8, 0, 4, 0);
            _teamGrid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(29, 35, 50);
            _teamGrid.Rows.Add("Hoàng Trung Nam", "44.01.104.145", "48.01.CNTT.B");
            _teamGrid.Rows.Add("Lê Thị Vân Anh", "46.01.103.007", "48.01.TIN.SPA");
            _teamGrid.Rows.Add("Lê Thanh Tú", "50.01.104.172", "50.01.CNTT.B");
            _teamGrid.Rows.Add("Hồ Thị Mỹ Thuận", "50.01.104.157", "50.01.CNTT.A");
            //
            // _closeButton
            //
            _closeButton.BackColor = System.Drawing.Color.FromArgb(33, 41, 58);
            _closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            _closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            _closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _closeButton.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            _closeButton.ForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _closeButton.Location = new System.Drawing.Point(612, 600);
            _closeButton.Name = "_closeButton";
            _closeButton.Size = new System.Drawing.Size(120, 38);
            _closeButton.TabIndex = 7;
            _closeButton.Text = "Close";
            _closeButton.UseVisualStyleBackColor = false;
            _closeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(65, 76, 97);
            _closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 57, 74);
            //
            // GuideAndTeamForm
            //
            AcceptButton = _closeButton;
            BackColor = System.Drawing.Color.FromArgb(17, 20, 31);
            ClientSize = new System.Drawing.Size(760, 650);
            Controls.Add(_closeButton);
            Controls.Add(_teamGrid);
            Controls.Add(_teamHeading);
            Controls.Add(_instructionsLabel);
            Controls.Add(_instructionsHeading);
            Controls.Add(_divider);
            Controls.Add(_subtitleLabel);
            Controls.Add(_titleLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GuideAndTeamForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Guide and team";
            ((System.ComponentModel.ISupportInitialize)_teamGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
