using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ElementalSpirit.Domain.Leaderboard;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Dialogs;
using ElementalSpirit.Services;

namespace ElementalSpirit.Presentation.Forms
{
    public sealed partial class LeaderboardForm : Form
    {
        private static readonly Color WindowColor = Color.FromArgb(17, 20, 31);
        private static readonly Color SurfaceColor = Color.FromArgb(25, 30, 44);
        private static readonly Color HeaderColor = Color.FromArgb(33, 41, 58);
        private static readonly Color AccentColor = Color.FromArgb(221, 177, 83);
        private static readonly Color PrimaryTextColor = Color.FromArgb(242, 239, 230);
        private static readonly Color SecondaryTextColor = Color.FromArgb(154, 165, 183);
        private static readonly Font TopRankFont = new("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font RankFont = new("Segoe UI", 10f, FontStyle.Regular);

        private readonly ILocalizationService _localization = null!;
        private DataGridView _grid = null!;
        private Label _emptyLabel = null!;
        private Label _entryCountLabel = null!;
        private Button _clearButton = null!;
        private readonly LeaderboardService _leaderboardService = new();

        public LeaderboardForm()
        {
            _localization = null!;
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _localization = LocalizationManager.Instance;
            InitializeRuntime();
        }

        public LeaderboardForm(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            InitializeRuntime();
        }

        private void InitializeRuntime()
        {
            Text = _localization.Translate("leaderboard.title");
            _titleLabel.Text = _localization.Translate("leaderboard.title").ToUpperInvariant();
            _subtitleLabel.Text = _localization.Translate("leaderboard.subtitle");
            _grid.Columns[0].HeaderText = _localization.Translate("leaderboard.rank");
            _grid.Columns[1].HeaderText = _localization.Translate("leaderboard.player");
            _grid.Columns[2].HeaderText = _localization.Translate("leaderboard.stages");
            _grid.Columns[3].HeaderText = _localization.Translate("leaderboard.gold");
            _grid.Columns[4].HeaderText = _localization.Translate("leaderboard.date");
            _entryCountLabel.Text = string.Format(_localization.Translate("leaderboard.entries"), 0);
            _closeFooterButton.Text = _localization.Translate("leaderboard.close");
            _clearButton.Text = _localization.Translate("leaderboard.clear");
            LoadEntries();
        }

        private void CloseButton_Click(object? sender, EventArgs e)
        {
            Close();
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
