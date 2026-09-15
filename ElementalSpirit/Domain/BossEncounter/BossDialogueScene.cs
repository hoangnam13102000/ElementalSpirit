namespace ElementalSpirit.Domain.BossEncounter
{
    using System.Collections.Generic;

    public sealed class BossDialogueScene
    {
        public string SceneName { get; }
        public string? BackgroundImageName { get; }
        public IReadOnlyList<BossDialogueLine> Lines { get; }

        public BossDialogueScene(
            string sceneName,
            string? backgroundImageName,
            IReadOnlyList<BossDialogueLine> lines)
        {
            SceneName = sceneName ?? throw new System.ArgumentNullException(nameof(sceneName));
            BackgroundImageName = backgroundImageName;
            Lines = lines ?? throw new System.ArgumentNullException(nameof(lines));
        }
    }
}