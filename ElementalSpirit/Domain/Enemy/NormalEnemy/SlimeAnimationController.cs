using System;
using System.Collections.Generic;
using System.Drawing;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Domain.Enemy.NormalEnemy
{

    public class SlimeAnimationController : IDisposable
    {
        private readonly Dictionary<SlimeAnimationState, AnimationClip> _clips;
        private SlimeAnimationState _currentState;
        private AnimationClip _currentClip;

        public SlimeAnimationState CurrentState => _currentState;
        public AnimationClip CurrentClip => _currentClip;
        public Image CurrentImage => _currentClip.CurrentImage;
        public int CurrentFrameIndex => _currentClip.CurrentFrame;
        public bool IsCurrentCompleted => _currentClip.IsCompleted;

        public SlimeAnimationController(
            Dictionary<SlimeAnimationState, AnimationClip> clips,
            SlimeAnimationState initialState = SlimeAnimationState.Idle)
        {
            _clips = clips ?? throw new ArgumentNullException(nameof(clips));
            if (!_clips.ContainsKey(initialState))
                throw new ArgumentException($"Initial state '{initialState}' not found.", nameof(initialState));
            _currentState = initialState;
            _currentClip = _clips[initialState];
        }

        public void Play(SlimeAnimationState state)
        {
            if (state == _currentState) return;
            if (!_clips.ContainsKey(state)) return;
            _currentState = state;
            _currentClip = _clips[state];
            _currentClip.Reset();
        }

        public void Update(float deltaTime) => _currentClip.Update(deltaTime);

        public void Dispose() => _clips.Clear();
    }
}