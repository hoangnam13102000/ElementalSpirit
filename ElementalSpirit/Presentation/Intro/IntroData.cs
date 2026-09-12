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
                    new DialogueLine(Speaker.OnScreenText, "", "Long before mankind..."),
                    new DialogueLine(Speaker.Narrator, "Long before the first stone of civilization was laid, before kings drew borders on maps, before magic became a discipline studied in towers... a world was born from the harmony of four elements.", "This was Elaria."),
                    new DialogueLine(Speaker.Narrator, "A world of ancient forests and endless oceans, of smoldering volcanoes and winds that carried songs across continents.")
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
                    new DialogueLine(Speaker.OnScreenText, "", "Four spirits guarded Elaria."),
                    new DialogueLine(Speaker.Narrator, "They were not gods who demanded worship. They were not kings who demanded loyalty. They were the world's own breath made manifest — four Ancient Elemental Spirits, born from the essence of Elaria itself."),
                    new DialogueLine(Speaker.Narrator, "Terra, whose hands shaped the mountains.", "Earth"),
                    new DialogueLine(Speaker.Narrator, "Aqua, whose tears filled the oceans.", "Water"),
                    new DialogueLine(Speaker.Narrator, "Ignis, whose fire warmed the cold earth.", "Fire"),
                    new DialogueLine(Speaker.Narrator, "Zephyr, whose winds carried life across the world.", "Wind"),
                    new DialogueLine(Speaker.Narrator, "Together, they wove a balance — four elements in eternal harmony.")
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
                    new DialogueLine(Speaker.OnScreenText, "", "The balance was broken."),
                    new DialogueLine(Speaker.Narrator, "One night, without warning... the Symbol cracked."),
                    new DialogueLine(Speaker.Narrator, "The spirits vanished. Their power, no longer guided, began to corrupt the world."),
                    new DialogueLine(Speaker.OnScreenText, "", "Something... awakened."),
                    new DialogueLine(Speaker.Narrator, "Forests grew hostile. Rivers raged. Volcanoes awakened. And shadow creatures emerged from the corruption."),
                    new DialogueLine(Speaker.Narrator, "No one knew what happened. The spirits were simply... gone.")
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
                    new DialogueLine(Speaker.OnScreenText, "", "Arin — Elemental Mage Apprentice"),
                    new DialogueLine(Speaker.VillageElder, "Arin, come inside. The forest tonight... it is not peaceful."),
                    new DialogueLine(Speaker.Arin, "Elder... I hear something. A voice. From deep within the trees."),
                    new DialogueLine(Speaker.VillageElder, "It is only the wind, child. Go home."),
                    new DialogueLine(Speaker.Arin, "No... it is someone. Asking for help."),
                    new DialogueLine(Speaker.OnScreenText, "", "The Spirit Bond awakens...")
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
                    new DialogueLine(Speaker.OnScreenText, "", "Terra — Spirit of Earth"),
                    new DialogueLine(Speaker.Terra, "Young mage... you hear me."),
                    new DialogueLine(Speaker.Arin, "Who are you? Where am I?"),
                    new DialogueLine(Speaker.Terra, "I am Terra. And this... is all that remains of me."),
                    new DialogueLine(Speaker.Arin, "You are wounded. What happened?"),
                    new DialogueLine(Speaker.Terra, "Something older than memory tore us apart. Our power now poisons this land."),
                    new DialogueLine(Speaker.Arin, "How can I help?"),
                    new DialogueLine(Speaker.Terra, "Enter the Earth Forest. Find my scattered essence. Prove your worth. Then... I may lend you strength."),
                    new DialogueLine(Speaker.Arin, "I will come."),
                    new DialogueLine(Speaker.Terra, "Be careful. The truth... is darker than you imagine. And the Primordial—"),
                    new DialogueLine(Speaker.Arin, "Primordial? What is that?"),
                    new DialogueLine(Speaker.Terra, "...you will know. In time."),
                    new DialogueLine(Speaker.OnScreenText, "", "Something has been hidden...")
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
                    new DialogueLine(Speaker.Narrator, "Dawn broke pale and uncertain. Arin stood at the edge of the Earth Forest. Staff in hand. And a crystal that now glowed with faint green light."),
                    new DialogueLine(Speaker.VillageElder, "You do not have to do this. You are still young."),
                    new DialogueLine(Speaker.Arin, "I know. But if not me... then who?"),
                    new DialogueLine(Speaker.Arin, "I am no chosen one. I am just... someone who will try."),
                    new DialogueLine(Speaker.Narrator, "And so the first step was taken. Not by a prophesied hero. But by a young mage who refused to look away."),
                    new DialogueLine(Speaker.OnScreenText, "", "Your journey begins..."),
                    new DialogueLine(Speaker.Narrator, "LOCATION: Earth Forest\nOBJECTIVE: Find the source of corruption")
                });
        }
    }
}