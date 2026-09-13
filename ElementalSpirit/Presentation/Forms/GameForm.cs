using System;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;
using ElementalSpirit.Presentation.Rendering;

namespace ElementalSpirit.Presentation.Forms
{
    public class GameForm : Form
    {
        private readonly GameManager _gameManager;
        private readonly GameTimer _gameTimer;
        private readonly ILocalizationService _localization;
        private readonly GameRenderer _renderer;

        private readonly PlayerAnimationController _playerAnimController;
        private readonly Image[]? _fireballFrames;
        private PlayerAnimationState _lastAnimState = PlayerAnimationState.Idle;
        private bool _attackHitFrameTriggered;
        private bool _fireCastFrameTriggered;

        private const int AttackHitFrameIndex = 3;
        private const int FireCastFrameIndex = 5;

        public GameForm(GameManager gameManager, ILocalizationService localization)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
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
            try { _fireballFrames = MageAnimationLoader.LoadFireballFrames(); }
            catch { _fireballFrames = null; }

            _renderer = new GameRenderer(_gameManager, _playerAnimController, _fireballFrames, _localization);

            _gameTimer = new GameTimer(targetFps: 60);
            _gameTimer.OnTick += OnGameTick;

            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            Resize += OnFormResize;
            FormClosing += OnFormClosing;

            _gameTimer.Start();
        }

        private void OnGameTick(float deltaTime)
        {
            _gameManager.Update(deltaTime);
            UpdatePlayerAnimation(deltaTime);
            Invalidate();
        }

        private void UpdatePlayerAnimation(float deltaTime)
        {
            var player = _gameManager.Player;
            var desired = DetermineAnimationState(player);

            if (desired != _lastAnimState)
            {
                _attackHitFrameTriggered = false;
                _fireCastFrameTriggered = false;
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

            if (desired == PlayerAnimationState.Fire &&
                currentFrame >= FireCastFrameIndex && !_fireCastFrameTriggered)
            {
                _fireCastFrameTriggered = true;
                player.NotifyFireCastFrame();
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
                        player.NotifyFireAnimationEnded(); break;
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
            if (player.IsFiring) return PlayerAnimationState.Fire;
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
            if (e.KeyCode == Keys.B) { OpenShop(); return; }
            if (e.KeyCode == Keys.U) { OpenUpgrade(); return; }
            _gameManager.HandleKeyDown(e.KeyCode);
            if (e.KeyCode == Keys.Escape) Close();
        }

        private void OpenShop()
        {
            PauseGame();
            try
            {
                using var shop = new ShopForm(_gameManager.Wallet, _gameManager.Inventory,
                    _gameManager.Shop, onInventoryChanged: () => _gameManager.RefreshPlayerEquipmentStats());
                shop.ShowDialog(this);
            }
            finally { ResumeGame(); }
        }

        private void OpenUpgrade()
        {
            PauseGame();
            try
            {
                using var upgrade = new UpgradeForm(_gameManager.Spirits, _gameManager.Wallet, _gameManager.Upgrades, _localization);
                upgrade.ShowDialog(this);
            }
            finally { ResumeGame(); }
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
            _gameTimer.Stop();
            _gameTimer.Dispose();
            _playerAnimController.Dispose();
            if (_fireballFrames != null) foreach (var img in _fireballFrames) img.Dispose();
            AssetLoader.DisposeAll();
            MageAnimationLoader.DisposeAll();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _renderer.Render(e.Graphics, ClientSize);
        }
    }
}