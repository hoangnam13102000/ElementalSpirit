using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ElementalSpirit.Domain.Leaderboard;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Dialogs;
using ElementalSpirit.Services;

namespace ElementalSpirit.Presentation.Forms
{
    public sealed class LeaderboardForm : Form
    {
        private static readonly Color WindowColor = Color.FromArgb(17, 20, 31);
        private static readonly Color SurfaceColor = Color.FromArgb(25, 30, 44);
        private static readonly Color HeaderColor = Color.FromArgb(33, 41, 58);
        private static readonly Color AccentColor = Color.FromArgb(221, 177, 83);
        private static readonly Color PrimaryTextColor = Color.FromArgb(242, 239, 230);
        private static readonly Color SecondaryTextColor = Color.FromArgb(154, 165, 183);
        private static readonly Font TopRankFont = new("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font RankFont = new("Segoe UI", 10f, FontStyle.Regular);

        private readonly ILocalizationService _localization;
        private readonly DataGridView _grid;
        private readonly Label _emptyLabel;
        private readonly Label _entryCountLabel;
        private readonly Button _clearButton;
        private readonly LeaderboardService _leaderboardService = new();

        public LeaderboardForm(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            Text = _localization.Translate("leaderboard.title");
            ClientSize = new Size(960, 560);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = WindowColor;
            Padding = new Padding(1);

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 96,
                BackColor = WindowColor,
                Padding = new Padding(24, 15, 18, 12)
            };
            var accent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 4,
                BackColor = AccentColor
            };
            var titleLabel = new Label
            {
                Text = _localization.Translate("leaderboard.title").ToUpperInvariant(),
                Location = new Point(24, 17),
                AutoSize = true,
                ForeColor = PrimaryTextColor,
                Font = new Font("Georgia", 19f, FontStyle.Bold)
            };
            var subtitleLabel = new Label
            {
                Text = _localization.Translate("leaderboard.subtitle"),
                Location = new Point(26, 56),
                AutoSize = true,
                ForeColor = SecondaryTextColor,
                Font = new Font("Segoe UI", 9.5f)
            };
            var closeButton = new Button
            {
                Text = "×",
                Size = new Size(38, 38),
                Location = new Point(960 - 56, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = WindowColor,
                ForeColor = SecondaryTextColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 19f, FontStyle.Regular),
                UseVisualStyleBackColor = false,
                TabStop = false
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.FlatAppearance.MouseOverBackColor = HeaderColor;
            closeButton.Click += (_, _) => Close();
            header.Controls.AddRange(new Control[] { accent, titleLabel, subtitleLabel, closeButton });

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                RowHeadersVisible = false,
                ColumnHeadersHeight = 44,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 44 },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SurfaceColor,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(44, 52, 68),
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ForeColor = PrimaryTextColor,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EditMode = DataGridViewEditMode.EditProgrammatically
            };
            _grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = SurfaceColor,
                ForeColor = PrimaryTextColor,
                SelectionBackColor = Color.FromArgb(45, 57, 77),
                SelectionForeColor = PrimaryTextColor,
                Font = new Font("Segoe UI", 10.5f),
                Padding = new Padding(10, 0, 8, 0)
            };
            _grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(29, 35, 50),
                ForeColor = PrimaryTextColor,
                SelectionBackColor = Color.FromArgb(45, 57, 77),
                SelectionForeColor = PrimaryTextColor
            };
            _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = HeaderColor,
                ForeColor = SecondaryTextColor,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Padding = new Padding(10, 0, 8, 0),
                SelectionBackColor = HeaderColor,
                SelectionForeColor = SecondaryTextColor
            };
            _grid.RowHeadersDefaultCellStyle.SelectionBackColor = SurfaceColor;
            _grid.Columns.Add("rank", _localization.Translate("leaderboard.rank"));
            _grid.Columns.Add("player", _localization.Translate("leaderboard.player"));
            _grid.Columns.Add("stages", _localization.Translate("leaderboard.stages"));
            _grid.Columns.Add("gold", _localization.Translate("leaderboard.gold"));
            _grid.Columns.Add("date", _localization.Translate("leaderboard.date"));
            _grid.Columns[0].FillWeight = 58;
            _grid.Columns[1].FillWeight = 112;
            _grid.Columns[2].FillWeight = 104;
            _grid.Columns[3].FillWeight = 112;
            _grid.Columns[4].FillWeight = 150;
            _grid.Columns[0].MinimumWidth = 76;
            _grid.Columns[1].MinimumWidth = 135;
            _grid.Columns[2].MinimumWidth = 135;
            _grid.Columns[3].MinimumWidth = 140;
            _grid.Columns[4].MinimumWidth = 190;
            _grid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _grid.Columns[0].Width = 100;
            _grid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _grid.Columns[3].DefaultCellStyle.Format = "N0";
            _grid.Columns[4].DefaultCellStyle.ForeColor = SecondaryTextColor;
            _grid.ScrollBars = ScrollBars.Vertical;
            _grid.CellFormatting += Grid_CellFormatting;

            _emptyLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = SecondaryTextColor,
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                Visible = false
            };

            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 74,
                BackColor = WindowColor,
                Padding = new Padding(22, 12, 22, 14)
            };
            var footerDivider = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(48, 57, 74)
            };
            _entryCountLabel = new Label
            {
                Text = string.Format(_localization.Translate("leaderboard.entries"), 0),
                AutoSize = true,
                Location = new Point(22, 27),
                ForeColor = SecondaryTextColor,
                Font = new Font("Segoe UI", 9f)
            };
            var actionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                Width = 380,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = WindowColor,
                Padding = new Padding(0, 4, 0, 0)
            };
            var closeFooterButton = CreateFooterButton(
                _localization.Translate("leaderboard.close"),
                Point.Empty,
                new Size(120, 38),
                HeaderColor,
                PrimaryTextColor);
            closeFooterButton.Click += (_, _) => Close();
            _clearButton = CreateFooterButton(
                _localization.Translate("leaderboard.clear"),
                Point.Empty,
                new Size(230, 38),
                Color.FromArgb(75, 39, 43),
                Color.FromArgb(239, 157, 151));
            _clearButton.FlatAppearance.BorderColor = Color.FromArgb(103, 57, 61);
            _clearButton.Click += ClearButton_Click;
            _clearButton.Margin = new Padding(0, 0, 8, 0);
            closeFooterButton.Margin = Padding.Empty;
            actionsPanel.Controls.AddRange(new Control[] { _clearButton, closeFooterButton });
            footer.Controls.AddRange(new Control[] { footerDivider, _entryCountLabel, actionsPanel });
            _clearButton.BringToFront();
            closeFooterButton.BringToFront();

            var content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18, 0, 18, 0),
                BackColor = SurfaceColor
            };
            content.Controls.Add(_grid);
            content.Controls.Add(_emptyLabel);

            Controls.Add(content);
            Controls.Add(footer);
            Controls.Add(header);
            LoadEntries();
        }

        private static Button CreateFooterButton(
            string text,
            Point location,
            Size size,
            Color backColor,
            Color foreColor)
        {
            var button = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(65, 76, 97);
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(backColor.R + 12, 255),
                Math.Min(backColor.G + 12, 255),
                Math.Min(backColor.B + 12, 255));
            return button;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var border = new Pen(Color.FromArgb(62, 73, 96));
            e.Graphics.DrawRectangle(border, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
        }

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
            {
                e.Value = $"#{e.RowIndex + 1}";
                e.FormattingApplied = true;
                e.CellStyle.ForeColor = e.RowIndex switch
                {
                    0 => AccentColor,
                    1 => Color.FromArgb(197, 207, 222),
                    2 => Color.FromArgb(196, 143, 106),
                    _ => SecondaryTextColor
                };
                e.CellStyle.Font = e.RowIndex < 3 ? TopRankFont : RankFont;
            }
            else if (e.ColumnIndex == 1 && e.RowIndex == 0)
            {
                e.CellStyle.Font = TopRankFont;
                e.CellStyle.ForeColor = PrimaryTextColor;
            }
            else if (e.ColumnIndex == 3)
            {
                e.CellStyle.ForeColor = Color.FromArgb(230, 194, 111);
            }
        }

        private void LoadEntries()
        {
            try
            {
                IReadOnlyList<LeaderboardEntry> entries = _leaderboardService.GetEntries();
                _grid.Rows.Clear();
                for (int i = 0; i < entries.Count; i++)
                {
                    LeaderboardEntry entry = entries[i];
                    _grid.Rows.Add(
                        i + 1,
                        entry.PlayerName,
                        entry.ClearedStageCount,
                        entry.TotalGoldEarned,
                        entry.RecordedAtUtc.ToLocalTime().ToString("g"));
                }

                _emptyLabel.Text = _localization.Translate("leaderboard.empty");
                _emptyLabel.Visible = entries.Count == 0;
                _clearButton.Enabled = entries.Count > 0;
                _entryCountLabel.Text = string.Format(
                    _localization.Translate("leaderboard.entries"),
                    entries.Count);
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException ||
                ex is InvalidDataException)
            {
                System.Diagnostics.Debug.WriteLine($"[LeaderboardForm] Could not load leaderboard: {ex}");
                MessageBox.Show(
                    this,
                    _localization.Translate("leaderboard.loadError"),
                    _localization.Translate("leaderboard.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _grid.CellFormatting -= Grid_CellFormatting;
            base.Dispose(disposing);
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            using var confirmationDialog = new ConfirmationDialog(
                _localization.Translate("leaderboard.clear"),
                _localization.Translate("leaderboard.clear.confirm"),
                _localization.Translate("leaderboard.clear.confirmButton"),
                _localization.Translate("settings.button.cancel"));
            if (confirmationDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _leaderboardService.Clear();
                LoadEntries();
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine($"[LeaderboardForm] Could not clear leaderboard: {ex}");
                MessageBox.Show(
                    this,
                    _localization.Translate("leaderboard.clearError"),
                    _localization.Translate("leaderboard.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
