using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElementalSpirit.Domain.Equipment;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Presentation.Assets;

namespace ElementalSpirit.Presentation.Forms
{
    public class GameForm : Form
    {
        private readonly GameManager _gameManager;
        private readonly GameTimer _gameTimer;

        private readonly Image? _playerImage = AssetLoader.Get("Player.png");
        private string _loadedBackgroundName = "";
        private Image? _backgroundImage;

        public GameForm()
        {
            Text = "Elemental Spirit - Phase 5 (Currency / Upgrade / Shop)";
            ClientSize = new Size(1280, 720);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            _gameManager = new GameManager();
            _gameManager.SetPlayArea(ClientSize.Width, ClientSize.Height);

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
            Invalidate();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.B)
            {
                OpenShop();
                return;
            }

            if (e.KeyCode == Keys.U)
            {
                OpenUpgrade();
                return;
            }

            _gameManager.HandleKeyDown(e.KeyCode);

            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void OpenShop()
        {
            PauseGame();
            try
            {
                using var shop = new ShopForm(
                    _gameManager.Wallet,
                    _gameManager.Inventory,
                    _gameManager.Shop,
                    onInventoryChanged: () => _gameManager.RefreshPlayerEquipmentStats());
                shop.ShowDialog(this);
            }
            finally
            {
                ResumeGame();
            }
        }

        private void OpenUpgrade()
        {
            PauseGame();
            try
            {
                using var upgrade = new UpgradeForm(
                    _gameManager.Spirits,
                    _gameManager.Wallet,
                    _gameManager.Upgrades);
                upgrade.ShowDialog(this);
            }
            finally
            {
                ResumeGame();
            }
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

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            _gameManager.HandleKeyUp(e.KeyCode);
        }

        private void OnFormResize(object? sender, EventArgs e)
        {
            _gameManager.SetPlayArea(ClientSize.Width, ClientSize.Height);
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            _gameTimer.Stop();
            _gameTimer.Dispose();
            AssetLoader.DisposeAll();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(18, 22, 32));

            DrawBackground(g);
            DrawGround(g);
            DrawPlayer(g);
            DrawProjectiles(g);
            DrawEnemies(g);
            DrawSpiritHud(g);
            DrawCurrencyHud(g);
            DrawEquipmentHud(g);
            DrawDebugInfo(g);
        }

        private void DrawBackground(Graphics g)
        {
            string bgName = _gameManager.Waves.StageBackgroundImageName;
            if (bgName != _loadedBackgroundName)
            {
                _backgroundImage = AssetLoader.Get(bgName);
                _loadedBackgroundName = bgName;
            }

            if (_backgroundImage != null)
            {
                // Stretch to fill the play area, cropping/letterboxing is skipped
                // for simplicity since the window size is fixed.
                g.DrawImage(_backgroundImage, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
            }
        }

        private void DrawGround(Graphics g)
        {
            // Visual ground rendering is intentionally empty — the Earth Forest background
            // already provides a beautiful ground/path artwork. GroundY is still used
            // for physics collision; no extra visual overlay needed.
        }

        private void DrawPlayer(Graphics g)
        {
            var p = _gameManager.Player;
            if (!p.IsAlive) return;

            if (_playerImage != null)
            {
                // The character artwork is drawn larger than the (small) collision box
                // so it reads well on screen, centered on the actual hitbox.
                float drawWidth = p.Width * 3.2f;
                float drawHeight = p.Height * 3.2f;
                float centerX = p.X + p.Width / 2f;
                float bottomY = p.Y + p.Height;
                float drawX = centerX - drawWidth / 2f;
                float drawY = bottomY - drawHeight;

                var state = g.Save();
                if (_gameManager.FacingDirection < 0)
                {
                    // Flip horizontally in place when facing left.
                    g.TranslateTransform(drawX + drawWidth, drawY);
                    g.ScaleTransform(-1f, 1f);
                    g.DrawImage(_playerImage, 0, 0, drawWidth, drawHeight);
                }
                else
                {
                    g.DrawImage(_playerImage, drawX, drawY, drawWidth, drawHeight);
                }
                g.Restore(state);
            }
            else
            {
                using var body = new SolidBrush(Color.FromArgb(90, 160, 255));
                using var outline = new Pen(Color.FromArgb(40, 90, 180), 2f);
                g.FillEllipse(body, p.Bounds);
                g.DrawEllipse(outline, p.Bounds);
            }

            if (p.ActiveShield)
            {
                using var shieldPen = new Pen(Color.FromArgb(120, 200, 120), 3f);
                g.DrawEllipse(shieldPen, p.X - 6, p.Y - 6, p.Width + 12, p.Height + 12);
            }
            if (p.ActiveFireBoost)
            {
                using var fireBrush = new SolidBrush(Color.FromArgb(180, 255, 100, 40));
                g.FillEllipse(fireBrush, p.X + 4, p.Y - 10, 10, 10);
            }
            if (p.ActiveHealEffect)
            {
                using var healBrush = new SolidBrush(Color.FromArgb(160, 80, 220, 255));
                g.FillEllipse(healBrush, p.X + p.Width - 14, p.Y - 10, 10, 10);
            }
            if (p.ActiveWindBarrage)
            {
                using var windPen = new Pen(Color.FromArgb(150, 180, 255, 220), 2f);
                g.DrawArc(windPen, p.X - 8, p.Y - 8, p.Width + 16, p.Height + 16, 0, 270);
            }
        }

        private void DrawProjectiles(Graphics g)
        {
            foreach (var p in _gameManager.Projectiles.Projectiles)
            {
                if (!p.IsAlive) continue;
                using var brush = new SolidBrush(Color.FromArgb(255, 220, 80));
                g.FillEllipse(brush, p.Bounds);
            }
        }

        private void DrawEnemies(Graphics g)
        {
            var player = _gameManager.Player;
            float playerCenterX = player.X + player.Width / 2f;

            foreach (var e in _gameManager.Enemies.Enemies)
            {
                if (!e.IsAlive) continue;

                if (e.Image != null)
                {
                    float drawScale = 1.2f;
                    float drawWidth = e.Width * drawScale;
                    float drawHeight = e.Height * drawScale;
                    float drawX = e.X + (e.Width - drawWidth) / 2f;
                    float drawY = e.Y + (e.Height - drawHeight) / 2f;
                    float enemyCenterX = e.X + e.Width / 2f;
                    var state = g.Save();
                    if (playerCenterX < enemyCenterX)
                    {
                        g.TranslateTransform(drawX + drawWidth, drawY);
                        g.ScaleTransform(-1f, 1f);

                        g.DrawImage(e.Image, 0, 0, drawWidth, drawHeight);
                    }
                    else
                    {
                        g.DrawImage(e.Image, drawX, drawY, drawWidth, drawHeight);
                    }
                    g.Restore(state);
                }
                else
                {
                    using var body = new SolidBrush(Color.FromArgb(80, 200, 90));
                    using var outline = new Pen(Color.FromArgb(40, 120, 50), 2f);
                    g.FillEllipse(body, e.Bounds);
                    g.DrawEllipse(outline, e.Bounds);
                }

                float hpPercent = (float)e.Health / e.MaxHealth;
                using var hpBrush = new SolidBrush(Color.FromArgb(220, 60, 60));
                g.FillRectangle(hpBrush, e.X, e.Y - 10, e.Width * hpPercent, 5);
            }
        }

        private void DrawSpiritHud(Graphics g)
        {
            var spirits = _gameManager.Spirits;
            float startX = 12f;
            float startY = ClientSize.Height - 78f;
            float slotW = 210f;
            float slotH = 58f;

            for (int i = 0; i < SpiritManager.MaxEquipped; i++)
            {
                var spirit = spirits.Equipped[i];
                float x = startX + i * (slotW + 12);

                using var bg = new SolidBrush(Color.FromArgb(180, 20, 24, 36));
                using var border = new Pen(Color.FromArgb(100, 140, 180, 255), 1.5f);
                g.FillRectangle(bg, x, startY, slotW, slotH);
                g.DrawRectangle(border, x, startY, slotW, slotH);

                using var keyFont = new Font("Consolas", 14f, FontStyle.Bold);
                using var keyBrush = new SolidBrush(Color.FromArgb(220, 230, 255));
                g.DrawString($"[{i + 1}]", keyFont, keyBrush, x + 8, startY + 6);

                if (spirit == null)
                {
                    using var emptyFont = new Font("Consolas", 10f);
                    using var emptyBrush = new SolidBrush(Color.FromArgb(140, 150, 170));
                    g.DrawString("Empty", emptyFont, emptyBrush, x + 50, startY + 20);
                    continue;
                }

                Color elementColor = spirit.Element switch
                {
                    "Earth" => Color.FromArgb(120, 200, 100),
                    "Fire" => Color.FromArgb(255, 140, 60),
                    "Water" => Color.FromArgb(80, 180, 255),
                    "Wind" => Color.FromArgb(180, 220, 255),
                    _ => Color.White
                };

                using var nameFont = new Font("Consolas", 11f, FontStyle.Bold);
                using var nameBrush = new SolidBrush(elementColor);
                g.DrawString($"{spirit.Name}  Lv.{spirit.Level}", nameFont, nameBrush, x + 48, startY + 8);

                using var statusFont = new Font("Consolas", 9f);
                string status;
                Color statusColor;
                if (spirit.IsActive)
                {
                    status = $"ACTIVE  {spirit.ActiveRemaining:0.0}s";
                    statusColor = Color.FromArgb(120, 255, 160);
                }
                else if (spirit.CooldownRemaining > 0)
                {
                    status = $"CD  {spirit.CooldownRemaining:0.0}s";
                    statusColor = Color.FromArgb(255, 160, 100);
                }
                else
                {
                    status = "READY";
                    statusColor = Color.FromArgb(180, 220, 255);
                }
                using var statusBrush = new SolidBrush(statusColor);
                g.DrawString(status, statusFont, statusBrush, x + 48, startY + 30);

                float cdRatio = spirit.CooldownDuration <= 0 ? 0
                    : Math.Clamp(spirit.CooldownRemaining / spirit.CooldownDuration, 0f, 1f);
                if (cdRatio > 0)
                {
                    using var cdBrush = new SolidBrush(Color.FromArgb(120, 255, 100, 60));
                    g.FillRectangle(cdBrush, x + 2, startY + slotH - 6, (slotW - 4) * cdRatio, 4);
                }
            }
        }

        private void DrawCurrencyHud(Graphics g)
        {
            var w = _gameManager.Wallet;
            string text = $"Gold: {w.Gold}   Shards: {w.SpiritShards}   Crystals: {w.Crystals}";
            using var font = new Font("Consolas", 12f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.FromArgb(255, 220, 140));
            var size = g.MeasureString(text, font);
            g.DrawString(text, font, brush, ClientSize.Width - size.Width - 16, 12);

            if (!string.IsNullOrEmpty(_gameManager.StatusMessage))
            {
                using var msgFont = new Font("Consolas", 11f);
                using var msgBrush = new SolidBrush(Color.FromArgb(180, 255, 200));
                var msgSize = g.MeasureString(_gameManager.StatusMessage, msgFont);
                g.DrawString(_gameManager.StatusMessage, msgFont, msgBrush,
                    ClientSize.Width - msgSize.Width - 16, 34);
            }
        }

        private void DrawEquipmentHud(Graphics g)
        {
            var inv = _gameManager.Inventory;
            float x = ClientSize.Width - 260;
            float y = 60;
            using var titleFont = new Font("Consolas", 10f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(180, 200, 230));
            g.DrawString("EQUIPPED", titleFont, titleBrush, x, y);
            y += 18;
            using var itemFont = new Font("Consolas", 9f);
            using var itemBrush = new SolidBrush(Color.FromArgb(200, 210, 230));
            DrawEquipLine(g, itemFont, itemBrush, x, ref y, "WPN", inv.GetEquipped(EquipmentSlot.Weapon));
            DrawEquipLine(g, itemFont, itemBrush, x, ref y, "ARM", inv.GetEquipped(EquipmentSlot.Armor));
            DrawEquipLine(g, itemFont, itemBrush, x, ref y, "ACC", inv.GetEquipped(EquipmentSlot.Accessory));
            y += 8;
            g.DrawString($"Owned: {inv.Items.Count} items", itemFont, itemBrush, x, y);
        }

        private static void DrawEquipLine(Graphics g, Font font, Brush brush, float x, ref float y,
            string label, Equipment? item)
        {
            string name = item?.Name ?? "(none)";
            string bonus = item == null ? ""
                : $" +{item.BonusDamage}DMG +{item.BonusMaxHp}HP +{item.BonusDefense}DEF";
            g.DrawString($"{label}: {name}{bonus}", font, brush, x, y);
            y += 16;
        }

        private void DrawDebugInfo(Graphics g)
        {
            var p = _gameManager.Player;
            var waves = _gameManager.Waves;
            string stageStatus = _gameManager.IsStageCompleted
                ? "STAGE CLEAR! (Press ESC)"
                : $"Wave {waves.CurrentWaveNumber}/{waves.TotalWaves}  |  State: {waves.State}";

            string effects = "";
            if (p.ActiveShield) effects += " [SHIELD]";
            if (p.ActiveFireBoost) effects += $" [FIRE DMG={p.Damage}]";
            if (p.ActiveHealEffect) effects += " [HEAL]";
            if (p.ActiveWindBarrage) effects += $" [WIND +{p.ExtraProjectiles}]";
            string platState = p.IsGrounded ? $"GROUNDED [{p.MovementState}]" : $"AIR [{p.MovementState}] VY={p.VelocityY:0}";

            string info =
                $"Phase 5 – Currency / Upgrade / Shop\n" +
                $"Stage: {waves.StageName}\n" +
                $"{stageStatus}\n" +
                $"HP: {p.CurrentHp}/{p.MaxHp}  DMG: {p.Damage}  DEF: {p.Defense}{effects}\n" +
                $"{platState}\n" +
                $"Enemies: {_gameManager.Enemies.Enemies.Count}  Projectiles: {_gameManager.Projectiles.Projectiles.Count}\n" +
                $"1/2 Spirit | 3/4 Loadout | B Shop (pause) | U Upgrade (pause) | G +Gold/Shards\n" +
                $"A/D Move | W/Up Jump | Space Shoot | N Next Wave | ESC Exit";

            using var font = new Font("Consolas", 11f);
            using var brush = new SolidBrush(Color.FromArgb(200, 220, 255));
            g.DrawString(info, font, brush, 12, 12);

            if (_gameManager.IsStageCompleted)
            {
                using var bigFont = new Font("Consolas", 28f, FontStyle.Bold);
                using var bigBrush = new SolidBrush(Color.FromArgb(100, 255, 150));
                string msg = "STAGE CLEAR!";
                var size = g.MeasureString(msg, bigFont);
                g.DrawString(msg, bigFont, bigBrush,
                    (ClientSize.Width - size.Width) / 2, ClientSize.Height / 2 - 40);
            }
        }
    }
}