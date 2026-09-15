namespace ElementalSpirit.Domain.BossEncounter
{
    using ElementalSpirit.Localization;
    using System.Collections.Generic;

    /// <summary>
    /// Nhà cung cấp dữ liệu tĩnh cho cuộc gặp boss Gorgon.
    /// Chứa toàn bộ nội dung cốt truyện: đoạn thoại gặp boss,
    /// đoạn thoại sau khi đánh bại boss, và cảnh giải cứu tinh linh đất.
    /// 
    /// Single Responsibility: chỉ chịu trách nhiệm cung cấp dữ liệu thoại,
    /// không chứa logic xử lý trạng thái hay logic game.
    /// </summary>
    public static class BossEncounterData
    {
        /// <summary>
        /// Tạo cảnh thoại mở đầu khi Arin vừa gặp Gorgon trong rừng sâu.
        /// Nội dung: Gorgon chế nhạo, Terra kêu cứu, Arin quyết tâm chiến đấu.
        /// </summary>
        public static BossDialogueScene CreatePreBossScene(ILocalizationService localization)
        {
            var lines = new List<BossDialogueLine>
            {
                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Narrator,
                    "boss.pre.l2.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Gorgon,
                    "boss.pre.l3.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Arin,
                    "boss.pre.l4.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Gorgon,
                    "boss.pre.l5.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Terra,
                    "boss.pre.l6.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Arin,
                    "boss.pre.l7.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Gorgon,
                    "boss.pre.l8.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Arin,
                    "boss.pre.l9.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.OnScreenText,
                    "", "boss.pre.l10.caption")
            };

            return new BossDialogueScene(
                sceneName: "The Gorgon's Lair",
                backgroundImageName: "Backgrounds/DarkForest.jpg",
                lines: lines.AsReadOnly());
        }

        /// <summary>
        /// Tạo cảnh thoại sau khi Gorgon bị đánh bại.
        /// Nội dung: Gorgon tan biến, tinh linh Terra được giải phóng,
        /// Terra cảm ơn Arin và trao quyền năng.
        /// </summary>
        public static BossDialogueScene CreatePostBossScene(ILocalizationService localization)
        {
            var lines = new List<BossDialogueLine>
            {
                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Narrator,
                    "boss.post.l1.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Gorgon,
                    "boss.post.l2.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Narrator,
                    "boss.post.l3.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.OnScreenText,
                    "", "boss.post.l4.caption"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Terra,
                    "boss.post.l5.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Arin,
                    "boss.post.l6.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Terra,
                    "boss.post.l7.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Arin,
                    "boss.post.l8.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Terra,
                    "boss.post.l9.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Narrator,
                    "boss.post.l10.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.OnScreenText,
                    "", "boss.post.l11.caption")
            };

            return new BossDialogueScene(
                sceneName: "Terra's Liberation",
                backgroundImageName: "FinalForest.png",
                lines: lines.AsReadOnly());
        }

        /// <summary>
        /// Tạo cảnh thoại giải cứu tinh linh - Terra trao quyền năng cho Arin.
        /// </summary>
        public static BossDialogueScene CreateSpiritRescueScene(ILocalizationService localization)
        {
            var lines = new List<BossDialogueLine>
            {
                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Terra,
                    "boss.rescue.l1.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Arin,
                    "boss.rescue.l2.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Terra,
                    "boss.rescue.l3.text"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.OnScreenText,
                    "", "boss.rescue.l4.caption"),

                BossDialogueLine.FromLocalization(
                    localization, BossDialogueSpeaker.Narrator,
                    "boss.rescue.l5.text")
            };

            return new BossDialogueScene(
                sceneName: "Spirit Bond",
                backgroundImageName: "FinalForest.png",
                lines: lines.AsReadOnly());
        }
    }
}