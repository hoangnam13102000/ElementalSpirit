using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Presentation.Assets
{
    public static class MageAnimationLoader
    {
        private static readonly Dictionary<PlayerAnimationState, AnimationClip> _cache = new();
        private static bool _loaded;

        private static string MageRoot =>
            Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "Characters", "Mage");

        private static readonly (string folder, float fps, bool looping)[] _stateDefs = new[]
        {
            (folder: "Idle",        fps: 10f, looping: true),
            (folder: "Walk",        fps: 10f, looping: true),
            (folder: "Run",         fps: 14f, looping: true),
            (folder: "Jump",        fps: 12f, looping: false),
            (folder: "High_Jump",   fps: 12f, looping: false),
            (folder: "Attack",      fps: 14f, looping: false),
            (folder: "Walk_Attack", fps: 12f, looping: false),
            (folder: "Run_Attack",  fps: 14f, looping: false),
            (folder: "Fire_Extra",  fps: 12f, looping: false),
            (folder: "Fire_Extra",  fps: 12f, looping: false),
            (folder: "Hurt",        fps: 10f, looping: false),
            (folder: "Death",       fps: 10f, looping: false),
            (folder: "Push",        fps: 10f, looping: true),
            (folder: "Climb",       fps: 10f, looping: true),
        };

        private static readonly PlayerAnimationState[] _statesInOrder = new[]
        {
            PlayerAnimationState.Idle,
            PlayerAnimationState.Walk,
            PlayerAnimationState.Run,
            PlayerAnimationState.Jump,
            PlayerAnimationState.HighJump,
            PlayerAnimationState.Attack,
            PlayerAnimationState.WalkAttack,
            PlayerAnimationState.RunAttack,
            PlayerAnimationState.Fire,
            PlayerAnimationState.FireExtra,
            PlayerAnimationState.Hurt,
            PlayerAnimationState.Death,
            PlayerAnimationState.Push,
            PlayerAnimationState.Climb,
        };

        public static PlayerAnimationController CreateController()
        {
            EnsureLoaded();
            var dict = new Dictionary<PlayerAnimationState, AnimationClip>();
            foreach (var state in _statesInOrder)
            {
                if (_cache.TryGetValue(state, out var clip))
                    dict[state] = clip;
            }
            return new PlayerAnimationController(dict, PlayerAnimationState.Idle);
        }

        public static Image[] LoadFireballFrames()
        {
            return LoadFramesFromFolder(Path.Combine(MageRoot, "Fire"));
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            for (int i = 0; i < _statesInOrder.Length; i++)
            {
                var state = _statesInOrder[i];
                var def = _stateDefs[i];
                try
                {
                    string folderPath = Path.Combine(MageRoot, def.folder);
                    var frames = LoadFramesFromFolder(folderPath);
                    if (frames.Length == 0)
                    {
                        Debug.WriteLine($"[MageAnimationLoader] WARNING: No frames for {state}");
                        continue;
                    }
                    _cache[state] = new AnimationClip(frames, def.fps, def.looping);
                    Debug.WriteLine($"[MageAnimationLoader] Loaded {state}: {frames.Length} frames");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[MageAnimationLoader] FAILED {state}: {ex.Message}");
                }
            }
            _loaded = true;
        }

        private static Image[] LoadFramesFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return Array.Empty<Image>();
            var files = Directory.GetFiles(folderPath, "*.png");
            var numbered = new List<(int number, string path)>();
            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
                Match m = Regex.Match(name, @"(\d+)$");
                if (!m.Success) continue;
                if (int.TryParse(m.Groups[1].Value, out int num))
                    numbered.Add((num, file));
            }
            numbered.Sort((a, b) => a.number.CompareTo(b.number));
            var images = new List<Image>();
            foreach (var (_, path) in numbered)
            {
                try
                {
                    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    using var raw = Image.FromStream(stream);
                    images.Add(new Bitmap(raw));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[MageAnimationLoader] Failed frame '{path}': {ex.Message}");
                }
            }
            return images.ToArray();
        }

        public static void DisposeAll()
        {
            foreach (var clip in _cache.Values) clip.Dispose();
            _cache.Clear();
            _loaded = false;
        }
    }
}