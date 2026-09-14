using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ElementalSpirit.Presentation.Dialogs
{
    public abstract class BaseDialog : Form
    {
        protected readonly Label MessageLabel;
        protected readonly Panel ButtonPanel;

        protected BaseDialog(
            string title,
            string message,
            string primaryButtonText,
            string secondaryButtonText,
            Color primaryButtonColor,
            Color accentColor)
        {
            Text = title;
            ClientSize = new Size(500, 260);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;
            BackColor = Color.White;
            DoubleBuffered = true;
            Region = CreateRoundedRegion(ClientSize.Width, ClientSize.Height, 16);

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.White
            };
            header.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.FromArgb(185, 198, 220));
                e.Graphics.DrawLine(pen, 24, header.Height - 1, header.Width - 24, header.Height - 1);
            };

            var titleLabel = new Label
            {
                Text = title,
                Location = new Point(68, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 16f, FontStyle.Regular),
                ForeColor = Color.FromArgb(20, 25, 35)
            };
            header.Controls.Add(titleLabel);

            var iconPanel = new Panel
            {
                Location = new Point(28, 90),
                Size = new Size(72, 72),
                BackColor = Color.FromArgb(255, 242, 242)
            };
            iconPanel.Paint += (_, e) => DrawIcon(e.Graphics, iconPanel.ClientRectangle, accentColor);

            MessageLabel = new Label
            {
                Text = message,
                Location = new Point(126, 108),
                Size = new Size(340, 48),
                AutoEllipsis = false,
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(25, 30, 40)
            };

            ButtonPanel = new Panel
            {
                Location = new Point(28, 194),
                Size = new Size(ClientSize.Width - 56, 52)
            };

            var secondary = CreateButton(
                secondaryButtonText,
                new Point(ButtonPanel.Width - 256, 6),
                110,
                40);
            secondary.BackColor = Color.FromArgb(241, 245, 250);
            secondary.ForeColor = Color.FromArgb(35, 45, 60);
            secondary.DialogResult = DialogResult.Cancel;

            var primary = CreateButton(
                primaryButtonText,
                new Point(ButtonPanel.Width - 134, 6),
                110,
                40);
            primary.BackColor = primaryButtonColor;
            primary.ForeColor = Color.White;
            primary.DialogResult = DialogResult.OK;

            ButtonPanel.Controls.Add(secondary);
            ButtonPanel.Controls.Add(primary);
            Controls.Add(header);
            Controls.Add(iconPanel);
            Controls.Add(MessageLabel);
            Controls.Add(ButtonPanel);

            AcceptButton = primary;
            CancelButton = secondary;
            Shown += (_, _) => ActiveControl = primary;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = CreateRoundedPath(ClientSize.Width - 2, ClientSize.Height - 2, 16, 1);
            using var pen = new Pen(Color.FromArgb(165, 180, 205), 1.5f);
            e.Graphics.DrawPath(pen, path);
        }

        private static Button CreateButton(string text, Point location, int width, int height)
        {
            var button = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, height),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f),
                Cursor = Cursors.Hand,
                TabStop = true
            };
            button.FlatAppearance.BorderSize = 0;
            button.Region = CreateRoundedRegion(width, height, 10);
            return button;
        }

        private static Region CreateRoundedRegion(int width, int height, int radius)
        {
            return new Region(CreateRoundedPath(width, height, radius, 0));
        }

        private static GraphicsPath CreateRoundedPath(
            int width,
            int height,
            int radius,
            int offset)
        {
            var path = new GraphicsPath();
            float diameter = radius * 2f;
            float x = offset;
            float y = offset;
            float pathWidth = width - offset * 2;
            float pathHeight = height - offset * 2;
            path.AddArc(x, y, diameter, diameter, 180, 90);
            path.AddArc(x + pathWidth - diameter, y, diameter, diameter, 270, 90);
            path.AddArc(
                x + pathWidth - diameter,
                y + pathHeight - diameter,
                diameter,
                diameter,
                0,
                90);
            path.AddArc(x, y + pathHeight - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void DrawIcon(Graphics graphics, Rectangle bounds, Color accentColor)
        {
            using var brush = new SolidBrush(accentColor);
            using var pen = new Pen(Color.White, 5f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            float centerY = bounds.Top + bounds.Height / 2f;
            graphics.FillRoundedRectangle(brush, bounds.Left, bounds.Top, bounds.Width, bounds.Height, 18);
            graphics.DrawLine(pen, bounds.Left + 22, centerY, bounds.Right - 22, centerY);
            graphics.DrawLine(pen, bounds.Right - 32, centerY - 10, bounds.Right - 20, centerY);
            graphics.DrawLine(pen, bounds.Right - 32, centerY + 10, bounds.Right - 20, centerY);
        }
    }

    internal static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(
            this Graphics graphics,
            Brush brush,
            float x,
            float y,
            float width,
            float height,
            float radius)
        {
            using var path = new GraphicsPath();
            float diameter = radius * 2f;
            path.AddArc(x, y, diameter, diameter, 180, 90);
            path.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
            path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
            path.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            graphics.FillPath(brush, path);
        }
    }
}
