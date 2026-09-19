using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using ElementalSpirit.Domain.Equipment;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Projectile;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Enemy.NormalEnemy;
using ElementalSpirit.Domain.Skill;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Presentation.Assets;
using ElementalSpirit.Presentation.BossEncounter;

namespace ElementalSpirit.Presentation.Rendering
{

    public class GameRenderer
    {
        private readonly GameManager _gameManager;
        private readonly PlayerAnimationController _playerAnimController;
        private readonly SkillAnimationController _skillAnimController;
        private readonly PortalAnimationController _portalAnimController;
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
            PortalAnimationController portalAnimController,
            Image[]? fireballFrames,
            ILocalizationService localization)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            _playerAnimController = playerAnimController ?? throw new ArgumentNullException(nameof(playerAnimController));
            _skillAnimController = skillAnimController ?? throw new ArgumentNullException(nameof(skillAnimController));
            _portalAnimController = portalAnimController ?? throw new ArgumentNullException(nameof(portalAnimController));
            _fireballFrames = fireballFrames;
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public void Render(Graphics g, Size clientSize)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(18, 22, 32));
            DrawBackground(g, clientSize);

            if (!_gameManager.BossEncounter.IsIntroDialogue)
            {
                DrawPlayer(g);
                DrawProjectiles(g);
                DrawEnemies(g);
                DrawPortals(g);
                DrawSkillHud(g, clientSize);
                DrawCurrencyHud(g, clientSize);
                DrawEquipmentHud(g, clientSize);
                DrawDebugInfo(g, clientSize);
            }

            if (_gameManager.BossEncounter.IsIntroDialogue)
                DrawIntroNarratorText(g);
            else
                DrawBossSpeechBubbles(g);
        }

        private void DrawBackground(Graphics g, Size clientSize)
        {
            string bgName = _gameManager.BossEncounter.ActiveBackgroundImageName ??
                            _gameManager.Waves.StageBackgroundImageName;
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

        private void DrawBossSpeechBubbles(Graphics g)
        {
            var bubble = _gameManager.BossEncounter.ActiveSpeechBubble;
            if (bubble.IsVisible)
            {
                DrawSpeechBubble(g, bubble, isBoss: !bubble.IsPlayerBubble);
            }

            var playerBubble = _gameManager.BossEncounter.PlayerSpeechBubble;
            if (playerBubble.IsVisible)
            {
                DrawSpeechBubble(g, playerBubble, isBoss: false);
            }
        }

        private void DrawIntroNarratorText(Graphics g)
        {
            var bubble = _gameManager.BossEncounter.ActiveSpeechBubble;
            if (!bubble.IsVisible || string.IsNullOrWhiteSpace(bubble.Text)) return;

            using var textFont = new Font("Georgia", 28f, FontStyle.Italic);
            using var shadowBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0));
            using var textBrush = new SolidBrush(Color.FromArgb(240, 255, 240, 200));
            using var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var textRect = new RectangleF(90f, 145f, 1100f, 250f);
            var shadowRect = textRect;
            shadowRect.Offset(3f, 3f);

            g.DrawString(bubble.Text, textFont, shadowBrush, shadowRect, format);
            g.DrawString(bubble.Text, textFont, textBrush, textRect, format);

            using var hintFont = new Font("Consolas", 9.5f);
            using var hintBrush = new SolidBrush(Color.FromArgb(160, 180, 210, 255));
            const string hint = "Nhấn Space để xem tiếp";
            var hintSize = g.MeasureString(hint, hintFont);
            g.DrawString(
                hint,
                hintFont,
                hintBrush,
                (g.VisibleClipBounds.Width - hintSize.Width) / 2f,
                g.VisibleClipBounds.Height - 30f);
        }

        private void DrawSpeechBubble(Graphics g, BossSpeechBubble bubble, bool isBoss)
        {
            const float paddingX = 14f;
            const float paddingY = 12f;
            const float lineSpacing = 3f;

            using var nameFont = new Font("Segoe UI", 11f, FontStyle.Bold);
            using var textFont = new Font("Segoe UI", 10f, FontStyle.Regular);
            using var hintFont = new Font("Segoe UI", 8f, FontStyle.Italic);
            using var nameBrush = new SolidBrush(isBoss ? Color.FromArgb(255, 255, 90, 90) : Color.FromArgb(255, 120, 200, 255));
            using var textBrush = new SolidBrush(Color.White);
            using var hintBrush = new SolidBrush(Color.FromArgb(180, 220, 220, 220));
            using var backgroundBrush = new SolidBrush(Color.FromArgb(220, 20, 20, 20));
            using var borderPen = new Pen(isBoss ? Color.FromArgb(255, 120, 40, 40) : Color.FromArgb(255, 80, 110, 160), 2.5f);
            using var format = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };

            string displayText = bubble.Text.Trim();
            string[] lines = WrapTextForMeasure(displayText, 32);
            string hint = "Nhấn Space để xem tiếp";

            float nameHeight = !string.IsNullOrEmpty(bubble.Speaker) ? nameFont.GetHeight() + 8f : 0f;
            float textHeight = 0f;
            foreach (string line in lines)
            {
                textHeight += textFont.GetHeight() + lineSpacing;
            }
            float hintHeight = hintFont.GetHeight() + 4f;
            float width = Math.Max(240f, MeasureLongestLine(lines, textFont) + paddingX * 2f);
            float height = paddingY + nameHeight + textHeight + hintHeight + 8f;

            var rect = new RectangleF(bubble.X, bubble.Y, width, height);
            using var roundedPath = CreateRoundedRectangle(rect, 8f);

            g.FillPath(backgroundBrush, roundedPath);
            g.DrawPath(borderPen, roundedPath);

            float currentY = rect.Y + paddingY;
            if (!string.IsNullOrEmpty(bubble.Speaker))
            {
                g.DrawString(bubble.Speaker, nameFont, nameBrush, rect.X + paddingX, currentY, format);
                currentY += nameFont.GetHeight() + 6f;
            }

            foreach (string line in lines)
            {
                g.DrawString(line, textFont, textBrush, rect.X + paddingX, currentY, format);
                currentY += textFont.GetHeight() + lineSpacing;
            }

            g.DrawString(hint, hintFont, hintBrush, rect.X + paddingX, currentY + 2f, format);
        }

        private static GraphicsPath CreateRoundedRectangle(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float diameter = radius * 2f;
            var arcRect = new RectangleF(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arcRect, 180, 90);
            path.AddArc(new RectangleF(rect.Right - diameter, rect.Y, diameter, diameter), 270, 90);
            path.AddArc(new RectangleF(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter), 0, 90);
            path.AddArc(new RectangleF(rect.X, rect.Bottom - diameter, diameter, diameter), 90, 90);
            path.CloseFigure();
            return path;
        }

        private static string[] WrapTextForMeasure(string text, int maxCharsPerLine)
        {
            if (string.IsNullOrEmpty(text)) return new[] { "" };

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var lines = new List<string>();
            string current = string.Empty;

            foreach (var word in words)
            {
                string candidate = string.IsNullOrEmpty(current) ? word : current + " " + word;
                if (candidate.Length <= maxCharsPerLine)
                {
                    current = candidate;
                }
                else
                {
                    if (!string.IsNullOrEmpty(current))
                        lines.Add(current);
                    current = word;
                }
            }

            if (!string.IsNullOrEmpty(current))
                lines.Add(current);

            return lines.Count == 0 ? new[] { text } : lines.ToArray();
        }

        private static float MeasureLongestLine(string[] lines, Font font)
        {
            using var g = Graphics.FromHwnd(IntPtr.Zero);
            float longest = 0f;
            foreach (string line in lines)
            {
                var size = g.MeasureString(line, font);
                if (size.Width > longest)
                    longest = size.Width;
            }

            return longest;
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

            // The gameplay hitbox is intentionally small, while the animation
            // frame is larger. Keep the visible feet on the same ground line.
            /*bool bossDialogueActive = _gameManager.BossEncounter.CurrentState ==
                                      Domain.BossEncounter.BossEncounterState.PreBossDialogue;
            if (bossDialogueActive || p.IsGrounded)
            {
                feetY = _gameManager.GroundY;
                anchorRatioY = 1f;
            }*/
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

            DrawPlayerHealthBar(g, p, drawY);
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

        private void DrawPlayerHealthBar(Graphics g, Player p, float visualTopY)
        {
            if (p.IsDead) return;

            float centerX = p.X + p.Width / 2f;
            centerX += p.Facing == FacingDirection.Left ? HealthBarVisualOffsetX : -HealthBarVisualOffsetX;
            float barX = centerX - HealthBarWidth / 2f;
            float barY = visualTopY - HealthBarVerticalGap;

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
                if (p is EnemyProjectile enemyProjectile)
                {
                    if (enemyProjectile.Type == EnemyProjectileType.NuclearExplosion)
                    {
                        string[] nuclearFrames = Enumerable.Range(1, 10)
                            .Select(index => $"Enemies/BossSkill/Nuclear_explosion/Nuclear_explosion{index}.png")
                            .ToArray();
                        int nuclearFrameIndex = (int)(Math.Abs(enemyProjectile.Lifetime * 12f)) % nuclearFrames.Length;
                        var nuclearImage = AssetLoader.Get(nuclearFrames[nuclearFrameIndex]);
                        const float nuclearVisualSize = 72f;
                        float nuclearX = enemyProjectile.X + enemyProjectile.Width / 2f - nuclearVisualSize / 2f;
                        float nuclearY = enemyProjectile.Y + enemyProjectile.Height - nuclearVisualSize;

                        if (nuclearImage != null)
                        {
                            g.DrawImage(nuclearImage, nuclearX, nuclearY, nuclearVisualSize, nuclearVisualSize);
                            continue;
                        }
                    }

                    string[] bossSkillFrames =
                    {
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle1.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle2.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle3.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle4.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle5.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle6.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle7.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle8.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle9.png",
                        "Enemies/BossSkill/Explosion_blue_circle/Explosion_blue_circle10.png"
                    };

                    int frameIndex = (int)(Math.Abs(p.Lifetime * 10f)) % bossSkillFrames.Length;
                    var bossSkillImage = AssetLoader.Get(bossSkillFrames[frameIndex]);
                    float drawW = Math.Max(28f, p.Width * 2.2f);
                    float drawH = drawW;
                    float drawX = p.X + p.Width / 2f - drawW / 2f;
                    float drawY = p.Y + p.Height / 2f - drawH / 2f;

                    if (bossSkillImage != null)
                    {
                        g.DrawImage(bossSkillImage, drawX, drawY, drawW, drawH);
                        continue;
                    }

                    using var glowBrush = new SolidBrush(Color.FromArgb(120, 120, 220, 255));
                    g.FillEllipse(glowBrush, drawX - 6f, drawY - 6f, drawW + 12f, drawH + 12f);
                    using var orbBrush = new SolidBrush(Color.FromArgb(220, 90, 200, 255));
                    g.FillEllipse(orbBrush, drawX, drawY, drawW, drawH);
                    using var innerPen = new Pen(Color.FromArgb(200, 230, 245, 255), 2f);
                    g.DrawEllipse(innerPen, drawX + 5f, drawY + 5f, drawW - 10f, drawH - 10f);
                    continue;
                }

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
                    enemyImage = slime.CurrentImage ?? AssetLoader.Get(Slime.AssetKey);
                }
                else if (e is GorgonBoss gorgon)
                {
                    enemyImage = gorgon.CurrentImage ?? AssetLoader.Get("Enemies/Boss/Gorgon_1/Idle.png")
                        ?? AssetLoader.Get("Boss/Gorgon_1/Idle.png")
                        ?? AssetLoader.Get("Gorgon_1/Idle.png");
                }

                if (enemyImage != null)
                {
                    float targetSize = Math.Max(e.Width, e.Height) * 1.8f;
                    float drawWidth = targetSize;
                    float drawHeight = targetSize;
                    float drawX = e.X + e.Width / 2f - drawWidth / 2f;
                    float drawY = e.Y + e.Height - drawHeight * 0.85f;

                    bool flipLeft = e is Slime s
                        ? s.Facing == Domain.Player.FacingDirection.Left
                        : e is GorgonBoss boss
                            ? boss.Facing == Domain.Player.FacingDirection.Left
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
                    float hpPercent = e.MaxHealth > 0 ? (float)e.Health / e.MaxHealth : 0f;
                    float barWidth = e is GorgonBoss ? e.Width * 1.25f : e.Width;
                    float barX = e.X + (e.Width - barWidth) / 2f;
                    float barY = e.Y - 14f;

                    using var bgBrush = new SolidBrush(Color.FromArgb(120, 40, 40, 40));
                    g.FillRectangle(bgBrush, barX, barY, barWidth, 6);
                    using var hpBrush = new SolidBrush(Color.FromArgb(220, 60, 60));
                    g.FillRectangle(hpBrush, barX, barY, barWidth * hpPercent, 6);
                    using var border = new Pen(Color.FromArgb(180, 240, 240, 240), 1f);
                    g.DrawRectangle(border, barX, barY, barWidth, 6);
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
                    icon.Dispose();
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

        private void DrawPortals(Graphics g)
        {
            foreach (var portal in _gameManager.Portals.Portals)
            {
                if (!portal.IsVisible) continue;

                var bounds = portal.Bounds;
                var portalImage = _portalAnimController.CurrentImage;

                if (portalImage != null)
                {
                    float drawWidth = bounds.Width + 14f;
                    float drawHeight = bounds.Height + 10f;
                    float drawX = bounds.X - 7f;
                    float drawY = bounds.Y - 5f;

                    g.DrawImage(portalImage, drawX, drawY, drawWidth, drawHeight);
                    continue;
                }

                bool isForward = portal.Type == PortalType.ForwardPortal;
                Color portalColor = isForward
                    ? Color.FromArgb(100, 190, 255)
                    : Color.FromArgb(110, 255, 165);
                float pulse = 0.5f + 0.5f *
                    (float)Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds * 3f);

                using var fill = new SolidBrush(Color.FromArgb(45, portalColor));
                using var glow = new Pen(Color.FromArgb((int)(170 + pulse * 70), portalColor), 4f);
                using var border = new Pen(Color.FromArgb(230, portalColor), 2f);

                g.FillRectangle(fill, bounds);
                g.DrawRectangle(glow, bounds);
                g.DrawRectangle(border, bounds.X + 4f, bounds.Y + 4f,
                    bounds.Width - 8f, bounds.Height - 8f);

                for (int i = 0; i < 6; i++)
                {
                    float t = (float)((DateTime.Now.TimeOfDay.TotalSeconds * 0.6f + i / 6f) % 1f);
                    float particleY = bounds.Bottom - t * bounds.Height;
                    float particleX = bounds.X + bounds.Width / 2f +
                        (float)Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds * 2f + i) * 14f;
                    using var particle = new SolidBrush(Color.FromArgb(210, portalColor));
                    g.FillEllipse(particle, particleX - 3f, particleY - 3f, 6f, 6f);
                }
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