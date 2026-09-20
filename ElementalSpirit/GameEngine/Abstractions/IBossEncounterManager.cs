namespace ElementalSpirit.GameEngine.Abstractions
{
    using ElementalSpirit.Domain.BossEncounter;
    using ElementalSpirit.Domain.Enemy;
    using ElementalSpirit.Presentation.BossEncounter;
    using System;

    public interface IBossEncounterManager : IBossEncounterListener
    {
        /// <summary>Trạng thái hiện tại của cuộc gặp boss</summary>
        BossEncounterState CurrentState { get; }

        /// <summary>Background của cảnh thoại đang hiển thị, nếu có.</summary>
        string? ActiveBackgroundImageName { get; }

        /// <summary>Cho biết đang ở cảnh intro trước khi nhân vật xuất hiện.</summary>
        bool IsIntroDialogue { get; }

        /// <summary>Boss hiện tại (nếu đã được spawn)</summary>
        GorgonBoss? CurrentBoss { get; }

        /// <summary>Bong bóng thoại của boss đang hiển thị</summary>
        BossSpeechBubble ActiveSpeechBubble { get; }

        /// <summary>Bong bóng thoại của người chơi đang hiển thị</summary>
        BossSpeechBubble PlayerSpeechBubble { get; }

        /// <summary>Cảnh thoại mở đầu</summary>
        BossDialogueScene PreBossScene { get; }

        /// <summary>Cảnh thoại sau khi đánh bại boss</summary>
        BossDialogueScene PostBossScene { get; }

        /// <summary>Cảnh thoại giải cứu tinh linh</summary>
        BossDialogueScene SpiritRescueScene { get; }

        /// <summary>
        /// Bắt đầu toàn bộ luồng cuộc gặp boss.
        /// Chuyển sang trạng thái PreBossDialogue.
        /// </summary>
        void StartEncounter();

        /// <summary>
        /// Kết thúc đoạn thoại mở đầu, bắt đầu trận đấu.
        /// </summary>
        void BeginBossFight();

        /// <summary>
        /// Thông báo rằng boss đã bị đánh bại.
        /// Chuyển sang trạng thái PostBossDialogue.
        /// </summary>
        void NotifyBossDefeated(GorgonBoss boss);

        /// <summary>
        /// Kết thúc đoạn thoại sau boss, bắt đầu cảnh giải cứu tinh linh.
        /// </summary>
        void BeginSpiritRescue();

        /// <summary>
        /// Hoàn thành cảnh giải cứu tinh linh, kết thúc màn boss.
        /// </summary>
        void CompleteEncounter();

        /// <summary>
        /// Chuyển sang dòng thoại tiếp theo trong cảnh hiện tại.
        /// </summary>
        void AdvanceDialogue();

        /// <summary>
        /// Bỏ qua toàn bộ đoạn thoại đang hiển thị.
        /// </summary>
        void SkipDialogue();

        /// <summary>
        /// Update mỗi frame, kiểm tra trạng thái boss.
        /// </summary>
        void Update(float deltaTime);

        /// <summary>
        /// Đăng ký boss hiện tại để theo dõi.
        /// </summary>
        void RegisterBoss(GorgonBoss boss);
    }
}