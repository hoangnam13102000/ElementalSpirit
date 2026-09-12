using System.Collections.Generic;

namespace ElementalSpirit.Presentation.Intro
{
    public static class IntroData
    {
        private const string BG_WORLD = "Intro/Intro_World.png";
        private const string BG_FOUR_SPIRITS = "Intro/Intro_FourSpirits.png";
        private const string BG_CRACKED_SYMBOL = "Intro/Intro_CrackedSymbol.png";
        private const string BG_VILLAGE = "Intro/Intro_Village.png";
        private const string BG_TERRA = "Intro/Intro_Terra.png";
        private const string BG_FOREST_ENTRANCE = "Intro/Intro_ForestEntrance.png";

        private const string MUSIC_MAIN = "Intro_MainTheme";
        private const string MUSIC_SPIRITS = "Intro_Spirits";
        private const string MUSIC_CORRUPTION = "Intro_Corruption";
        private const string MUSIC_ARIN = "Intro_Arin";
        private const string MUSIC_TERRA = "Intro_Terra";
        private const string MUSIC_JOURNEY = "Intro_JourneyBegins";

        public static List<DialogueScene> CreateAllScenes()
        {
            return new List<DialogueScene>
            {
                CreateScene1_TheBeginning(),
                CreateScene2_TheFourSpirits(),
                CreateScene3_TheCorruption(),
                CreateScene4_Arin(),
                CreateScene5_TheFirstSpirit(),
                CreateScene6_TheJourneyBegins()
            };
        }

        private static DialogueScene CreateScene1_TheBeginning()
        {
            return new DialogueScene(
                sceneName: "The Beginning",
                backgroundImageName: BG_WORLD,
                musicTrackName: MUSIC_MAIN,
                lines: new List<DialogueLine>
                {
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s1.l1.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s1.l2.text", "intro.s1.l2.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s1.l3.text")
                });
        }

        private static DialogueScene CreateScene2_TheFourSpirits()
        {
            return new DialogueScene(
                sceneName: "The Four Spirits",
                backgroundImageName: BG_FOUR_SPIRITS,
                musicTrackName: MUSIC_SPIRITS,
                lines: new List<DialogueLine>
                {
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s2.l1.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s2.l2.text"),
                    new DialogueLine(Speaker.Narrator, "intro.s2.l3.text", "intro.s2.l3.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s2.l4.text", "intro.s2.l4.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s2.l5.text", "intro.s2.l5.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s2.l6.text", "intro.s2.l6.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s2.l7.text")
                });
        }

        private static DialogueScene CreateScene3_TheCorruption()
        {
            return new DialogueScene(
                sceneName: "The Corruption",
                backgroundImageName: BG_CRACKED_SYMBOL,
                musicTrackName: MUSIC_CORRUPTION,
                lines: new List<DialogueLine>
                {
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s3.l1.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s3.l2.text"),
                    new DialogueLine(Speaker.Narrator, "intro.s3.l3.text"),
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s3.l4.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s3.l5.text"),
                    new DialogueLine(Speaker.Narrator, "intro.s3.l6.text")
                });
        }

        private static DialogueScene CreateScene4_Arin()
        {
            return new DialogueScene(
                sceneName: "Arin",
                backgroundImageName: BG_VILLAGE,
                musicTrackName: MUSIC_ARIN,
                lines: new List<DialogueLine>
                {
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s4.l1.caption"),
                    new DialogueLine(Speaker.VillageElder, "intro.s4.l2.text"),
                    new DialogueLine(Speaker.Arin, "intro.s4.l3.text"),
                    new DialogueLine(Speaker.VillageElder, "intro.s4.l4.text"),
                    new DialogueLine(Speaker.Arin, "intro.s4.l5.text"),
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s4.l6.caption")
                });
        }

        private static DialogueScene CreateScene5_TheFirstSpirit()
        {
            return new DialogueScene(
                sceneName: "The First Spirit",
                backgroundImageName: BG_TERRA,
                musicTrackName: MUSIC_TERRA,
                lines: new List<DialogueLine>
                {
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s5.l1.caption"),
                    new DialogueLine(Speaker.Terra, "intro.s5.l2.text"),
                    new DialogueLine(Speaker.Arin, "intro.s5.l3.text"),
                    new DialogueLine(Speaker.Terra, "intro.s5.l4.text"),
                    new DialogueLine(Speaker.Arin, "intro.s5.l5.text"),
                    new DialogueLine(Speaker.Terra, "intro.s5.l6.text"),
                    new DialogueLine(Speaker.Arin, "intro.s5.l7.text"),
                    new DialogueLine(Speaker.Terra, "intro.s5.l8.text"),
                    new DialogueLine(Speaker.Arin, "intro.s5.l9.text"),
                    new DialogueLine(Speaker.Terra, "intro.s5.l10.text"),
                    new DialogueLine(Speaker.Arin, "intro.s5.l11.text"),
                    new DialogueLine(Speaker.Terra, "intro.s5.l12.text"),
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s5.l13.caption")
                });
        }

        private static DialogueScene CreateScene6_TheJourneyBegins()
        {
            return new DialogueScene(
                sceneName: "The Journey Begins",
                backgroundImageName: BG_FOREST_ENTRANCE,
                musicTrackName: MUSIC_JOURNEY,
                lines: new List<DialogueLine>
                {
                    new DialogueLine(Speaker.Narrator, "intro.s6.l1.text"),
                    new DialogueLine(Speaker.VillageElder, "intro.s6.l2.text"),
                    new DialogueLine(Speaker.Arin, "intro.s6.l3.text"),
                    new DialogueLine(Speaker.Arin, "intro.s6.l4.text"),
                    new DialogueLine(Speaker.Narrator, "intro.s6.l5.text"),
                    new DialogueLine(Speaker.OnScreenText, "", "intro.s6.l6.caption"),
                    new DialogueLine(Speaker.Narrator, "intro.s6.l7.text")
                });
        }
    }
}