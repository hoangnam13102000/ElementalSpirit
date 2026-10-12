namespace ElementalSpirit.Presentation.Dialogs
{
    partial class InformationDialog
    {
        private void InitializeComponent()
        {
            SuspendLayout();
            Controls.Remove(SecondaryButton);
            SecondaryButton.Visible = false;
            PrimaryButton.Location = new System.Drawing.Point(195, 182);
            ClientSize = new System.Drawing.Size(500, 260);
            Name = "InformationDialog";
            Text = "Information";
            ResumeLayout(false);
        }
    }
}
