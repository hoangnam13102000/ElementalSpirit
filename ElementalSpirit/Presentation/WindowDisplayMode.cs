using System;
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
            if (form == null || form.IsDisposed) return;

            try
            {
                // === TOÀN BỘ CODE CŨ NẰM TRONG NÀY ===
                if (enabled)
                {
                    if (!WindowStates.ContainsKey(form))
                    {
                        WindowStates[form] = new WindowState(form.FormBorderStyle, form.Bounds, form.WindowState);
                    }

                    form.WindowState = FormWindowState.Normal;
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.Bounds = Screen.FromControl(form).Bounds;
                    return;
                }

                if (WindowStates.Remove(form, out WindowState? previous))
                {
                    form.FormBorderStyle = previous.BorderStyle;
                    form.Bounds = previous.Bounds;
                    form.WindowState = previous.State;
                }
                else
                {
                    form.FormBorderStyle = FormBorderStyle.Sizable;
                    form.Size = new Size(1280, 720);
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.WindowState = FormWindowState.Normal;
                }
            }
            catch (Exception)
            {
                // Phương án dự phòng: Nếu WinForms phát sinh lỗi vẽ lại giao diện,
                // ép form trở về chế độ Normal để game không bị treo/mất hiển thị.
                form.WindowState = FormWindowState.Normal;
            }
            /*if (enabled)
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
        } */
        }
    }
}
