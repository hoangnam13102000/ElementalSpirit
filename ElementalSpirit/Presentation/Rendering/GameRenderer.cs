using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using ElementalSpirit.Domain.Equipment;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Projectile;
using ElementalSpirit.Domain.Enemy.NormalEnemy;
using ElementalSpirit.Domain.Skill;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;

namespace ElementalSpirit.Presentation.Rendering
{

    public class GameRenderer
    {
        private readonly GameManager _gameManager;
        private readonly PlayerAnimationController _playerAnimController;
        private readonly SkillAnimationController _skillAnimController;
        private readonly Image[]? _fireballFrames;
        private readonly ILocalizationService _localization;
        private readonly Dictionary<string, Rectangle> _visibleAssetBounds = new();
        private readonly Dictionary<Image, Rectangle> _visibleImageBounds = new();

        private const float BaseRenderScale = 0.9f;
        private const int StandardFrameSize = 128;
        private const int DeathFrameSize = 256;
        private const float ProjectileVisualScale = 2.2f;

        private string _loadedBackgroundName = "";
        private Image? _backgroundImage;
        private string _loadedIncomingBackgroundName = "";
        private Image? _incomingBackgroundImage;

        public GameRenderer(
            GameManager gameManager,
            PlayerAnimationController playerAnimController,
            SkillAnimationController skillAnimController,
            Image[]? fireballFrames,
            ILocalizationService localization)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            _playerAnimController = playerAnimController ?? throw new ArgumentNullException(nameof(playerAnimController));
            _skillAnimController = skillAnimController ?? throw new ArgumentNullException(nameof(skillAnimController));
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
            DrawSkillHud(g, clientSize);
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

            // Trong lúc "Fading" (chuyển cảnh sang background kế tiếp), crossfade dần
            // ảnh background mới đè lên ảnh hiện tại theo TransitionProgress (0..1).
            string? incomingName = _gameManager.IncomingBackgroundImageName;
            if (_gameManager.TransitionPhase == StageTransitionPhase.Fading && !string.IsNullOrEmpty(incomingName))
            {
                if (incomingName != _loadedIncomingBackgroundName)
                {
                    _incomingBackgroundImage = AssetLoader.Get(incomingName);
                    _loadedIncomingBackgroundName = incomingName;
                }

                if (_incomingBackgroundImage != null)
                {
                    DrawImageWithOpacity(
                        g,
                        _incomingBackgroundImage,
                        new Rectangle(0, 0, clientSize.Width, clientSize.Height),
                        _gameManager.TransitionProgress);
                }
            }
        }

        private static void DrawImageWithOpacity(Graphics g, Image image, Rectangle destRect, float opacity)
        {
            opacity = Math.Clamp(opacity, 0f, 1f);
            if (opacity <= 0f) return;

            var colorMatrix = new ColorMatrix { Matrix33 = opacity };
            using var attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            g.DrawImage(
                image,
                destRect,
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel,
                attributes);
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
            if (_gameManager.Skills.CurrentAnimationState == Domain.Skill.SkillAnimationState.Waterfall &&
                _skillAnimController.CurrentImage != null)
            {
                var image = _skillAnimController.CurrentImage;
                Rectangle sourceBounds = GetVisibleAssetBounds(image);
                float direction = p.Facing == FacingDirection.Right ? 1f : -1f;
                float baseX = p.X + p.Width / 2f + direction * WaterfallSkill.ColumnSpacing;
                float columnWidth = WaterfallSkill.ColumnWidth;
                float columnHeight = WaterfallSkill.ColumnHeight;
                float columnY = p.Y + p.Height - columnHeight;
                int activeColumnCount = _gameManager.Skills.ActiveWaterfall?.ActiveColumnCount
                    ?? WaterfallSkill.ColumnCount;

                for (int column = 0; column < activeColumnCount; column++)
                {
                    float centerX = baseX + direction * column * WaterfallSkill.ColumnSpacing;
                    g.DrawImage(
                        image,
                        new Rectangle(
                            (int)(centerX - columnWidth / 2f),
                            (int)columnY,
                            (int)columnWidth,
                            (int)columnHeight),
                        sourceBounds.X,
                        sourceBounds.Y,
                        sourceBounds.Width,
                        sourceBounds.Height,
                        GraphicsUnit.Pixel);
                }
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
                else if (p is PlayerProjectile slash && slash.Type == ProjectileType.Slash)
                {
                    var slashImage = AssetLoader.Get(
                        "Characters/Skill/Slash/WindSlash/Double Wind Slashes_Frame_01.png");
                    if (slashImage != null)
                    {
                        Rectangle sourceBounds = GetVisibleAssetBounds(
                            "Characters/Skill/Slash/WindSlash/Double Wind Slashes_Frame_01.png",
                            slashImage);
                        float drawW = 58f;
                        float drawH = 58f;
                        float centerX = slash.X + slash.Width / 2f;
                        float centerY = slash.Y + slash.Height / 2f;
                        var state = g.Save();
                        g.TranslateTransform(centerX, centerY);
                        if (slash.HorizontalDirection > 0f)
                            g.ScaleTransform(-1f, 1f);
                        g.RotateTransform(90f);
                        g.DrawImage(
                            slashImage,
                            new Rectangle(
                                (int)-drawW / 2,
                                (int)-drawH / 2,
                                (int)drawW,
                                (int)drawH),
                            sourceBounds.X,
                            sourceBounds.Y,
                            sourceBounds.Width,
                            sourceBounds.Height,
                            GraphicsUnit.Pixel);
                        g.Restore(state);
                    }
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

                Image? enemyImage = null;
                if (e is Slime slime)
                {
                    // Prefer animated frame; fall back to legacy static sprite.
                    enemyImage = slime.CurrentImage ?? AssetLoader.Get(Slime.AssetKey);
                }

                if (enemyImage != null)
                {
                    // Animated frames are 128x128; scale to roughly match entity bounds.
                    float targetSize = Math.Max(e.Width, e.Height) * 1.8f;
                    float drawWidth = targetSize;
                    float drawHeight = targetSize;
                    float drawX = e.X + e.Width / 2f - drawWidth / 2f;
                    float drawY = e.Y + e.Height - drawHeight * 0.85f; // feet-ish anchor

                    // Use slime facing direction for consistent flip with movement.
                    bool flipLeft = e is Slime s
                        ? s.Facing == Domain.Player.FacingDirection.Left
                        : playerCenterX < e.X + e.Width / 2f;

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

                    // Hurt flash only when not already playing dedicated Hurt anim frames
                    if (e.IsHurt && e is Slime { CurrentAnimState: not SlimeAnimationState.Hurt })
                    {
                        using var flashBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
                        g.FillRectangle(flashBrush, drawX, drawY, drawWidth, drawHeight);
                    }
                    if (e.IsDying && e is Slime { CurrentAnimState: not SlimeAnimationState.Dead })
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

        private void DrawSkillHud(Graphics g, Size clientSize)
        {
            var skills = _gameManager.Skills.Skills;
            const int slotCount = 4;
            const float slotSize = 72f;
            const float slotGap = 8f;
            float totalWidth = slotCount * slotSize + (slotCount - 1) * slotGap;
            float startX = (clientSize.Width - totalWidth) / 2f;
            float startY = clientSize.Height - slotSize - 16f;

            for (int i = 0; i < slotCount; i++)
            {
                float x = startX + i * (slotSize + slotGap);
                var skill = i < skills.Count ? skills[i] : null;
                using var bg = new SolidBrush(Color.FromArgb(210, 16, 20, 30));
                using var border = new Pen(
                    skill?.IsActive == true
                        ? Color.FromArgb(255, 240, 210, 90)
                        : Color.FromArgb(180, 110, 145, 190),
                    skill?.IsActive == true ? 2f : 1.5f);
                g.FillRectangle(bg, x, startY, slotSize, slotSize);
                g.DrawRectangle(border, x, startY, slotSize, slotSize);

                if (skill == null)
                {
                    using var emptyBrush = new SolidBrush(Color.FromArgb(90, 100, 115));
                    g.FillRectangle(emptyBrush, x + 12, startY + 12, slotSize - 24, slotSize - 24);
                    DrawSkillKey(g, i + 1, x, startY);
                    continue;
                }

                var icon = AssetLoader.Get(skill.IconAssetKey);
                if (icon != null)
                {
                    Rectangle sourceBounds = GetVisibleAssetBounds(skill.IconAssetKey, icon);
                    float iconSize = (slotSize - 8f) * Math.Clamp(skill.IconScale, 0.4f, 1f);
                    float iconX = x + (slotSize - iconSize) / 2f;
                    float iconY = startY + (slotSize - iconSize) / 2f;
                    g.DrawImage(
                        icon,
                        new Rectangle(
                            (int)iconX,
                            (int)iconY,
                            (int)iconSize,
                            (int)iconSize),
                        sourceBounds.X,
                        sourceBounds.Y,
                        sourceBounds.Width,
                        sourceBounds.Height,
                        GraphicsUnit.Pixel);
                }
                else if (string.IsNullOrEmpty(skill.IconAssetKey))
                {
                    DrawBasicProjectileIcon(g, x, startY, slotSize);
                }

                DrawSkillCooldown(g, skill, x, startY, slotSize);
                DrawSkillKey(g, i + 1, x, startY);
                using var nameFont = new Font("Consolas", 8f, FontStyle.Bold);
                using var nameBrush = new SolidBrush(Color.White);
                var nameSize = g.MeasureString(skill.Name, nameFont);
                g.DrawString(skill.Name, nameFont, nameBrush,
                    x + (slotSize - nameSize.Width) / 2f, startY + slotSize - 15f);
            }
        }

        private static void DrawSkillKey(Graphics g, int key, float x, float y)
        {
            using var keyFont = new Font("Consolas", 10f, FontStyle.Bold);
            using var keyBrush = new SolidBrush(Color.White);
            g.DrawString(key.ToString(), keyFont, keyBrush, x + 5f, y + 4f);
        }

        private static void DrawBasicProjectileIcon(Graphics g, float x, float y, float slotSize)
        {
            float size = slotSize * 0.3f;
            float centerX = x + slotSize / 2f;
            float centerY = y + slotSize / 2f - 2f;
            var bounds = new RectangleF(
                centerX - size / 2f,
                centerY - size / 2f,
                size,
                size);

            using var glow = new SolidBrush(Color.FromArgb(120, 100, 220, 255));
            g.FillEllipse(glow, bounds.X - 5f, bounds.Y - 5f, bounds.Width + 10f, bounds.Height + 10f);
            using var core = new SolidBrush(Color.FromArgb(255, 90, 225, 255));
            g.FillEllipse(core, bounds);
            using var outline = new Pen(Color.FromArgb(255, 225, 250, 255), 2f);
            g.DrawEllipse(outline, bounds);
        }

        private static void DrawSkillCooldown(
            Graphics g,
            Domain.Skill.ISkill skill,
            float x,
            float y,
            float slotSize)
        {
            if (skill.CooldownRemaining <= 0f) return;

            using var overlay = new SolidBrush(Color.FromArgb(125, 5, 8, 14));
            g.FillRectangle(overlay, x, y, slotSize, slotSize);

            using var clockPen = new Pen(Color.FromArgb(230, 220, 235, 255), 2f);
            float clockSize = 28f;
            float clockX = x + (slotSize - clockSize) / 2f;
            float clockY = y + 10f;
            g.DrawEllipse(clockPen, clockX, clockY, clockSize, clockSize);
            g.DrawLine(
                clockPen,
                clockX + clockSize / 2f,
                clockY + clockSize / 2f,
                clockX + clockSize / 2f,
                clockY + 6f);
            g.DrawLine(
                clockPen,
                clockX + clockSize / 2f,
                clockY + clockSize / 2f,
                clockX + clockSize - 7f,
                clockY + clockSize / 2f);

            using var timeFont = new Font("Consolas", 10f, FontStyle.Bold);
            using var timeBrush = new SolidBrush(Color.White);
            string timeText = $"{skill.CooldownRemaining:0.0}s";
            var timeSize = g.MeasureString(timeText, timeFont);
            g.DrawString(
                timeText,
                timeFont,
                timeBrush,
                x + (slotSize - timeSize.Width) / 2f,
                y + 43f);
        }

        private Rectangle GetVisibleAssetBounds(string assetKey, Image image)
        {
            if (_visibleAssetBounds.TryGetValue(assetKey, out var cached))
                return cached;

            if (image is not Bitmap bitmap)
                return new Rectangle(0, 0, image.Width, image.Height);

            var bounds = FindVisibleAssetBounds(bitmap);
            _visibleAssetBounds[assetKey] = bounds;
            return bounds;
        }

        private Rectangle GetVisibleAssetBounds(Image image)
        {
            if (_visibleImageBounds.TryGetValue(image, out var cached))
                return cached;

            var bounds = image is Bitmap bitmap
                ? FindVisibleAssetBounds(bitmap)
                : new Rectangle(0, 0, image.Width, image.Height);
            _visibleImageBounds[image] = bounds;
            return bounds;
        }

        private static Rectangle FindVisibleAssetBounds(Bitmap bitmap)
        {
            int minX = bitmap.Width;
            int minY = bitmap.Height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y).A < 16) continue;
                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }

            return maxX < 0
                ? new Rectangle(0, 0, bitmap.Width, bitmap.Height)
                : Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
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

            string stageStatus;
            if (_gameManager.IsInStageTransition)
            {
                stageStatus = $"{_localization.Translate("hud.stageTransition")} ({_gameManager.TransitionPhase})";
            }
            else if (_gameManager.IsStageCompleted)
            {
                stageStatus = _localization.Translate("hud.stageClear.pressEsc");
            }
            else
            {
                stageStatus = $"{_localization.Translate("hud.wave.label")} {waves.CurrentWaveNumber}/{waves.TotalWaves}  |  {_localization.Translate("hud.state.label")} {waves.State}";
            }

            string info =
                $"{_localization.Translate("hud.phase")}\n" +
                $"{_localization.Translate("hud.stage.label")} {waves.StageName}\n" +
                $"{stageStatus}\n" +
                $"{_localization.Translate("hud.hp.label")} {p.CurrentHp}/{p.MaxHp}\n" +
                $"{_localization.Translate("hud.currency.gold")} {_gameManager.Wallet.Gold}\n" +
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