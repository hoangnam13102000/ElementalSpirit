namespace ElementalSpirit.Presentation.Forms
{
    partial class LeaderboardForm
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Panel _header;
        private System.Windows.Forms.Panel _accent;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.Label _subtitleLabel;
        private System.Windows.Forms.Button _closeButton;
        private System.Windows.Forms.Panel _footer;
        private System.Windows.Forms.Panel _footerDivider;
        private System.Windows.Forms.FlowLayoutPanel _actionsPanel;
        private System.Windows.Forms.Button _closeFooterButton;
        private System.Windows.Forms.Panel _content;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _header = new System.Windows.Forms.Panel();
            _accent = new System.Windows.Forms.Panel();
            _titleLabel = new System.Windows.Forms.Label();
            _subtitleLabel = new System.Windows.Forms.Label();
            _closeButton = new System.Windows.Forms.Button();
            _footer = new System.Windows.Forms.Panel();
            _footerDivider = new System.Windows.Forms.Panel();
            _entryCountLabel = new System.Windows.Forms.Label();
            _actionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            _clearButton = new System.Windows.Forms.Button();
            _closeFooterButton = new System.Windows.Forms.Button();
            _content = new System.Windows.Forms.Panel();
            _grid = new System.Windows.Forms.DataGridView();
            _emptyLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)_grid).BeginInit();
            _header.SuspendLayout();
            _footer.SuspendLayout();
            _actionsPanel.SuspendLayout();
            _content.SuspendLayout();
            SuspendLayout();
            //
            // _header
            //
            _header.BackColor = System.Drawing.Color.FromArgb(17, 20, 31);
            _header.Dock = System.Windows.Forms.DockStyle.Top;
            _header.Height = 96;
            _header.Name = "_header";
            _header.Padding = new System.Windows.Forms.Padding(24, 15, 18, 12);
            _header.Controls.Add(_accent);
            _header.Controls.Add(_titleLabel);
            _header.Controls.Add(_subtitleLabel);
            _header.Controls.Add(_closeButton);
            //
            // _accent
            //
            _accent.BackColor = System.Drawing.Color.FromArgb(221, 177, 83);
            _accent.Dock = System.Windows.Forms.DockStyle.Left;
            _accent.Name = "_accent";
            _accent.Width = 4;
            //
            // _titleLabel
            //
            _titleLabel.AutoSize = true;
            _titleLabel.Font = new System.Drawing.Font("Georgia", 19F, System.Drawing.FontStyle.Bold);
            _titleLabel.ForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _titleLabel.Location = new System.Drawing.Point(24, 17);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Text = "LEADERBOARD";
            //
            // _subtitleLabel
            //
            _subtitleLabel.AutoSize = true;
            _subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            _subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _subtitleLabel.Location = new System.Drawing.Point(26, 56);
            _subtitleLabel.Name = "_subtitleLabel";
            _subtitleLabel.Text = "Player records";
            //
            // _closeButton
            //
            _closeButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            _closeButton.BackColor = System.Drawing.Color.FromArgb(17, 20, 31);
            _closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _closeButton.Font = new System.Drawing.Font("Segoe UI", 19F);
            _closeButton.ForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _closeButton.Location = new System.Drawing.Point(904, 18);
            _closeButton.Name = "_closeButton";
            _closeButton.Size = new System.Drawing.Size(38, 38);
            _closeButton.TabStop = false;
            _closeButton.Text = "×";
            _closeButton.UseVisualStyleBackColor = false;
            _closeButton.FlatAppearance.BorderSize = 0;
            _closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(33, 41, 58);
            _closeButton.Click += CloseButton_Click;
            //
            // _footer
            //
            _footer.BackColor = System.Drawing.Color.FromArgb(17, 20, 31);
            _footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            _footer.Height = 74;
            _footer.Name = "_footer";
            _footer.Padding = new System.Windows.Forms.Padding(22, 12, 22, 14);
            _footer.Controls.Add(_footerDivider);
            _footer.Controls.Add(_entryCountLabel);
            _footer.Controls.Add(_actionsPanel);
            //
            // _footerDivider
            //
            _footerDivider.BackColor = System.Drawing.Color.FromArgb(48, 57, 74);
            _footerDivider.Dock = System.Windows.Forms.DockStyle.Top;
            _footerDivider.Height = 1;
            _footerDivider.Name = "_footerDivider";
            //
            // _entryCountLabel
            //
            _entryCountLabel.AutoSize = true;
            _entryCountLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            _entryCountLabel.ForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _entryCountLabel.Location = new System.Drawing.Point(22, 27);
            _entryCountLabel.Name = "_entryCountLabel";
            _entryCountLabel.Text = "0 entries";
            //
            // _actionsPanel
            //
            _actionsPanel.BackColor = System.Drawing.Color.FromArgb(17, 20, 31);
            _actionsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            _actionsPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            _actionsPanel.Name = "_actionsPanel";
            _actionsPanel.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            _actionsPanel.Width = 380;
            _actionsPanel.WrapContents = false;
            _actionsPanel.Controls.Add(_clearButton);
            _actionsPanel.Controls.Add(_closeFooterButton);
            //
            // _clearButton
            //
            _clearButton.BackColor = System.Drawing.Color.FromArgb(75, 39, 43);
            _clearButton.ForeColor = System.Drawing.Color.FromArgb(239, 157, 151);
            _clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _clearButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _clearButton.UseVisualStyleBackColor = false;
            _clearButton.Cursor = System.Windows.Forms.Cursors.Hand;
            _clearButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(103, 57, 61);
            _clearButton.FlatAppearance.BorderSize = 1;
            _clearButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(87, 51, 55);
            _clearButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            _clearButton.Size = new System.Drawing.Size(230, 38);
            _clearButton.Name = "_clearButton";
            _clearButton.Text = "Clear records";
            _clearButton.Click += ClearButton_Click;
            //
            // _closeFooterButton
            //
            _closeFooterButton.BackColor = System.Drawing.Color.FromArgb(33, 41, 58);
            _closeFooterButton.ForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _closeFooterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            _closeFooterButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _closeFooterButton.UseVisualStyleBackColor = false;
            _closeFooterButton.Cursor = System.Windows.Forms.Cursors.Hand;
            _closeFooterButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(65, 76, 97);
            _closeFooterButton.FlatAppearance.BorderSize = 1;
            _closeFooterButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(45, 53, 70);
            _closeFooterButton.Margin = System.Windows.Forms.Padding.Empty;
            _closeFooterButton.Size = new System.Drawing.Size(120, 38);
            _closeFooterButton.Name = "_closeFooterButton";
            _closeFooterButton.Text = "Close";
            _closeFooterButton.Click += CloseButton_Click;
            //
            // _content
            //
            _content.BackColor = System.Drawing.Color.FromArgb(25, 30, 44);
            _content.Dock = System.Windows.Forms.DockStyle.Fill;
            _content.Name = "_content";
            _content.Padding = new System.Windows.Forms.Padding(18, 0, 18, 0);
            _content.Controls.Add(_grid);
            _content.Controls.Add(_emptyLabel);
            //
            // _grid
            //
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AllowUserToResizeColumns = false;
            _grid.AllowUserToResizeRows = false;
            _grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            _grid.BackgroundColor = System.Drawing.Color.FromArgb(25, 30, 44);
            _grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            _grid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            _grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            _grid.ColumnHeadersHeight = 44;
            _grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            _grid.EnableHeadersVisualStyles = false;
            _grid.GridColor = System.Drawing.Color.FromArgb(44, 52, 68);
            _grid.Dock = System.Windows.Forms.DockStyle.Fill;
            _grid.MultiSelect = false;
            _grid.Name = "_grid";
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.RowTemplate.Height = 44;
            _grid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            _grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _grid.Columns.Add("rank", "Rank");
            _grid.Columns.Add("player", "Player");
            _grid.Columns.Add("stages", "Stages");
            _grid.Columns.Add("gold", "Gold");
            _grid.Columns.Add("date", "Date");
            _grid.Columns[0].FillWeight = 58F;
            _grid.Columns[1].FillWeight = 112F;
            _grid.Columns[2].FillWeight = 104F;
            _grid.Columns[3].FillWeight = 112F;
            _grid.Columns[4].FillWeight = 150F;
            _grid.Columns[0].MinimumWidth = 76;
            _grid.Columns[1].MinimumWidth = 135;
            _grid.Columns[2].MinimumWidth = 135;
            _grid.Columns[3].MinimumWidth = 140;
            _grid.Columns[4].MinimumWidth = 190;
            _grid.CellFormatting += Grid_CellFormatting;
            _grid.Columns[0].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            _grid.Columns[0].Width = 100;
            _grid.Columns[0].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns[2].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns[3].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            _grid.Columns[3].DefaultCellStyle.Format = "N0";
            _grid.Columns[4].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _grid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(25, 30, 44);
            _grid.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(45, 57, 77);
            _grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _grid.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            _grid.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(10, 0, 8, 0);
            _grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(29, 35, 50);
            _grid.AlternatingRowsDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(45, 57, 77);
            _grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(242, 239, 230);
            _grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(33, 41, 58);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            _grid.ColumnHeadersDefaultCellStyle.Padding = new System.Windows.Forms.Padding(10, 0, 8, 0);
            _grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(33, 41, 58);
            _grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _grid.RowHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(25, 30, 44);
            //
            // _emptyLabel
            //
            _emptyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            _emptyLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            _emptyLabel.ForeColor = System.Drawing.Color.FromArgb(154, 165, 183);
            _emptyLabel.Name = "_emptyLabel";
            _emptyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _emptyLabel.Visible = false;
            //
            // LeaderboardForm
            //
            BackColor = System.Drawing.Color.FromArgb(17, 20, 31);
            ClientSize = new System.Drawing.Size(960, 560);
            Controls.Add(_content);
            Controls.Add(_footer);
            Controls.Add(_header);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LeaderboardForm";
            Padding = new System.Windows.Forms.Padding(1);
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Leaderboard";
            ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
            _content.ResumeLayout(false);
            _actionsPanel.ResumeLayout(false);
            _footer.ResumeLayout(false);
            _footer.PerformLayout();
            _header.ResumeLayout(false);
            _header.PerformLayout();
            ResumeLayout(false);
        }

    }
}
