namespace ElementalSpirit.Presentation.Forms
{
    partial class IntroForm
    {
        private System.ComponentModel.IContainer components;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            //
            // IntroForm
            //
            BackColor = System.Drawing.Color.FromArgb(8, 10, 18);
            ClientSize = new System.Drawing.Size(1280, 720);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            KeyPreview = true;
            Name = "IntroForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Elemental Spirit";
            KeyDown += OnKeyPressed;
            MouseClick += OnMouseClicked;
            ResumeLayout(false);
        }
    }
}
