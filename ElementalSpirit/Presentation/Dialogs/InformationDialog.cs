using System.ComponentModel;
using System.Drawing;

namespace ElementalSpirit.Presentation.Dialogs
{
    public sealed partial class InformationDialog : BaseDialog
    {
        public InformationDialog() : base()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
        }

        public InformationDialog(string title, string message, string okText)
            : base(
                title,
                message,
                okText,
                string.Empty,
                Color.FromArgb(45, 105, 190),
                Color.FromArgb(45, 105, 190))
        {
            InitializeComponent();
        }
    }
}
