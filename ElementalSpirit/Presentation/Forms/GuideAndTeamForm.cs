using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Localization;

namespace ElementalSpirit.Presentation.Forms
{
    public sealed class GuideAndTeamForm : Form
    {
        private static readonly Color WindowColor = Color.FromArgb(17, 20, 31);
        private static readonly Color SurfaceColor = Color.FromArgb(25, 30, 44);
        private static readonly Color HeaderColor = Color.FromArgb(33, 41, 58);
        private static readonly Color AccentColor = Color.FromArgb(221, 177, 83);
        private static readonly Color PrimaryTextColor = Color.FromArgb(242, 239, 230);
        private static readonly Color SecondaryTextColor = Color.FromArgb(154, 165, 183);

        public GuideAndTeamForm(ILocalizationService localization)
        {
            ArgumentNullException.ThrowIfNull(localization);
            Text = localization.Translate("guide.title");
            ClientSize = new Size(760, 650);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = WindowColor;

            var title = new Label
            {
                Text = localization.Translate("guide.title"),
                Location = new Point(28, 20),
                AutoSize = true,
                ForeColor = PrimaryTextColor,
                Font = new Font("Georgia", 20f, FontStyle.Bold)
            };
            var subtitle = new Label
            {
                Text = localization.Translate("guide.subtitle"),
                Location = new Point(30, 61),
                AutoSize = true,
                ForeColor = SecondaryTextColor,
                Font = new Font("Segoe UI", 9.5f)
            };
            var divider = new Panel
            {
                Location = new Point(28, 94),
                Size = new Size(704, 2),
                BackColor = AccentColor
            };
            var instructionsHeading = CreateHeading(localization.Translate("guide.howToPlay"), 28, 112);
            var instructions = new Label
            {
                Text = localization.Translate("guide.instructions"),
                Location = new Point(30, 146),
                Size = new Size(700, 238),
                ForeColor = PrimaryTextColor,
                Font = new Font("Segoe UI", 10.5f),
                BackColor = SurfaceColor,
                Padding = new Padding(16),
                BorderStyle = BorderStyle.FixedSingle
            };
            var teamHeading = CreateHeading(localization.Translate("guide.team"), 28, 402);
            var teamGrid = CreateTeamGrid(localization);
            teamGrid.Location = new Point(28, 438);
            teamGrid.Size = new Size(704, 150);

            var closeButton = new Button
            {
                Text = localization.Translate("guide.close"),
                Size = new Size(120, 38),
                Location = new Point(612, 600),
                BackColor = HeaderColor,
                ForeColor = PrimaryTextColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderColor = Color.FromArgb(65, 76, 97);
            closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(48, 57, 74);
            AcceptButton = closeButton;

            Controls.AddRange(new Control[]
            {
                title, subtitle, divider, instructionsHeading, instructions,
                teamHeading, teamGrid, closeButton
            });
        }

        private static Label CreateHeading(string text, int x, int y) => new()
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            ForeColor = AccentColor,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold)
        };

        private static DataGridView CreateTeamGrid(ILocalizationService localization)
        {
            var grid = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                RowHeadersVisible = false,
                ColumnHeadersHeight = 36,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 28 },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SurfaceColor,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(44, 52, 68),
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ScrollBars = ScrollBars.None
            };
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = HeaderColor,
                ForeColor = SecondaryTextColor,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                SelectionBackColor = HeaderColor,
                SelectionForeColor = SecondaryTextColor
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = SurfaceColor,
                ForeColor = PrimaryTextColor,
                SelectionBackColor = SurfaceColor,
                SelectionForeColor = PrimaryTextColor,
                Font = new Font("Segoe UI", 9.5f),
                Padding = new Padding(8, 0, 4, 0)
            };
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(29, 35, 50);
            grid.Columns.Add("name", localization.Translate("guide.member"));
            grid.Columns.Add("studentId", localization.Translate("guide.studentId"));
            grid.Columns.Add("className", localization.Translate("guide.class"));
            grid.Columns[0].FillWeight = 125;
            grid.Columns[1].FillWeight = 100;
            grid.Columns[2].FillWeight = 110;

            grid.Rows.Add("Hoàng Trung Nam", "44.01.104.145", "48.01.CNTT.B");
            grid.Rows.Add("Lê Thị Vân Anh", "46.01.103.007", "48.01.TIN.SPA");
            grid.Rows.Add("Lê Thanh Tú", localization.Translate("guide.notProvided"), "50.01.CNTT.B");
            grid.Rows.Add("Hồ Thị Mỹ Thuận", "50.01.104.157", "50.01.CNTT.A");
            return grid;
        }
    }
}
