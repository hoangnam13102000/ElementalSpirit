using System.Drawing;

namespace ElementalSpirit.Presentation.Dialogs
{
    public sealed class InformationDialog : BaseDialog
    {
        public InformationDialog(string title, string message, string okText)
            : base(
                title,
                message,
                okText,
                string.Empty,
                Color.FromArgb(45, 105, 190),
                Color.FromArgb(45, 105, 190))
        {
            ButtonPanel.Controls[0].Visible = false;
            ButtonPanel.Controls[1].Location = new Point(ButtonPanel.Width - 134, 6);
        }
    }
}
