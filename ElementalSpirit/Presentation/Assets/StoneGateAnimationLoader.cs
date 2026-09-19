using System;
using System.Collections.Generic;
using System.Drawing;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Presentation.Assets
{
    public static class StoneGateAnimationLoader
    {
        private static readonly List<Image> _frameCache = new();
        private static bool _loaded;

        public static PortalAnimationController CreateController()
        {
            EnsureLoaded();
            return new PortalAnimationController(_frameCache.ToArray(), fps: 8f, isLooping: true);
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;

            var sheet = AssetLoader.Get("Backgrounds/stone_gate.png");
            if (sheet == null)
            {
                _loaded = true;
                return;
            }

            try
            {
                const int cols = 4;
                const int rows = 4;
                int cellWidth = sheet.Width / cols;
                int cellHeight = sheet.Height / rows;

                if (cellWidth <= 0 || cellHeight <= 0)
                {
                    _loaded = true;
                    return;
                }

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        var frame = new Bitmap(cellWidth, cellHeight);
                        using var g = Graphics.FromImage(frame);
                        g.DrawImage(
                            sheet,
                            new Rectangle(0, 0, cellWidth, cellHeight),
                            new Rectangle(col * cellWidth, row * cellHeight, cellWidth, cellHeight),
                            GraphicsUnit.Pixel);

                        _frameCache.Add(frame);
                    }
                }
            }
            finally
            {
                _loaded = true;
            }
        }

        public static void DisposeAll()
        {
            foreach (var img in _frameCache)
                img.Dispose();
            _frameCache.Clear();
            _loaded = false;
        }
    }
}
