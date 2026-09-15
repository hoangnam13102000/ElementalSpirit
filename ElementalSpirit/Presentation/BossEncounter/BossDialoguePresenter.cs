namespace ElementalSpirit.Presentation.BossEncounter
{
    using ElementalSpirit.Domain.BossEncounter;
    using System;

    /// <summary>
    /// Presenter cho hệ thống hiển thị thoại boss.
    /// Lớp trung gian giữa Model (BossDialogueScene) và View (IBossDialogueView).
    /// 
    /// Model-View-Presenter:
    /// - Model: BossDialogueScene, BossDialogueLine
    /// - View: IBossDialogueView (interface)
    /// - Presenter: BossDialoguePresenter (lớp này)
    /// 
    /// Single Responsibility: chỉ chịu trách nhiệm điều phối luồng hiển thị thoại,
    /// không chứa logic game hay logic render cụ thể.
    /// 
    /// Dependency Inversion: phụ thuộc vào IBossDialogueView (abstraction),
    /// không phụ thuộc vào implementation cụ thể của View.
    /// </summary>
    public sealed class BossDialoguePresenter
    {
        private readonly IBossDialogueView _view;
        private BossDialogueScene? _currentScene;
        private int _currentLineIndex;

        /// <summary>Sự kiện khi toàn bộ cảnh thoại đã hoàn thành</summary>
        public event EventHandler? SceneCompleted;

        public BossDialoguePresenter(IBossDialogueView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));

            // Đăng ký lắng nghe sự kiện từ View
            _view.NextLineRequested += OnNextLineRequested;
            _view.SkipSceneRequested += OnSkipSceneRequested;
        }

        /// <summary>
        /// Bắt đầu hiển thị một cảnh thoại mới.
        /// </summary>
        public void StartScene(BossDialogueScene scene)
        {
            _currentScene = scene ?? throw new ArgumentNullException(nameof(scene));
            _currentLineIndex = 0;

            _view.ShowSceneName(scene.SceneName);
            _view.SetDialogueVisible(true);

            if (scene.Lines.Count > 0)
            {
                _view.ShowDialogueLine(scene.Lines[0]);
            }
            else
            {
                CompleteScene();
            }
        }

        /// <summary>
        /// Xử lý khi người dùng yêu cầu dòng thoại tiếp theo.
        /// </summary>
        private void OnNextLineRequested(object? sender, EventArgs e)
        {
            if (_currentScene == null) return;

            _currentLineIndex++;

            if (_currentLineIndex >= _currentScene.Lines.Count)
            {
                CompleteScene();
            }
            else
            {
                _view.ShowDialogueLine(_currentScene.Lines[_currentLineIndex]);
            }
        }

        /// <summary>
        /// Xử lý khi người dùng bỏ qua toàn bộ cảnh thoại.
        /// </summary>
        private void OnSkipSceneRequested(object? sender, EventArgs e)
        {
            CompleteScene();
        }

        /// <summary>
        /// Hoàn thành cảnh thoại hiện tại, thông báo cho người nghe.
        /// </summary>
        private void CompleteScene()
        {
            _view.SetDialogueVisible(false);
            SceneCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}