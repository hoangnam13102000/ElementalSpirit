using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ElementalSpirit.Presentation
{
    internal static class WindowDisplayMode
    {
        private sealed record WindowState(FormBorderStyle BorderStyle, Rectangle Bounds, FormWindowState State);

        private static readonly Dictionary<Form, WindowState> WindowStates = new();

        public static void SetFullscreen(Form form, bool enabled)
        {
            if (enabled)
            {
                if (!WindowStates.ContainsKey(form))
                {
                    WindowStates.Add(
                        form,
                        new WindowState(form.FormBorderStyle, form.Bounds, form.WindowState));
                }

                form.WindowState = FormWindowState.Normal;
                form.FormBorderStyle = FormBorderStyle.None;
                Rectangle screenBounds = Screen.FromControl(form).Bounds;
                form.Location = screenBounds.Location;
                form.Size = screenBounds.Size;
                return;
            }

            if (!WindowStates.Remove(form, out WindowState? previous))
                return;

            form.WindowState = FormWindowState.Normal;
            form.FormBorderStyle = previous.BorderStyle;
            form.Bounds = previous.Bounds;
            form.WindowState = previous.State;
        }
    }
}
