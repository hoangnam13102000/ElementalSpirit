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
        protected Button SecondaryButton = null!;
        protected Button PrimaryButton = null!;

        private Label _titleLabel = null!;
        private Panel _headerPanel = null!;
        private Panel _iconPanel = null!;
        private Color _accentColor = Color.FromArgb(45, 105, 190);
        private readonly bool _hasSecondaryButton;

        public BaseDialog() : this(true)
        {
        }

        protected BaseDialog(bool hasSecondaryButton)
        {
            _hasSecondaryButton = hasSecondaryButton;
            InitializeComponent();
            SecondaryButton = new Button
            {
                BackColor = Color.FromArgb(241, 245, 250),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(35, 45, 60),
                Location = new Point(216, 200),
                Name = "SecondaryButton",
                Size = new Size(110, 40),
                TabIndex = 0,
                Text = "Cancel",
                UseVisualStyleBackColor = false,
                Visible = hasSecondaryButton
            };
            SecondaryButton.FlatAppearance.BorderSize = 0;
            if (hasSecondaryButton)
            {
                Controls.Add(SecondaryButton);
                CancelButton = SecondaryButton;
            }

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
        }

        protected BaseDialog(
            string title,
            string message,
            string primaryButtonText,
            string secondaryButtonText,
            Color primaryButtonColor,
            Color accentColor,
            bool hasSecondaryButton = true)
            : this(hasSecondaryButton)
        {
            Text = title;
            _titleLabel.Text = title;
            MessageLabel.Text = message;
            PrimaryButton.Text = primaryButtonText;
            PrimaryButton.BackColor = primaryButtonColor;
            _accentColor = accentColor;
            PrimaryButton.DialogResult = DialogResult.None;
            AcceptButton = PrimaryButton;
            if (_hasSecondaryButton)
            {
                SecondaryButton.Text = secondaryButtonText;
                SecondaryButton.DialogResult = DialogResult.None;
                CancelButton = SecondaryButton;
                SecondaryButton.Visible = !string.IsNullOrEmpty(secondaryButtonText);
                SecondaryButton.Enabled = !string.IsNullOrEmpty(secondaryButtonText);
                SecondaryButton.Click += OnSecondaryButtonClick;
            }

            PrimaryButton.Visible = !string.IsNullOrEmpty(primaryButtonText);
            PrimaryButton.Enabled = !string.IsNullOrEmpty(primaryButtonText);
            PrimaryButton.Click += OnPrimaryButtonClick;
            _iconPanel.Invalidate();
            RecalculateLayout();
        }

        private void OnSecondaryButtonClick(object? sender, EventArgs e)
        {
            CloseWithResult(DialogResult.Cancel);
        }

        private void OnPrimaryButtonClick(object? sender, EventArgs e)
        {
            CloseWithResult(DialogResult.OK);
        }

        private void CloseWithResult(DialogResult result)
        {
            DialogResult = result;
            Close();
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

        private void RecalculateLayout()
        {
            MessageLabel.AutoSize = false;
            MessageLabel.AutoEllipsis = false;
            MessageLabel.MaximumSize = new Size(GetMessageContentWidth(), 0);
            MessageLabel.Width = GetMessageContentWidth();

            int textHeight = TextRenderer.MeasureText(
                MessageLabel.Text,
                MessageLabel.Font,
                new Size(MessageLabel.Width, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix).Height + 12;

            MessageLabel.Height = Math.Max(48, textHeight);
            MessageLabel.Location = new Point(126, 108);

            int buttonTop = MessageLabel.Bottom + 26;
            UpdateButtonLayout();

            int buttonBottom = buttonTop + 46;
            int dialogHeight = buttonBottom + 24;
            if (dialogHeight > ClientSize.Height)
                ClientSize = new Size(ClientSize.Width, dialogHeight);
        }

        private void UpdateButtonLayout()
        {
            if (_hasSecondaryButton && SecondaryButton!.Visible && PrimaryButton.Visible)
            {
                int spacing = 18;
                int totalWidth = SecondaryButton.Width + PrimaryButton.Width + spacing;
                int startX = (ClientSize.Width - totalWidth) / 2;
                int buttonTop = MessageLabel.Bottom + 26;
                SecondaryButton.Location = new Point(startX, buttonTop);
                PrimaryButton.Location = new Point(startX + SecondaryButton.Width + spacing, buttonTop);
                return;
            }

            Button? visibleButton = PrimaryButton.Visible
                ? PrimaryButton
                : _hasSecondaryButton ? SecondaryButton : null;
            if (visibleButton is not null && visibleButton.Visible)
            {
                visibleButton.Location = new Point(
                    (ClientSize.Width - visibleButton.Width) / 2,
                    MessageLabel.Bottom + 26);
            }
        }

        private static int GetMessageContentWidth()
        {
            return 340;
        }

        private void BaseDialog_Shown(object? sender, EventArgs e)
        {
            RecalculateLayout();
            ActiveControl = PrimaryButton;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
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
