using System;
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
        private readonly ILocalizationService _localization;
        private readonly string _textKey;
        private readonly string? _captionKey;

        public DialogueLine(ILocalizationService localization, Speaker speaker, string textKey, string? captionKey = null)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            Speaker = speaker;
            _textKey = textKey;
            _captionKey = captionKey;
        }

        public string Text =>
            string.IsNullOrEmpty(_textKey) ? "" : _localization.Translate(_textKey);

        public string? OnScreenCaption =>
            _captionKey == null ? null : _localization.Translate(_captionKey);
    }
}