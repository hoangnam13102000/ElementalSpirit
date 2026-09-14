using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Presentation.Assets
{
    public enum GorgonAnimationState
    {
        Idle,
        Walk,
        Run,
        Attack,
        Special,
        Hurt,
        Dead
    }

    public sealed class GorgonBossAnimationController : IDisposable
    {
        private readonly Dictionary<GorgonAnimationState, AnimationClip> _clips;
        private GorgonAnimationState _currentState;
        private AnimationClip _currentClip;

        public GorgonAnimationState CurrentState => _currentState;
        public Image CurrentImage => _currentClip.CurrentImage;
        public int CurrentFrameIndex => _currentClip.CurrentFrame;
        public bool IsCurrentCompleted => _currentClip.IsCompleted;

        public GorgonBossAnimationController(
            Dictionary<GorgonAnimationState, AnimationClip> clips,
            GorgonAnimationState initialState = GorgonAnimationState.Idle)
        {
            _clips = clips ?? throw new ArgumentNullException(nameof(clips));
            if (!_clips.ContainsKey(initialState))
                throw new ArgumentException($"Initial state '{initialState}' not found.", nameof(initialState));
            _currentState = initialState;
            _currentClip = _clips[initialState];
        }

        public void Play(GorgonAnimationState state, bool forceReset = false)
        {
            if (state == _currentState && !forceReset) return;
            if (!_clips.ContainsKey(state)) return;
            _currentState = state;
            _currentClip = _clips[state];
            _currentClip.Reset();
        }

        public void Update(float deltaTime) => _currentClip.Update(deltaTime);

        public void Dispose() => _clips.Clear();
    }

    public static class GorgonAnimationLoader
    {
        public const int FrameSize = 128;

        private static readonly Dictionary<GorgonAnimationState, List<Image>> _cache = new();
        private static bool _loaded;

        private static readonly (GorgonAnimationState state, string fileName)[] _files =
        {
            (GorgonAnimationState.Idle, "Idle.png"),
            (GorgonAnimationState.Idle, "Idle_2.png"),
            (GorgonAnimationState.Walk, "Walk.png"),
            (GorgonAnimationState.Run, "Run.png"),
            (GorgonAnimationState.Attack, "Attack_1.png"),
            (GorgonAnimationState.Attack, "Attack_2.png"),
            (GorgonAnimationState.Attack, "Attack_3.png"),
            (GorgonAnimationState.Special, "Special.png"),
            (GorgonAnimationState.Hurt, "Hurt.png"),
            (GorgonAnimationState.Dead, "Dead.png")
        };

        private static string BossRoot =>
            Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "Enemies", "Boss", "Gorgon_1");

        public static GorgonBossAnimationController CreateController()
        {
            EnsureLoaded();
            var dict = new Dictionary<GorgonAnimationState, AnimationClip>();
            foreach (var state in Enum.GetValues<GorgonAnimationState>())
            {
                if (_cache.TryGetValue(state, out var frames) && frames.Count > 0)
                {
                    var looping = state == GorgonAnimationState.Idle || state == GorgonAnimationState.Walk || state == GorgonAnimationState.Run;
                    dict[state] = new AnimationClip(frames.ToArray(), state == GorgonAnimationState.Run ? 12f : 10f, looping);
                }
            }
            if (dict.Count == 0)
                throw new InvalidOperationException("No Gorgon animation clips loaded.");
            return new GorgonBossAnimationController(dict, GorgonAnimationState.Idle);
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;

            foreach (var (state, fileName) in _files)
            {
                string path = Path.Combine(BossRoot, fileName);
                if (!File.Exists(path))
                {
                    Debug.WriteLine($"[GorgonAnimationLoader] missing: {path}");
                    continue;
                }

                try
                {
                    var frames = LoadFramesFromFile(path, state);
                    if (frames.Count == 0) continue;
                    if (!_cache.TryGetValue(state, out var existing))
                        _cache[state] = new List<Image>();
                    _cache[state].AddRange(frames);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[GorgonAnimationLoader] failed {state}: {ex.Message}");
                }
            }

            _loaded = true;
        }

        private static List<Image> LoadFramesFromFile(string path, GorgonAnimationState state)
        {
            var images = new List<Image>();
            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using var raw = Image.FromStream(stream);

                if (raw.Width <= FrameSize && raw.Height <= FrameSize)
                {
                    images.Add(new Bitmap(raw));
                    return images;
                }

                int columns = raw.Width / FrameSize;
                if (columns <= 0 || raw.Height < FrameSize)
                    return images;

                for (int i = 0; i < columns; i++)
                {
                    var frame = new Bitmap(FrameSize, FrameSize);
                    using var g = Graphics.FromImage(frame);
                    g.DrawImage(
                        raw,
                        new Rectangle(0, 0, FrameSize, FrameSize),
                        new Rectangle(i * FrameSize, 0, FrameSize, FrameSize),
                        GraphicsUnit.Pixel);
                    images.Add(frame);
                }

                return images;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GorgonAnimationLoader] Exception for {state}: {ex.Message}");
                return images;
            }
        }

        public static void DisposeAll()
        {
            foreach (var frames in _cache.Values)
                foreach (var img in frames)
                    img.Dispose();
            _cache.Clear();
            _loaded = false;
        }
    }
}
