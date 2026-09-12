using System;
using System.Drawing;

namespace ElementalSpirit.Domain.Player
{
    public class AnimationClip : IDisposable
    {
        private readonly Image[] _frames;
        private float _frameTimer;
        private bool _completedRaised;

        public Image[] Frames => _frames;
        public int FrameCount => _frames.Length;
        public int CurrentFrame { get; private set; }
        public float Fps { get; set; }
        public bool IsLooping { get; set; }
        public bool IsCompleted { get; private set; }
        public event Action? Completed;

        public AnimationClip(Image[] frames, float fps, bool isLooping)
        {
            _frames = frames ?? throw new ArgumentNullException(nameof(frames));
            if (_frames.Length == 0)
                throw new ArgumentException("Animation clip must have at least one frame.", nameof(frames));
            Fps = fps;
            IsLooping = isLooping;
            CurrentFrame = 0;
            _frameTimer = 0f;
            IsCompleted = false;
            _completedRaised = false;
        }

        public Image CurrentImage => _frames[CurrentFrame];

        public void Update(float deltaTime)
        {
            if (FrameCount <= 1)
            {
                if (!IsLooping && !_completedRaised)
                {
                    IsCompleted = true;
                    _completedRaised = true;
                    Completed?.Invoke();
                }
                return;
            }
            if (!IsLooping && IsCompleted) return;
            float frameDuration = 1f / Fps;
            _frameTimer += deltaTime;
            while (_frameTimer >= frameDuration)
            {
                _frameTimer -= frameDuration;
                AdvanceFrame();
                if (!IsLooping && IsCompleted) break;
            }
        }

        private void AdvanceFrame()
        {
            if (IsLooping)
            {
                CurrentFrame = (CurrentFrame + 1) % FrameCount;
            }
            else
            {
                if (CurrentFrame < FrameCount - 1)
                {
                    CurrentFrame++;
                }
                else
                {
                    IsCompleted = true;
                    if (!_completedRaised)
                    {
                        _completedRaised = true;
                        Completed?.Invoke();
                    }
                }
            }
        }

        public void Reset()
        {
            CurrentFrame = 0;
            _frameTimer = 0f;
            IsCompleted = false;
            _completedRaised = false;
        }

        public void Dispose()
        {
            foreach (var img in _frames)
                img.Dispose();
        }
    }
}