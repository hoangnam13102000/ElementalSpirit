namespace ElementalSpirit.Presentation.Forms
{
    partial class GuideAndTeamForm
    {
        private System.ComponentModel.IContainer components = null!;
        private System.Windows.Forms.Panel _divider;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            _titleLabel = new Label();
            _subtitleLabel = new Label();
            _divider = new Panel();
            _instructionsHeading = new Label();
            _instructionsLabel = new Label();
            _teamHeading = new Label();
            _teamGrid = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            _closeButton = new Button();
            ((System.ComponentModel.ISupportInitialize)_teamGrid).BeginInit();
            SuspendLayout();
            // 
            // _titleLabel
            // 
            _titleLabel.AutoSize = true;
            _titleLabel.Font = new Font("Georgia", 20F, FontStyle.Bold);
            _titleLabel.ForeColor = Color.FromArgb(242, 239, 230);
            _titleLabel.Location = new Point(28, 20);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new Size(346, 46);
            _titleLabel.TabIndex = 0;
            _titleLabel.Text = "Guide and team";
            _titleLabel.Click += _titleLabel_Click;
            // 
            // _subtitleLabel
            // 
            _subtitleLabel.AutoSize = true;
            _subtitleLabel.Font = new Font("Segoe UI", 9.5F);
            _subtitleLabel.ForeColor = Color.FromArgb(154, 165, 183);
            _subtitleLabel.Location = new Point(30, 61);
            _subtitleLabel.Name = "_subtitleLabel";
            _subtitleLabel.Size = new Size(339, 25);
            _subtitleLabel.TabIndex = 1;
            _subtitleLabel.Text = "Controls, instructions, and contributors";
            // 
            // _divider
            // 
            _divider.BackColor = Color.FromArgb(221, 177, 83);
            _divider.Location = new Point(28, 94);
            _divider.Name = "_divider";
            _divider.Size = new Size(704, 2);
            _divider.TabIndex = 2;
            // 
            // _instructionsHeading
            // 
            _instructionsHeading.AutoSize = true;
            _instructionsHeading.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            _instructionsHeading.ForeColor = Color.FromArgb(221, 177, 83);
            _instructionsHeading.Location = new Point(28, 112);
            _instructionsHeading.Name = "_instructionsHeading";
            _instructionsHeading.Size = new Size(139, 30);
            _instructionsHeading.TabIndex = 3;
            _instructionsHeading.Text = "How to play";
            // 
            // _instructionsLabel
            // 
            _instructionsLabel.BackColor = Color.FromArgb(25, 30, 44);
            _instructionsLabel.BorderStyle = BorderStyle.FixedSingle;
            _instructionsLabel.Font = new Font("Segoe UI", 10.5F);
            _instructionsLabel.ForeColor = Color.FromArgb(242, 239, 230);
            _instructionsLabel.Location = new Point(30, 146);
            _instructionsLabel.Name = "_instructionsLabel";
            _instructionsLabel.Padding = new Padding(16);
            _instructionsLabel.Size = new Size(700, 238);
            _instructionsLabel.TabIndex = 4;
            _instructionsLabel.Text = "Instructions";
            _instructionsLabel.Click += _instructionsLabel_Click;
            // 
            // _teamHeading
            // 
            _teamHeading.AutoSize = true;
            _teamHeading.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            _teamHeading.ForeColor = Color.FromArgb(221, 177, 83);
            _teamHeading.Location = new Point(28, 402);
            _teamHeading.Name = "_teamHeading";
            _teamHeading.Size = new Size(68, 30);
            _teamHeading.TabIndex = 5;
            _teamHeading.Text = "Team";
            // 
            // _teamGrid
            // 
            _teamGrid.AllowUserToAddRows = false;
            _teamGrid.AllowUserToDeleteRows = false;
            _teamGrid.AllowUserToResizeColumns = false;
            _teamGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(29, 35, 50);
            _teamGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            _teamGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _teamGrid.BackgroundColor = Color.FromArgb(25, 30, 44);
            _teamGrid.BorderStyle = BorderStyle.None;
            _teamGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _teamGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(33, 41, 58);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(154, 165, 183);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(33, 41, 58);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(154, 165, 183);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            _teamGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            _teamGrid.ColumnHeadersHeight = 36;
            _teamGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _teamGrid.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3 });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(25, 30, 44);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.5F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(242, 239, 230);
            dataGridViewCellStyle3.Padding = new Padding(8, 0, 4, 0);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(25, 30, 44);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(242, 239, 230);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            _teamGrid.DefaultCellStyle = dataGridViewCellStyle3;
            _teamGrid.EnableHeadersVisualStyles = false;
            _teamGrid.GridColor = Color.FromArgb(44, 52, 68);
            _teamGrid.Location = new Point(28, 438);
            _teamGrid.MultiSelect = false;
            _teamGrid.Name = "_teamGrid";
            _teamGrid.ReadOnly = true;
            _teamGrid.RowHeadersVisible = false;
            _teamGrid.RowHeadersWidth = 62;
            _teamGrid.RowTemplate.Height = 28;
            _teamGrid.ScrollBars = ScrollBars.None;
            _teamGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _teamGrid.Size = new Size(704, 173);
            _teamGrid.TabIndex = 6;
            _teamGrid.CellContentClick += _teamGrid_CellContentClick;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Member";
            dataGridViewTextBoxColumn1.MinimumWidth = 8;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Student ID";
            dataGridViewTextBoxColumn2.MinimumWidth = 8;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Class";
            dataGridViewTextBoxColumn3.MinimumWidth = 8;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // _closeButton
            // 
            _closeButton.BackColor = Color.FromArgb(33, 41, 58);
            _closeButton.Cursor = Cursors.Hand;
            _closeButton.DialogResult = DialogResult.OK;
            _closeButton.FlatAppearance.BorderColor = Color.FromArgb(65, 76, 97);
            _closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(48, 57, 74);
            _closeButton.FlatStyle = FlatStyle.Flat;
            _closeButton.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _closeButton.ForeColor = Color.FromArgb(242, 239, 230);
            _closeButton.Location = new Point(610, 617);
            _closeButton.Name = "_closeButton";
            _closeButton.Size = new Size(120, 38);
            _closeButton.TabIndex = 7;
            _closeButton.Text = "Close";
            _closeButton.UseVisualStyleBackColor = false;
            // 
            // GuideAndTeamForm
            // 
            AcceptButton = _closeButton;
            BackColor = Color.FromArgb(17, 20, 31);
            ClientSize = new Size(760, 678);
            Controls.Add(_closeButton);
            Controls.Add(_teamGrid);
            Controls.Add(_teamHeading);
            Controls.Add(_instructionsLabel);
            Controls.Add(_instructionsHeading);
            Controls.Add(_divider);
            Controls.Add(_subtitleLabel);
            Controls.Add(_titleLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GuideAndTeamForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "  ";
            Load += GuideAndTeamForm_Load;
            ((System.ComponentModel.ISupportInitialize)_teamGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    }
}
