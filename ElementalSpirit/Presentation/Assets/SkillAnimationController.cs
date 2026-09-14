using System;
using System.Drawing;

namespace ElementalSpirit.Presentation.Assets
{
    public sealed class SkillAnimationController : IDisposable
    {
        private readonly Image[] _frames;
        private readonly float _fps;
        private float _timer;

        public int CurrentFrameIndex { get; private set; }
        public bool IsCompleted { get; private set; }
        public Image? CurrentImage =>
            _frames.Length == 0 ? null : _frames[CurrentFrameIndex];

        public SkillAnimationController(Image[] frames, float fps)
        {
            _frames = frames ?? throw new ArgumentNullException(nameof(frames));
            _fps = fps > 0f ? fps : throw new ArgumentOutOfRangeException(nameof(fps));
        }

        public void Restart()
        {
            CurrentFrameIndex = 0;
            _timer = 0f;
            IsCompleted = _frames.Length == 0;
        }

        public void Update(float deltaTime)
        {
            if (IsCompleted || _frames.Length == 0) return;

            _timer += Math.Max(0f, deltaTime);
            float frameDuration = 1f / _fps;
            while (_timer >= frameDuration && !IsCompleted)
            {
                _timer -= frameDuration;
                if (CurrentFrameIndex >= _frames.Length - 1)
                    IsCompleted = true;
                else
                    CurrentFrameIndex++;
            }
        }

        public void Dispose()
        {
            foreach (var frame in _frames)
                frame.Dispose();
        }
    }
}
