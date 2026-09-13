using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ElementalSpirit.Domain.Equipment;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Projectile;
using ElementalSpirit.Domain.Enemy.NormalEnemy;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;

namespace ElementalSpirit.Presentation.Rendering
{
    /// <summary>
    /// Chịu trách nhiệm duy nhất: vẽ trạng thái hiện tại của GameManager lên Graphics.
    /// Được tách ra khỏi GameForm để GameForm chỉ còn giữ vai trò điều phối
    /// game loop/input (single responsibility).
    /// </summary>
    public class GameRenderer
    {
        private readonly GameManager _gameManager;
        private readonly PlayerAnimationController _playerAnimController;
        private readonly Image[]? _fireballFrames;
        private readonly ILocalizationService _localization;

        private const float BaseRenderScale = 0.9f;
        private const int StandardFrameSize = 128;
        private const int DeathFrameSize = 256;
        private const float ProjectileVisualScale = 2.2f;

        private string _loadedBackgroundName = "";
        private Image? _backgroundImage;

        public GameRenderer(
            GameManager gameManager,
            PlayerAnimationController playerAnimController,
            Image[]? fireballFrames,
            ILocalizationService localization)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            _playerAnimController = playerAnimController ?? throw new ArgumentNullException(nameof(playerAnimController));
            _fireballFrames = fireballFrames;
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public void Render(Graphics g, Size clientSize)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(18, 22, 32));
            DrawBackground(g, clientSize);
            DrawPlayer(g);
            DrawProjectiles(g);
            DrawEnemies(g);
            DrawSpiritHud(g, clientSize);
            DrawCurrencyHud(g, clientSize);
            DrawEquipmentHud(g, clientSize);
            DrawDebugInfo(g, clientSize);
        }

        private void DrawBackground(Graphics g, Size clientSize)
        {
            string bgName = _gameManager.Waves.StageBackgroundImageName;
            if (bgName != _loadedBackgroundName)
            {
                _backgroundImage = AssetLoader.Get(bgName);
                _loadedBackgroundName = bgName;
            }
            if (_backgroundImage != null)
                g.DrawImage(_backgroundImage, new Rectangle(0, 0, clientSize.Width, clientSize.Height));
        }

        private void DrawPlayer(Graphics g)
        {
            var p = _gameManager.Player;
            var currentImage = _playerAnimController.CurrentImage;
            if (currentImage == null) return;

            bool isDeath = _playerAnimController.CurrentState == PlayerAnimationState.Death;
            int frameSize = isDeath ? DeathFrameSize : StandardFrameSize;
            float drawSize = frameSize * BaseRenderScale;

            float feetX = p.X + p.Width / 2f;
            float feetY = p.Y + p.Height;
            float anchorRatioY = isDeath ? 0.95f : 0.85f;
            float drawX = feetX - drawSize / 2f;
            float drawY = feetY - drawSize * anchorRatioY;

            var state = g.Save();
            if (p.Facing == FacingDirection.Left)
            {
                g.TranslateTransform(drawX + drawSize, drawY);
                g.ScaleTransform(-1f, 1f);
                g.DrawImage(currentImage, 0, 0, drawSize, drawSize);
            }
            else
            {
                g.DrawImage(currentImage, drawX, drawY, drawSize, drawSize);
            }
            g.Restore(state);

            if (p.IsHurt)
            {
                using var flashBrush = new SolidBrush(Color.FromArgb(80, 255, 80, 80));
                g.FillRectangle(flashBrush, drawX, drawY, drawSize, drawSize);
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

            DrawPlayerHealthBar(g, p);
        }

        private const float HealthBarWidth = 46f;
        private const float HealthBarHeight = 6f;
        private const float HealthBarVerticalGap = 22f;
        // The Mage sprite artwork isn't perfectly centered inside its square frame
        // (extra space is reserved for the staff), so the hitbox's geometric
        // center doesn't line up with what the eye reads as "the character's
        // center". This offset (in screen pixels) corrects for that, and flips
        // sign with the sprite's mirrored facing.
        private const float HealthBarVisualOffsetX = 16f;

        private void DrawPlayerHealthBar(Graphics g, Player p)
        {
            if (p.IsDead) return;

            float centerX = p.X + p.Width / 2f;
            centerX += p.Facing == FacingDirection.Left ? HealthBarVisualOffsetX : -HealthBarVisualOffsetX;
            float barX = centerX - HealthBarWidth / 2f;
            float barY = p.Y - HealthBarVerticalGap;

            float hpPercent = p.MaxHp > 0 ? Math.Clamp((float)p.CurrentHp / p.MaxHp, 0f, 1f) : 0f;

            using var bgBrush = new SolidBrush(Color.FromArgb(170, 30, 30, 34));
            g.FillRectangle(bgBrush, barX, barY, HealthBarWidth, HealthBarHeight);

            Color hpColor = hpPercent > 0.5f
                ? Color.FromArgb(230, 90, 220, 100)
                : hpPercent > 0.25f
                    ? Color.FromArgb(230, 235, 200, 60)
                    : Color.FromArgb(230, 230, 70, 70);
            using var hpBrush = new SolidBrush(hpColor);
            g.FillRectangle(hpBrush, barX, barY, HealthBarWidth * hpPercent, HealthBarHeight);

            using var borderPen = new Pen(Color.FromArgb(200, 10, 10, 12), 1f);
            g.DrawRectangle(borderPen, barX, barY, HealthBarWidth, HealthBarHeight);
        }

        private void DrawProjectiles(Graphics g)
        {
            foreach (var p in _gameManager.Projectiles.Projectiles)
            {
                if (!p.IsAlive) continue;
                if (p is PlayerProjectile pp && pp.IsFireball && _fireballFrames != null && _fireballFrames.Length > 0)
                {
                    var fireImg = _fireballFrames[_fireballFrames.Length / 2];
                    float scale = 1.4f;
                    float drawW = fireImg.Width * scale;
                    float drawH = fireImg.Height * scale;
                    float drawX = p.X + p.Width / 2f - drawW / 2f;
                    float drawY = p.Y + p.Height / 2f - drawH / 2f;
                    g.DrawImage(fireImg, drawX, drawY, drawW, drawH);
                }
                else
                {
                    float centerX = p.X + p.Width / 2f;
                    float centerY = p.Y + p.Height / 2f;
                    float drawW = p.Width * ProjectileVisualScale;
                    float drawH = p.Height * ProjectileVisualScale;
                    var visualBounds = new RectangleF(centerX - drawW / 2f, centerY - drawH / 2f, drawW, drawH);

                    using var glow = new SolidBrush(Color.FromArgb(130, 150, 235, 255));
                    g.FillEllipse(glow, visualBounds.X - 3f, visualBounds.Y - 3f, visualBounds.Width + 6f, visualBounds.Height + 6f);
                    using var brush = new SolidBrush(Color.FromArgb(255, 90, 225, 255));
                    g.FillEllipse(brush, visualBounds);
                    using var corePen = new Pen(Color.FromArgb(255, 230, 250, 255), 1.5f);
                    g.DrawEllipse(corePen, visualBounds);
                }
            }
        }

        private void DrawEnemies(Graphics g)
        {
            var player = _gameManager.Player;
            float playerCenterX = player.X + player.Width / 2f;

            foreach (var e in _gameManager.Enemies.Enemies)
            {
                if (!e.IsAlive && e.IsDeathAnimationComplete) continue;

                Image? enemyImage = e is Slime ? AssetLoader.Get(Slime.AssetKey) : null;
                if (enemyImage != null)
                {
                    float drawScale = 1.2f;
                    float drawWidth = e.Width * drawScale;
                    float drawHeight = e.Height * drawScale;
                    float drawX = e.X + (e.Width - drawWidth) / 2f;
                    float drawY = e.Y + (e.Height - drawHeight) / 2f;
                    bool flipLeft = playerCenterX < e.X + e.Width / 2f;

                    var state = g.Save();
                    if (flipLeft)
                    {
                        g.TranslateTransform(drawX + drawWidth, drawY);
                        g.ScaleTransform(-1f, 1f);
                        g.DrawImage(enemyImage, 0, 0, drawWidth, drawHeight);
                    }
                    else
                    {
                        g.DrawImage(enemyImage, drawX, drawY, drawWidth, drawHeight);
                    }
                    g.Restore(state);

                    if (e.IsHurt)
                    {
                        using var flashBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
                        g.FillRectangle(flashBrush, drawX, drawY, drawWidth, drawHeight);
                    }
                    if (e.IsDying)
                    {
                        using var fadeBrush = new SolidBrush(Color.FromArgb(140, 60, 60, 60));
                        g.FillRectangle(fadeBrush, drawX, drawY, drawWidth, drawHeight);
                    }
                }
                else
                {
                    using var body = new SolidBrush(Color.FromArgb(80, 200, 90));
                    using var outline = new Pen(Color.FromArgb(40, 120, 50), 2f);
                    g.FillEllipse(body, e.Bounds);
                    g.DrawEllipse(outline, e.Bounds);
                }

                if (e.IsAlive && !e.IsDying)
                {
                    float hpPercent = (float)e.Health / e.MaxHealth;
                    using var bgBrush = new SolidBrush(Color.FromArgb(120, 40, 40, 40));
                    g.FillRectangle(bgBrush, e.X, e.Y - 10, e.Width, 5);
                    using var hpBrush = new SolidBrush(Color.FromArgb(220, 60, 60));
                    g.FillRectangle(hpBrush, e.X, e.Y - 10, e.Width * hpPercent, 5);
                }
            }
        }

        private void DrawSpiritHud(Graphics g, Size clientSize)
        {
            var spirits = _gameManager.Spirits;
            float startX = 12f, startY = clientSize.Height - 78f;
            float slotW = 210f, slotH = 58f;

            for (int i = 0; i < 2; i++)
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
                    g.DrawString(_localization.Translate("hud.spirit.empty"), emptyFont, emptyBrush, x + 50, startY + 20);
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
                string status; Color statusColor;
                if (spirit.IsActive)
                { status = $"{_localization.Translate("hud.spirit.active")}  {spirit.ActiveRemaining:0.0}s"; statusColor = Color.FromArgb(120, 255, 160); }
                else if (spirit.CooldownRemaining > 0)
                { status = $"{_localization.Translate("hud.spirit.cd")}  {spirit.CooldownRemaining:0.0}s"; statusColor = Color.FromArgb(255, 160, 100); }
                else
                { status = _localization.Translate("hud.spirit.ready"); statusColor = Color.FromArgb(180, 220, 255); }

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

        private void DrawCurrencyHud(Graphics g, Size clientSize)
        {
            var w = _gameManager.Wallet;
            string text = $"{_localization.Translate("hud.currency.gold")} {w.Gold}   " +
                          $"{_localization.Translate("hud.currency.shards")} {w.SpiritShards}   " +
                          $"{_localization.Translate("hud.currency.crystals")} {w.Crystals}";
            using var font = new Font("Consolas", 12f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.FromArgb(255, 220, 140));
            var size = g.MeasureString(text, font);
            g.DrawString(text, font, brush, clientSize.Width - size.Width - 16, 12);

            if (!string.IsNullOrEmpty(_gameManager.StatusMessage))
            {
                using var msgFont = new Font("Consolas", 11f);
                using var msgBrush = new SolidBrush(Color.FromArgb(180, 255, 200));
                var msgSize = g.MeasureString(_gameManager.StatusMessage, msgFont);
                g.DrawString(_gameManager.StatusMessage, msgFont, msgBrush,
                    clientSize.Width - msgSize.Width - 16, 34);
            }
        }

        private void DrawEquipmentHud(Graphics g, Size clientSize)
        {
            var inv = _gameManager.Inventory;
            float x = clientSize.Width - 260, y = 60;
            using var titleFont = new Font("Consolas", 10f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(180, 200, 230));
            g.DrawString(_localization.Translate("hud.equipped.title"), titleFont, titleBrush, x, y);
            y += 18;
            using var itemFont = new Font("Consolas", 9f);
            using var itemBrush = new SolidBrush(Color.FromArgb(200, 210, 230));
            DrawEquipLine(g, itemFont, itemBrush, x, ref y, _localization.Translate("hud.equip.weapon"), inv.GetEquipped(EquipmentSlot.Weapon));
            DrawEquipLine(g, itemFont, itemBrush, x, ref y, _localization.Translate("hud.equip.armor"), inv.GetEquipped(EquipmentSlot.Armor));
            DrawEquipLine(g, itemFont, itemBrush, x, ref y, _localization.Translate("hud.equip.accessory"), inv.GetEquipped(EquipmentSlot.Accessory));
            y += 8;
            g.DrawString(string.Format(_localization.Translate("hud.owned"), inv.Items.Count), itemFont, itemBrush, x, y);
        }

        private void DrawEquipLine(Graphics g, Font font, Brush brush, float x, ref float y, string label, Equipment? item)
        {
            string name = item?.Name ?? _localization.Translate("hud.equip.none");
            string bonus = item == null ? "" : $" +{item.BonusDamage}DMG +{item.BonusMaxHp}HP +{item.BonusDefense}DEF";
            g.DrawString($"{label}: {name}{bonus}", font, brush, x, y);
            y += 16;
        }

        private void DrawDebugInfo(Graphics g, Size clientSize)
        {
            var p = _gameManager.Player;
            var waves = _gameManager.Waves;

            string stageStatus = _gameManager.IsStageCompleted
                ? _localization.Translate("hud.stageClear.pressEsc")
                : $"{_localization.Translate("hud.wave.label")} {waves.CurrentWaveNumber}/{waves.TotalWaves}  |  {_localization.Translate("hud.state.label")} {waves.State}";

            string effects = "";
            if (p.ActiveShield) effects += " [SHIELD]";
            if (p.ActiveFireBoost) effects += $" [FIRE DMG={p.Damage}]";
            if (p.ActiveHealEffect) effects += " [HEAL]";
            if (p.ActiveWindBarrage) effects += $" [WIND +{p.ExtraProjectiles}]";

            string groundedLabel = p.IsGrounded ? _localization.Translate("hud.grounded") : _localization.Translate("hud.air");
            string platState = p.IsGrounded
                ? $"{groundedLabel} [{p.MovementState}]"
                : $"{groundedLabel} [{p.MovementState}] VY={p.VelocityY:0}";

            string animState = $"ANIM: {_playerAnimController.CurrentState}  frame {_playerAnimController.CurrentFrameIndex + 1}";
            string facing = p.Facing == FacingDirection.Right ? "→ Right" : "← Left";
            string speedMode = p.WantsToRun ? "RUN" : "WALK";
            string combatFlags = "";
            if (p.IsAttacking) combatFlags += " ATTACKING";
            if (p.IsFiring) combatFlags += " FIRING";
            if (p.IsHurt) combatFlags += " HURT";
            if (p.IsDead) combatFlags += " DEAD";
            if (p.IsInvulnerable) combatFlags += " INVULN";

            string info =
                $"{_localization.Translate("hud.phase")}\n" +
                $"{_localization.Translate("hud.stage.label")} {waves.StageName}\n" +
                $"{stageStatus}\n" +
                $"{_localization.Translate("hud.hp.label")} {p.CurrentHp}/{p.MaxHp}  {_localization.Translate("hud.dmg.label")} {p.Damage}  {_localization.Translate("hud.def.label")} {p.Defense}{effects}\n" +
                $"{platState}  {facing}  [{speedMode}]\n" +
                $"{animState}{combatFlags}\n" +
                $"{_localization.Translate("hud.enemies.label")} {_gameManager.Enemies.Enemies.Count}  {_localization.Translate("hud.projectiles.label")} {_gameManager.Projectiles.Projectiles.Count}\n" +
                $"{_localization.Translate("hud.controls.line1")}\n" +
                $"{_localization.Translate("hud.controls.line2")}";

            using var font = new Font("Consolas", 11f);
            using var brush = new SolidBrush(Color.FromArgb(200, 220, 255));
            g.DrawString(info, font, brush, 12, 12);

            if (_gameManager.IsStageCompleted)
            {
                using var bigFont = new Font("Consolas", 28f, FontStyle.Bold);
                using var bigBrush = new SolidBrush(Color.FromArgb(100, 255, 150));
                string msg = _localization.Translate("hud.stageClear.big");
                var size = g.MeasureString(msg, bigFont);
                g.DrawString(msg, bigFont, bigBrush, (clientSize.Width - size.Width) / 2, clientSize.Height / 2 - 40);
            }

            if (p.IsDead)
            {
                using var deadFont = new Font("Consolas", 32f, FontStyle.Bold);
                using var deadBrush = new SolidBrush(Color.FromArgb(200, 255, 60, 60));
                string msg = "GAME OVER";
                var size = g.MeasureString(msg, deadFont);
                g.DrawString(msg, deadFont, deadBrush, (clientSize.Width - size.Width) / 2, clientSize.Height / 2 - 80);
                using var hintFont = new Font("Consolas", 14f);
                using var hintBrush = new SolidBrush(Color.FromArgb(200, 220, 255));
                string hint = "Press ESC to exit";
                var hintSize = g.MeasureString(hint, hintFont);
                g.DrawString(hint, hintFont, hintBrush, (clientSize.Width - hintSize.Width) / 2, clientSize.Height / 2 - 30);
            }
        }
    }
}