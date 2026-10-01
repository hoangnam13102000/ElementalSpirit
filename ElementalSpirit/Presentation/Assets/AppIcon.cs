using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ElementalSpirit.Presentation.Assets
{
    public static class AppIcon
    {
        private static readonly Icon SharedIcon = new(
            Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "Logo", "ElementalSpirit.ico"));

        public static void ApplyTo(Form form)
        {
            ArgumentNullException.ThrowIfNull(form);
            form.Icon = SharedIcon;
        }
    }
}
