namespace ElementalSpirit.Presentation.Intro
{
    public enum Speaker
    {
        Narrator,
        Arin,
        Terra,
        VillageElder,
        OnScreenText
    }

    public class DialogueLine
    {
        public Speaker Speaker { get; }
        public string Text { get; }
        public string? OnScreenCaption { get; }

        public DialogueLine(Speaker speaker, string text, string? onScreenCaption = null)
        {
            Speaker = speaker;
            Text = text;
            OnScreenCaption = onScreenCaption;
        }
    }
}