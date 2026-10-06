using ElementalSpirit.Domain.BossEncounter;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;
using ElementalSpirit.Presentation.BossEncounter;
using ElementalSpirit.Presentation.Forms.Settings;
using ElementalSpirit.Presentation.Rendering;
using ElementalSpirit.Services;
using ElementalSpirit.Services.Abstractions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElementalSpirit.Presentation.Forms
{
    public class GameForm : Form
    {
        private readonly GameManager _gameManager;
        private readonly GameTimer _gameTimer;
        private readonly ILocalizationService _localization;
        private readonly ISaveGameService _saveGameService;
        private readonly GameRenderer _renderer;
        private readonly PlayerAnimationController _playerAnimController;
        private readonly SkillAnimationController _skillAnimController;
        private readonly PortalAnimationController _portalAnimController;
        private readonly AnimationClip? _goldAnimation;
        private readonly Image[]? _fireballFrames;
        private PlayerAnimationState _lastAnimState = PlayerAnimationState.Idle;
        private bool _attackHitFrameTriggered;
        private const int AttackHitFrameIndex = 3;

        public GameForm(GameManager gameManager, ILocalizationService localization, ISaveGameService saveGameService)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _saveGameService = saveGameService ?? throw new ArgumentNullException(nameof(saveGameService));
            AppIcon.ApplyTo(this);
            Text = _localization.Translate("gameForm.windowTitle");
            ClientSize = new Size(1280, 720);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |

                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            _gameManager.SetPlayArea(ClientSize.Width, ClientSize.Height);
            _playerAnimController = MageAnimationLoader.CreateController();
            _skillAnimController = CreateWaterfallAnimationController();
            _portalAnimController = StoneGateAnimationLoader.CreateController();
            _goldAnimation = GoldAnimationLoader.CreateClip();

            try { _fireballFrames = MageAnimationLoader.LoadFireballFrames(); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameForm] Fireball animation unavailable: {ex}");
                _fireballFrames = null;
            }

            _renderer = new GameRenderer(
                _gameManager,
                _playerAnimController,
                _skillAnimController,
                _portalAnimController,
                _goldAnimation,
                _fireballFrames,
                _localization);

            _gameTimer = new GameTimer(targetFps: 60);
            _gameTimer.OnTick += OnGameTick;

            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            Resize += OnFormResize;
            FormClosing += OnFormClosing;
            _gameManager.Player.OnDeath += RecordAchievement;
            _gameManager.FinalBossDefeated += RecordAchievement;
            _gameManager.BossEncounter.OnPreBossDialogueStarted += OnPreBossDialogueStarted;

            if (_gameManager.BossEncounter.CurrentState is
                Domain.BossEncounter.BossEncounterState.PreBossDialogue or
                Domain.BossEncounter.BossEncounterState.BossFight)
                PlayBossTheme();
            else
                Services.AudioManager.Instance.PlayMusic("forest_theme.mp3", loop: true);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            WindowDisplayMode.SetFullscreen(this, Services.FullscreenPreferenceStore.Load());
            _gameManager.SetPlayArea(ClientSize.Width, ClientSize.Height);
            _gameTimer.Start();
        }

        private void OnGameTick(float deltaTime)
        {
            _gameManager.Update(deltaTime);
            UpdatePlayerAnimation(deltaTime);
            UpdateSkillAnimation(deltaTime);
            _portalAnimController.Update(deltaTime);
            _goldAnimation?.Update(deltaTime);
            Invalidate();
        }
        private void UpdateSkillAnimation(float deltaTime)
        {
            var state = _gameManager.Skills.CurrentAnimationState;
            if (state == Domain.Skill.SkillAnimationState.Waterfall &&
                _skillAnimController.IsCompleted)
            {
                _skillAnimController.Restart();
            }
            if (state == Domain.Skill.SkillAnimationState.Waterfall)
                _skillAnimController.Update(deltaTime);
        }
        private static SkillAnimationController CreateWaterfallAnimationController()
        {
            var frames = new System.Collections.Generic.List<Image>();
            for (int i = 0; i <= 12; i++)
            {
                var frame = AssetLoader.Get($"Characters/Skill/Watermagic/WaterFall/water600{i:00}.png");
                if (frame != null) frames.Add(new Bitmap(frame));
            }
            return new SkillAnimationController(frames.ToArray(), 12f);
        }
        private void UpdatePlayerAnimation(float deltaTime)
        {
            var player = _gameManager.Player;
            var desired = DetermineAnimationState(player);

            if (desired != _lastAnimState)
            {
                _attackHitFrameTriggered = false;
                _lastAnimState = desired;
            }
            _playerAnimController.Play(desired);
            _playerAnimController.Update(deltaTime);

            int currentFrame = _playerAnimController.CurrentFrameIndex;

            if (desired == PlayerAnimationState.Attack &&
                currentFrame >= AttackHitFrameIndex && !_attackHitFrameTriggered)
            {
                _attackHitFrameTriggered = true;
                player.NotifyAttackHitFrame();
            }

            if (_playerAnimController.IsCurrentCompleted)
            {
                switch (desired)
                {
                    case PlayerAnimationState.Attack:
                    case PlayerAnimationState.WalkAttack:
                    case PlayerAnimationState.RunAttack:
                        player.NotifyAttackAnimationEnded(); break;
                    case PlayerAnimationState.Fire:
                        player.NotifySkillAnimationEnded();
                        break;
                    case PlayerAnimationState.Hurt:
                        player.NotifyHurtAnimationEnded(); break;
                    case PlayerAnimationState.Death:
                        player.NotifyDeathAnimationEnded(); break;
                }
            }
        }
        private PlayerAnimationState DetermineAnimationState(Player player)
        {
            if (player.IsDead) return PlayerAnimationState.Death;
            if (player.IsHurt) return PlayerAnimationState.Hurt;
            if (player.IsCastingSkill) return PlayerAnimationState.Fire;
            if (player.IsAttacking)
            {
                if (!player.IsGrounded) return PlayerAnimationState.Attack;
                if (Math.Abs(player.VelocityX) > 1f)
                    return player.WantsToRun ? PlayerAnimationState.RunAttack : PlayerAnimationState.WalkAttack;
                return PlayerAnimationState.Attack;
            }
            if (!player.IsGrounded)
                return player.VelocityY < -300f ? PlayerAnimationState.HighJump : PlayerAnimationState.Jump;

            if (Math.Abs(player.VelocityX) > 1f)
                return player.WantsToRun ? PlayerAnimationState.Run : PlayerAnimationState.Walk;

            return PlayerAnimationState.Idle;
        }
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P) { OpenSettings(); return; }
            _gameManager.HandleKeyDown(e.KeyCode);
            if (e.KeyCode == Keys.Escape) Close();
        }
        private void OpenSettings()
        {
            PauseGame();
            bool exitToMenu;
            bool isCurrentlyFullscreen = (this.FormBorderStyle == FormBorderStyle.None);
            using (var settings = new SettingsForm(
                       _localization,
                       _saveGameService,
                       _gameManager,
                       RecordAchievement,
                       enabled => WindowDisplayMode.SetFullscreen(this, enabled),
                       isCurrentlyFullscreen))
            {
                settings.ShowDialog(this);
                exitToMenu = settings.ExitToMainMenuRequested;
            }
            if (exitToMenu)
            {
                Close();
                return;
            }
            ResumeGame();
        }
        private void PauseGame()
        {
            _gameManager.IsPaused = true;
            _gameManager.Input.Clear();
            _gameTimer.Pause();
        }
        private void ResumeGame()
        {
            _gameManager.IsPaused = false;
            _gameManager.Input.Clear();
            _gameTimer.Resume();
            Invalidate();
        }
        private void OnKeyUp(object? sender, KeyEventArgs e) => _gameManager.HandleKeyUp(e.KeyCode);
        private void OnFormResize(object? sender, EventArgs e) => _gameManager.SetPlayArea(ClientSize.Width, ClientSize.Height);
        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            _gameManager.Player.OnDeath -= RecordAchievement;
            _gameManager.FinalBossDefeated -= RecordAchievement;
            _gameManager.BossEncounter.OnPreBossDialogueStarted -= OnPreBossDialogueStarted;
            Services.AudioManager.Instance.StopMusic();
            _gameManager.Dispose();
            _gameTimer.Stop();
            _gameTimer.Dispose();
            _playerAnimController.Dispose();
            _skillAnimController.Dispose();
            _goldAnimation?.Dispose();
            if (_fireballFrames != null) foreach (var img in _fireballFrames) img.Dispose();
            AssetLoader.DisposeAll();
            MageAnimationLoader.DisposeAll();
            SlimeAnimationLoader.DisposeAll();
        }

        private void OnPreBossDialogueStarted(Domain.BossEncounter.BossDialogueScene _)
        {
            PlayBossTheme();
        }

        private static void PlayBossTheme()
        {
            Services.AudioManager.Instance.PlayMusic("boss_theme.wav", loop: true);
        }

        private void RecordAchievement()
        {
            try
            {
                new LeaderboardService().RecordRun(
                    _gameManager.RunId,
                    _gameManager.Player.Name,
                    _gameManager.ClearedStageCount,
                    _gameManager.Wallet.TotalGoldEarned);
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException ||
                ex is InvalidDataException)
            {
                System.Diagnostics.Debug.WriteLine($"[GameForm] Could not record leaderboard result: {ex}");
                MessageBox.Show(
                    this,
                    _localization.Translate("leaderboard.saveError"),
                    _localization.Translate("leaderboard.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _renderer.Render(e.Graphics, ClientSize);
        }
    }
}