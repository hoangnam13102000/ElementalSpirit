namespace ElementalSpirit.GameEngine
{
    using ElementalSpirit.Domain.BossEncounter;
    using ElementalSpirit.Domain.Enemy;
    using ElementalSpirit.GameEngine.Abstractions;
    using ElementalSpirit.Localization;
    using ElementalSpirit.Presentation.BossEncounter;
    using System;

    /// <summary>
    /// Quản lý luồng cuộc gặp boss kiểu "bong bóng thoại trên đầu boss".
    /// Không mở cửa sổ riêng, không tạm dừng game. Chỉ cập nhật trạng thái và bubble trên màn hình.
    /// </summary>
    public sealed class BossEncounterManager : IBossEncounterManager
    {
        private BossEncounterState _currentState = BossEncounterState.NotStarted;
        private GorgonBoss? _currentBoss;
        private bool _bossDefeatedNotified;
        private BossDialogueScene? _currentScene;
        private int _currentLineIndex;

        public BossEncounterState CurrentState => _currentState;
        public bool IsIntroDialogue => _currentState == BossEncounterState.PreBossDialogue &&
                                       _currentLineIndex == 0;
        public string? ActiveBackgroundImageName => IsIntroDialogue
            ? _currentScene?.BackgroundImageName
            : null;
        public GorgonBoss? CurrentBoss => _currentBoss;
        public BossDialogueScene PreBossScene { get; }
        public BossDialogueScene PostBossScene { get; }
        public BossDialogueScene SpiritRescueScene { get; }

        public BossSpeechBubble ActiveSpeechBubble { get; } = new();
        public BossSpeechBubble PlayerSpeechBubble { get; } = new();

        public event Action<BossEncounterState, BossEncounterState>? OnStateChanged;
        public event Action<BossDialogueScene>? OnPreBossDialogueStarted;
        public event Action? OnBossFightStarted;
        public event Action<BossDialogueScene>? OnPostBossDialogueStarted;
        public event Action? OnSpiritRescueStarted;
        public event Action? OnBossEncounterCompleted;

        public BossEncounterManager(ILocalizationService localization)
        {
            if (localization == null) throw new ArgumentNullException(nameof(localization));

            PreBossScene = BossEncounterData.CreatePreBossScene(localization);
            PostBossScene = BossEncounterData.CreatePostBossScene(localization);
            SpiritRescueScene = BossEncounterData.CreateSpiritRescueScene(localization);
        }

        public void StartEncounter()
        {
            if (_currentState != BossEncounterState.NotStarted) return;

            TransitionTo(BossEncounterState.PreBossDialogue);
            StartScene(PreBossScene);
            OnPreBossDialogueStarted?.Invoke(PreBossScene);
        }

        public void BeginBossFight()
        {
            if (_currentState != BossEncounterState.PreBossDialogue) return;

            HideSpeechBubbles();
            TransitionTo(BossEncounterState.BossFight);
            OnBossFightStarted?.Invoke();
        }

        public void NotifyBossDefeated(GorgonBoss boss)
        {
            if (_currentState != BossEncounterState.BossFight) return;
            if (_bossDefeatedNotified) return;

            _bossDefeatedNotified = true;
            TransitionTo(BossEncounterState.PostBossDialogue);
            StartScene(PostBossScene);
            OnPostBossDialogueStarted?.Invoke(PostBossScene);
        }

        public void BeginSpiritRescue()
        {
            if (_currentState != BossEncounterState.PostBossDialogue) return;

            HideSpeechBubbles();
            TransitionTo(BossEncounterState.SpiritRescue);
            StartScene(SpiritRescueScene);
            OnSpiritRescueStarted?.Invoke();
        }

        public void CompleteEncounter()
        {
            if (_currentState == BossEncounterState.Completed) return;

            HideSpeechBubbles();
            TransitionTo(BossEncounterState.Completed);
            OnBossEncounterCompleted?.Invoke();
        }

        public void Update(float deltaTime)
        {
            ActiveSpeechBubble.Update(deltaTime);
            PlayerSpeechBubble.Update(deltaTime);

            if (_currentState == BossEncounterState.BossFight &&
                _currentBoss != null &&
                !_currentBoss.IsAlive &&
                !_bossDefeatedNotified)
            {
                NotifyBossDefeated(_currentBoss);
            }
        }

        public void AdvanceDialogue()
        {
            if (_currentScene == null || _currentLineIndex >= _currentScene.Lines.Count)
            {
                CompleteCurrentScene();
                return;
            }

            _currentLineIndex++;
            if (_currentLineIndex >= _currentScene.Lines.Count)
            {
                CompleteCurrentScene();
                return;
            }

            ShowCurrentLine();
        }

        public void SkipDialogue()
        {
            CompleteCurrentScene();
        }

        public void RegisterBoss(GorgonBoss boss)
        {
            _currentBoss = boss ?? throw new ArgumentNullException(nameof(boss));
            _bossDefeatedNotified = false;
        }

        private void StartScene(BossDialogueScene scene)
        {
            _currentScene = scene ?? throw new ArgumentNullException(nameof(scene));
            _currentLineIndex = 0;
            ShowCurrentLine();
        }

        private void ShowCurrentLine()
        {
            if (_currentScene == null || _currentLineIndex < 0 || _currentLineIndex >= _currentScene.Lines.Count)
            {
                HideSpeechBubbles();
                return;
            }

            var line = _currentScene.Lines[_currentLineIndex];
            string speakerName = line.Speaker switch
            {
                BossDialogueSpeaker.Gorgon => "Gorgon",
                BossDialogueSpeaker.Arin => "Arin",
                BossDialogueSpeaker.Terra => "Terra",
                BossDialogueSpeaker.Narrator => "Narrator",
                _ => ""
            };

            string text = line.Speaker == BossDialogueSpeaker.OnScreenText
                ? line.OnScreenCaption ?? line.Text
                : line.Text;

            bool isPlayerBubble = line.Speaker == BossDialogueSpeaker.Arin ||
                                  line.Speaker == BossDialogueSpeaker.Terra;

            var bubble = isPlayerBubble ? PlayerSpeechBubble : ActiveSpeechBubble;
            bubble.Show(speakerName, text, displaySeconds: 4.5f, isPlayerBubble: isPlayerBubble);

            bubble.SetPositionFromAnchor(
                isPlayerBubble ? 260f : (_currentBoss != null ? _currentBoss.X + _currentBoss.Width / 2f : 640f),
                isPlayerBubble ? 220f : (_currentBoss != null ? _currentBoss.Y : 260f));
        }

        private void CompleteCurrentScene()
        {
            HideSpeechBubbles();
            _currentScene = null;
            _currentLineIndex = 0;

            if (_currentState == BossEncounterState.PreBossDialogue)
            {
                BeginBossFight();
            }
            else if (_currentState == BossEncounterState.PostBossDialogue)
            {
                BeginSpiritRescue();
            }
            else if (_currentState == BossEncounterState.SpiritRescue)
            {
                CompleteEncounter();
            }
        }

        private void HideSpeechBubbles()
        {
            ActiveSpeechBubble.Hide();
            PlayerSpeechBubble.Hide();
        }

        private void TransitionTo(BossEncounterState newState)
        {
            if (_currentState == newState) return;

            var oldState = _currentState;
            _currentState = newState;
            OnStateChanged?.Invoke(oldState, newState);
        }
    }
}