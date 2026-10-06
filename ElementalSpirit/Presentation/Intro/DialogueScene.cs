using System.Collections.Generic;

namespace ElementalSpirit.Presentation.Intro
{
    public class DialogueScene
    {
        public string SceneName { get; }
        public string? BackgroundImageName { get; }
        public List<DialogueLine> Lines { get; }

        public DialogueScene(
            string sceneName,
            string? backgroundImageName,
            List<DialogueLine> lines)
        {
            SceneName = sceneName;
            BackgroundImageName = backgroundImageName;
            Lines = lines;
        }
    }
}