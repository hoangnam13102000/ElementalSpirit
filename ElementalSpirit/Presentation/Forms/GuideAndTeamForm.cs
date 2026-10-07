using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Localization;

namespace ElementalSpirit.Presentation.Forms
{
    public sealed partial class GuideAndTeamForm : Form
    {
        private static readonly Color WindowColor = Color.FromArgb(17, 20, 31);
        private static readonly Color SurfaceColor = Color.FromArgb(25, 30, 44);
        private static readonly Color HeaderColor = Color.FromArgb(33, 41, 58);
        private static readonly Color AccentColor = Color.FromArgb(221, 177, 83);
        private static readonly Color PrimaryTextColor = Color.FromArgb(242, 239, 230);
        private static readonly Color SecondaryTextColor = Color.FromArgb(154, 165, 183);

        private Label _titleLabel = null!;
        private Label _subtitleLabel = null!;
        private Label _instructionsHeading = null!;
        private Label _instructionsLabel = null!;
        private Label _teamHeading = null!;
        private DataGridView _teamGrid = null!;
        private Button _closeButton = null!;

        public GuideAndTeamForm()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            ApplyTranslations(LocalizationManager.Instance);
        }

        public GuideAndTeamForm(ILocalizationService localization)
        {
            ArgumentNullException.ThrowIfNull(localization);
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            ApplyTranslations(localization);
        }

        private void ApplyTranslations(ILocalizationService localization)
        {
            Text = localization.Translate("guide.title");
            _titleLabel.Text = localization.Translate("guide.title");
            _subtitleLabel.Text = localization.Translate("guide.subtitle");
            _instructionsHeading.Text = localization.Translate("guide.howToPlay");
            _instructionsLabel.Text = localization.Translate("guide.instructions");
            _teamHeading.Text = localization.Translate("guide.team");
            _teamGrid.Columns[0].HeaderText = localization.Translate("guide.member");
            _teamGrid.Columns[1].HeaderText = localization.Translate("guide.studentId");
            _teamGrid.Columns[2].HeaderText = localization.Translate("guide.class");
            _closeButton.Text = localization.Translate("guide.close");
        }
    }
}
