using ElementalSpirit.Localization;

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
        private readonly string _textKey;
        private readonly string? _captionKey;

        public DialogueLine(Speaker speaker, string textKey, string? captionKey = null)
        {
            Speaker = speaker;
            _textKey = textKey;
            _captionKey = captionKey;
        }

        public string Text =>
            string.IsNullOrEmpty(_textKey) ? "" : LocalizationManager.Instance.Translate(_textKey);

        public string? OnScreenCaption =>
            _captionKey == null ? null : LocalizationManager.Instance.Translate(_captionKey);
    }
}