namespace ElementalSpirit.Domain.BossEncounter
{
    using ElementalSpirit.Localization;
    using System;

    public sealed class BossDialogueLine
    {
        public BossDialogueSpeaker Speaker { get; }
        public string Text { get; }
        public string? OnScreenCaption { get; }

        public BossDialogueLine(
            BossDialogueSpeaker speaker,
            string text,
            string? onScreenCaption = null)
        {
            Speaker = speaker;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            OnScreenCaption = onScreenCaption;
        }

        public static BossDialogueLine FromLocalization(
            ILocalizationService localization,
            BossDialogueSpeaker speaker,
            string textKey,
            string? captionKey = null)
        {
            if (localization == null) throw new ArgumentNullException(nameof(localization));
            string text = localization.Translate(textKey);
            string? caption = captionKey != null ? localization.Translate(captionKey) : null;
            return new BossDialogueLine(speaker, text, caption);
        }
    }
}