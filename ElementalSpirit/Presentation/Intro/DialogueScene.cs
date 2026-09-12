using System.Collections.Generic;

namespace ElementalSpirit.Presentation.Intro
{
    public class DialogueScene
    {
        public string SceneName { get; }
        public string? BackgroundImageName { get; }
        public string? MusicTrackName { get; }
        public List<DialogueLine> Lines { get; }

        public DialogueScene(
            string sceneName,
            string? backgroundImageName,
            string? musicTrackName,
            List<DialogueLine> lines)
        {
            SceneName = sceneName;
            BackgroundImageName = backgroundImageName;
            MusicTrackName = musicTrackName;
            Lines = lines;
        }
    }
}