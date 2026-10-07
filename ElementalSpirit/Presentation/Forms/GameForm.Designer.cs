namespace ElementalSpirit.Presentation.Forms
{
    partial class GameForm
    {
        private System.ComponentModel.IContainer components;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            //
            // GameForm
            //
            BackColor = System.Drawing.Color.FromArgb(12, 16, 28);
            ClientSize = new System.Drawing.Size(1280, 720);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            Name = "GameForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Elemental Spirit";
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            Resize += OnFormResize;
            FormClosing += OnFormClosing;
            ResumeLayout(false);
        }
    }
}
