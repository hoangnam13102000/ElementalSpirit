namespace ElementalSpirit.Presentation.BossEncounter
{
    using ElementalSpirit.Domain.BossEncounter;
    using System;

    public interface IBossDialogueView
    {
        /// <summary>Yêu cầu View hiển thị một dòng thoại</summary>
        void ShowDialogueLine(BossDialogueLine line);

        /// <summary>Yêu cầu View hiển thị tên cảnh</summary>
        void ShowSceneName(string sceneName);

        /// <summary>Yêu cầu View hiển thị hoặc ẩn khung thoại</summary>
        void SetDialogueVisible(bool visible);

        /// <summary>Yêu cầu View đóng</summary>
        void CloseView();

        /// <summary>Yêu cầu View hiển thị thông báo</summary>
        void ShowInfo(string message, string title);

        /// <summary>Sự kiện khi người dùng nhấn nút "Tiếp tục"</summary>
        event EventHandler? NextLineRequested;

        /// <summary>Sự kiện khi người dùng nhấn nút "Bỏ qua cảnh"</summary>
        event EventHandler? SkipSceneRequested;
    }
}