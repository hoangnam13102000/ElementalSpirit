using System;
using System.Windows.Forms;
using ElementalSpirit.Presentation.Forms;

namespace ElementalSpirit
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainMenuForm());
        }
    }
}