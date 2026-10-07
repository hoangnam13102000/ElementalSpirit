using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ElementalSpirit.Presentation.Dialogs
{
    public partial class BaseDialog : Form
    {
        protected Label MessageLabel = null!;
        protected Panel ButtonPanel = null!;
        protected Button SecondaryButton = null!;
        protected Button PrimaryButton = null!;

        private Label _titleLabel = null!;
        private Panel _headerPanel = null!;
        private Panel _iconPanel = null!;
        private Color _accentColor = Color.FromArgb(45, 105, 190);

        public BaseDialog()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            ApplyRoundedRegions();
        }

        protected BaseDialog(
            string title,
            string message,
            string primaryButtonText,
            string secondaryButtonText,
            Color primaryButtonColor,
            Color accentColor)
            : this()
        {
            Text = title;
            _titleLabel.Text = title;
            MessageLabel.Text = message;
            PrimaryButton.Text = primaryButtonText;
            SecondaryButton.Text = secondaryButtonText;
            PrimaryButton.BackColor = primaryButtonColor;
            _accentColor = accentColor;
            PrimaryButton.DialogResult = DialogResult.OK;
            SecondaryButton.DialogResult = DialogResult.Cancel;
            AcceptButton = PrimaryButton;
            CancelButton = SecondaryButton;
            _iconPanel.Invalidate();
        }

        private void ApplyRoundedRegions()
        {
            Region = CreateRoundedRegion(ClientSize.Width, ClientSize.Height, 16);
            SecondaryButton.Region = CreateRoundedRegion(
                SecondaryButton.Width,
                SecondaryButton.Height,
                10);
            PrimaryButton.Region = CreateRoundedRegion(
                PrimaryButton.Width,
                PrimaryButton.Height,
                10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = CreateRoundedPath(ClientSize.Width - 2, ClientSize.Height - 2, 16, 1);
            using var pen = new Pen(Color.FromArgb(165, 180, 205), 1.5f);
            e.Graphics.DrawPath(pen, path);
        }

        private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(Color.FromArgb(185, 198, 220));
            e.Graphics.DrawLine(pen, 24, _headerPanel.Height - 1, _headerPanel.Width - 24, _headerPanel.Height - 1);
        }

        private void IconPanel_Paint(object? sender, PaintEventArgs e)
        {
            DrawIcon(e.Graphics, _iconPanel.ClientRectangle, _accentColor);
        }

        private void BaseDialog_Shown(object? sender, EventArgs e)
        {
            ActiveControl = PrimaryButton;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
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
