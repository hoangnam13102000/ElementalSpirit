using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace ElementalSpirit.Presentation.Assets
{
    public static class AssetLoader
    {
        private static readonly Dictionary<string, Image> _cache = new();

        private static string ImagesFolder =>
            Path.Combine(AppContext.BaseDirectory, "Resources", "Images");

        public static Image? Get(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Debug.WriteLine("[AssetLoader] fileName is null/empty.");
                return null;
            }

            if (_cache.TryGetValue(fileName, out var cached))
                return cached;

            string path = Path.Combine(ImagesFolder, fileName);

            if (!File.Exists(path))
            {
                // Not directly under Resources\Images — search subfolders
                // (e.g. Characters\, Backgrounds\) for a matching file name.
                string? found = null;
                if (Directory.Exists(ImagesFolder))
                {
                    var matches = Directory.GetFiles(ImagesFolder, fileName, SearchOption.AllDirectories);
                    if (matches.Length > 0)
                        found = matches[0];
                }

                if (found == null)
                {
                    Debug.WriteLine($"[AssetLoader] NOT FOUND: '{path}' (BaseDirectory='{AppContext.BaseDirectory}')");
                    return null;
                }

                path = found;
            }

            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using var raw = Image.FromStream(stream);
                var image = new Bitmap(raw);

                _cache[fileName] = image;
                Debug.WriteLine($"[AssetLoader] LOADED OK: '{path}' ({image.Width}x{image.Height})");
                return image;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AssetLoader] EXCEPTION loading '{path}': {ex}");
                return null;
            }
        }

        public static void DisposeAll()
        {
            foreach (var img in _cache.Values)
                img.Dispose();
            _cache.Clear();
        }
    }
}