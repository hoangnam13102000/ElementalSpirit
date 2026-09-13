using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ElementalSpirit.Domain.Enemy.NormalEnemy;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Presentation.Assets
{
    public static class SlimeAnimationLoader
    {
        public const int FrameSize = 128;

        private static readonly Dictionary<SlimeAnimationState, Image[]> _frameCache = new();
        private static readonly Dictionary<SlimeAnimationState, (float fps, bool looping)> _defs = new()
        {
            { SlimeAnimationState.Idle,      (10f, true) },
            { SlimeAnimationState.Walk,      (10f, true) },
            { SlimeAnimationState.Run,       (12f, true) },
            { SlimeAnimationState.Jump,      (12f, false) },
            { SlimeAnimationState.Attack,    (12f, false) },
            { SlimeAnimationState.RunAttack, (12f, false) },
            { SlimeAnimationState.Hurt,      (10f, false) },
            { SlimeAnimationState.Dead,      (8f,  false) },
        };

        private static readonly (SlimeAnimationState state, string fileName)[] _sheetMap =
        {
            (SlimeAnimationState.Idle,      "Idle.png"),
            (SlimeAnimationState.Walk,      "Walk.png"),
            (SlimeAnimationState.Run,       "Run.png"),
            (SlimeAnimationState.Jump,      "Jump.png"),
            (SlimeAnimationState.Attack,    "Attack_1.png"),
            (SlimeAnimationState.RunAttack, "Run+Attack.png"),
            (SlimeAnimationState.Hurt,      "Hurt.png"),
            (SlimeAnimationState.Dead,      "Dead.png"),
        };

        private static bool _loaded;

        private static string SlimeRoot =>
            Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "Enemies", "Green_Slime");

        public static SlimeAnimationController CreateController()
        {
            EnsureLoaded();
            var clips = new Dictionary<SlimeAnimationState, AnimationClip>();
            foreach (var kv in _frameCache)
            {
                if (!_defs.TryGetValue(kv.Key, out var def)) continue;
                clips[kv.Key] = new AnimationClip(kv.Value, def.fps, def.looping);
            }
            if (clips.Count == 0)
                throw new InvalidOperationException("No slime animation clips loaded.");

            return new SlimeAnimationController(clips, SlimeAnimationState.Idle);
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;

            foreach (var (state, fileName) in _sheetMap)
            {
                try
                {
                    string path = Path.Combine(SlimeRoot, fileName);
                    var frames = SliceSpriteSheet(path);
                    if (frames.Length == 0)
                    {
                        Debug.WriteLine($"[SlimeAnimationLoader] WARNING: No frames for {state}");
                        continue;
                    }
                    _frameCache[state] = frames;
                    Debug.WriteLine($"[SlimeAnimationLoader] Loaded {state}: {frames.Length} frames");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SlimeAnimationLoader] FAILED {state}: {ex.Message}");
                }
            }

            if (!_frameCache.ContainsKey(SlimeAnimationState.Attack))
            {
                foreach (var alt in new[] { "Attack_2.png", "Attack_3.png" })
                {
                    var frames = SliceSpriteSheet(Path.Combine(SlimeRoot, alt));
                    if (frames.Length > 0)
                    {
                        _frameCache[SlimeAnimationState.Attack] = frames;
                        break;
                    }
                }
            }

            _loaded = true;
        }

        private static Image[] SliceSpriteSheet(string path)
        {
            if (!File.Exists(path)) return Array.Empty<Image>();

            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using var sheet = Image.FromStream(stream);
                int cols = sheet.Width / FrameSize;
                if (cols <= 0 || sheet.Height < FrameSize) return Array.Empty<Image>();

                var frames = new List<Image>(cols);
                for (int i = 0; i < cols; i++)
                {
                    var frame = new Bitmap(FrameSize, FrameSize);
                    using var g = Graphics.FromImage(frame);
                    g.DrawImage(
                        sheet,
                        new Rectangle(0, 0, FrameSize, FrameSize),
                        new Rectangle(i * FrameSize, 0, FrameSize, FrameSize),
                        GraphicsUnit.Pixel);
                    frames.Add(frame);
                }
                return frames.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SlimeAnimationLoader] Exception: {ex.Message}");
                return Array.Empty<Image>();
            }
        }

        public static void DisposeAll()
        {
            foreach (var frames in _frameCache.Values)
                foreach (var img in frames)
                    img.Dispose();
            _frameCache.Clear();
            _loaded = false;
        }
    }
}