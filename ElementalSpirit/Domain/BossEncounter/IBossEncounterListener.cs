namespace ElementalSpirit.Domain.BossEncounter
{
    using System;

    public interface IBossEncounterListener
    {
        /// <summary>Khi trạng thái cuộc gặp boss thay đổi</summary>
        event Action<BossEncounterState, BossEncounterState> OnStateChanged;

        /// <summary>Khi bắt đầu đoạn thoại mở đầu</summary>
        event Action<BossDialogueScene> OnPreBossDialogueStarted;

        /// <summary>Khi bắt đầu trận đấu boss</summary>
        event Action OnBossFightStarted;

        /// <summary>Khi boss bị đánh bại, bắt đầu đoạn thoại kết thúc</summary>
        event Action<BossDialogueScene> OnPostBossDialogueStarted;

        /// <summary>Khi bắt đầu cảnh giải cứu tinh linh</summary>
        event Action OnSpiritRescueStarted;

        /// <summary>Khi hoàn thành toàn bộ màn boss</summary>
        event Action OnBossEncounterCompleted;
    }
}