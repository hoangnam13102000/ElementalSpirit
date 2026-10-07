using System.ComponentModel;
using System.Drawing;

namespace ElementalSpirit.Presentation.Dialogs
{
    public sealed partial class ConfirmationDialog : BaseDialog
    {
        public ConfirmationDialog() : base()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
        }

        public ConfirmationDialog(
            string title,
            string message,
            string confirmText,
            string cancelText)
            : base(
                title,
                message,
                confirmText,
                cancelText,
                Color.FromArgb(220, 35, 40),
                Color.FromArgb(220, 35, 40))
        {
            InitializeComponent();
        }
    }
}
