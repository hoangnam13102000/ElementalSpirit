using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Localization;

namespace ElementalSpirit.Presentation.Forms
{
    public sealed partial class PlayerNameDialog : Form
    {
        private readonly ILocalizationService _localization = null!;

        public string PlayerName => _nameTextBox.Text.Trim();

        public PlayerNameDialog()
        {
            _localization = null!;
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _localization = LocalizationManager.Instance;
            ApplyTranslations();
        }

        public PlayerNameDialog(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            ApplyTranslations();
        }

        private void ApplyTranslations()
        {
            Text = _localization.Translate("playerName.title");
            _promptLabel.Text = _localization.Translate("playerName.prompt");
            _startButton.Text = _localization.Translate("playerName.start");
            _cancelButton.Text = _localization.Translate("playerName.cancel");
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PlayerName))
            {
                _errorLabel.Text = _localization.Translate("playerName.required");
                _errorLabel.Visible = true;
                _nameTextBox.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void _nameTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
