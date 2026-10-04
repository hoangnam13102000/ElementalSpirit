using System;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Localization;

namespace ElementalSpirit.Presentation.Forms
{
    public sealed class PlayerNameDialog : Form
    {
        private readonly ILocalizationService _localization;
        private readonly TextBox _nameTextBox;
        private readonly Label _errorLabel;

        public string PlayerName => _nameTextBox.Text.Trim();

        public PlayerNameDialog(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            Text = _localization.Translate("playerName.title");
            ClientSize = new Size(420, 185);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(24, 18, 36);

            var prompt = new Label
            {
                Text = _localization.Translate("playerName.prompt"),
                Location = new Point(24, 20),
                Size = new Size(372, 28),
                ForeColor = Color.FromArgb(230, 220, 255),
                Font = new Font("Segoe UI", 10f)
            };

            _nameTextBox = new TextBox
            {
                Location = new Point(24, 54),
                Size = new Size(372, 28),
                MaxLength = 24,
                Font = new Font("Segoe UI", 11f)
            };

            _errorLabel = new Label
            {
                Location = new Point(24, 85),
                Size = new Size(372, 22),
                ForeColor = Color.LightCoral,
                Visible = false
            };

            var startButton = new Button
            {
                Text = _localization.Translate("playerName.start"),
                Location = new Point(220, 120),
                Size = new Size(176, 38),
                DialogResult = DialogResult.None,
                BackColor = Color.FromArgb(55, 90, 140),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            startButton.FlatAppearance.BorderColor = Color.FromArgb(150, 190, 240);
            startButton.Click += (_, _) =>
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
            };

            var cancelButton = new Button
            {
                Text = _localization.Translate("playerName.cancel"),
                Location = new Point(24, 120),
                Size = new Size(176, 38),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(65, 55, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(150, 140, 170);

            Controls.AddRange(new Control[] { prompt, _nameTextBox, _errorLabel, startButton, cancelButton });
            AcceptButton = startButton;
            CancelButton = cancelButton;
        }
    }
}
