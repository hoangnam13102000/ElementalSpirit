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
            {
                try
                {
                    return (Image)cached.Clone();
                }
                catch (ArgumentException)
                {
                    // A previous caller may have disposed an image from an older cache version.
                    _cache.Remove(fileName);
                    cached.Dispose();
                }
            }

            string path = Path.Combine(ImagesFolder, fileName);

            if (!File.Exists(path))
            {
                // Tìm kiếm đệ quy CHỈ theo TÊN FILE (bỏ đường dẫn)
                string? found = null;
                if (Directory.Exists(ImagesFolder))
                {
                    string searchFileName = Path.GetFileName(fileName);
                    Debug.WriteLine($"[AssetLoader] Searching for: {searchFileName} in {ImagesFolder}");

                    var matches = Directory.GetFiles(ImagesFolder, searchFileName, SearchOption.AllDirectories);
                    if (matches.Length > 0)
                    {
                        found = matches[0];
                        Debug.WriteLine($"[AssetLoader] FOUND at: {found}");
                    }
                    else
                    {
                        Debug.WriteLine($"[AssetLoader] NOT FOUND file: {searchFileName}");
                    }
                }
                path = found ?? path;
            }
            else
            {
                Debug.WriteLine($"[AssetLoader] Direct load: '{path}'");
            }

            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using var raw = Image.FromStream(stream);
                var image = new Bitmap(raw);
                _cache[fileName] = image;
                Debug.WriteLine($"[AssetLoader] LOADED OK: {fileName} ({image.Width}x{image.Height})");
                return (Image)image.Clone();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AssetLoader] ERROR: {ex.GetType().Name} - {ex.Message}");
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