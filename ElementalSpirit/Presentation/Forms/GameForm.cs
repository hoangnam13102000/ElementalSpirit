using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ElementalSpirit.GameEngine;

namespace ElementalSpirit.Presentation.Forms
{
    public class GameForm : Form
    {
        private readonly GameManager _gameManager;
        private readonly GameTimer _gameTimer;

        public GameForm()
        {
            Text = "Elemental Spirit - Phase 3 (Wave System)";
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
            _gameManager.HandleKeyDown(e.KeyCode);

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
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
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.Clear(Color.FromArgb(20, 24, 36));

            DrawGrid(g);
            DrawEnemies(g);
            DrawPlayer(g);
            DrawProjectiles(g);
            DrawDebugInfo(g);
        }

        private void DrawGrid(Graphics g)
        {
            using var pen = new Pen(Color.FromArgb(40, 50, 70), 1f);
            const int step = 64;

            for (int x = 0; x < ClientSize.Width; x += step)
                g.DrawLine(pen, x, 0, x, ClientSize.Height);

            for (int y = 0; y < ClientSize.Height; y += step)
                g.DrawLine(pen, 0, y, ClientSize.Width, y);
        }

        private void DrawPlayer(Graphics g)
        {
            var p = _gameManager.Player;
            var rect = new RectangleF(p.X, p.Y, p.Width, p.Height);

            using var bodyBrush = new SolidBrush(Color.FromArgb(80, 160, 255));
            using var outlinePen = new Pen(Color.FromArgb(180, 220, 255), 2f);
            using var coreBrush = new SolidBrush(Color.FromArgb(200, 230, 255));

            g.FillEllipse(bodyBrush, rect);
            g.DrawEllipse(outlinePen, rect);

            float coreSize = p.Width * 0.45f;
            var coreRect = new RectangleF(
                p.X + (p.Width - coreSize) / 2,
                p.Y + (p.Height - coreSize) / 2,
                coreSize, coreSize);
            g.FillEllipse(coreBrush, coreRect);
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
            foreach (var e in _gameManager.Enemies.Enemies)
            {
                if (!e.IsAlive) continue;

                // Thân slime
                using var body = new SolidBrush(Color.FromArgb(80, 200, 90));
                using var outline = new Pen(Color.FromArgb(40, 120, 50), 2f);
                g.FillEllipse(body, e.Bounds);
                g.DrawEllipse(outline, e.Bounds);

                // Health bar
                float hpPercent = (float)e.Health / e.MaxHealth;
                var hpRect = new RectangleF(e.X, e.Y - 10, e.Width * hpPercent, 5);
                using var hpBrush = new SolidBrush(Color.FromArgb(220, 60, 60));
                g.FillRectangle(hpBrush, hpRect);
            }
        }

        private void DrawDebugInfo(Graphics g)
        {
            var p = _gameManager.Player;
            var waves = _gameManager.Waves;

            string stageStatus = _gameManager.IsStageCompleted
                ? "STAGE CLEAR! (Press ESC)"
                : $"Wave {waves.CurrentWaveNumber}/{waves.TotalWaves}  |  State: {waves.State}";

            string info =
                $"Phase 3 – Wave System\n" +
                $"Stage: {waves.StageName}\n" +
                $"{stageStatus}\n" +
                $"Pos: ({p.X:F0}, {p.Y:F0})\n" +
                $"HP: {p.CurrentHp}/{p.MaxHp}\n" +
                $"Enemies: {_gameManager.Enemies.Enemies.Count}\n" +
                $"Projectiles: {_gameManager.Projectiles.Projectiles.Count}\n" +
                $"Move: WASD | Shoot: Space | N: Next Wave (test) | ESC: Exit";

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
                    (ClientSize.Width - size.Width) / 2,
                    ClientSize.Height / 2 - 40);
            }
        }
    }
}