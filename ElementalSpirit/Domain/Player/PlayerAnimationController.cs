using System;
using System.Collections.Generic;
using System.Drawing;

namespace ElementalSpirit.Domain.Player
{
    public class PlayerAnimationController : IDisposable
    {
        private readonly Dictionary<PlayerAnimationState, AnimationClip> _clips;
        private PlayerAnimationState _currentState;
        private AnimationClip _currentClip;

        public PlayerAnimationState CurrentState => _currentState;
        public AnimationClip CurrentClip => _currentClip;
        public Image CurrentImage => _currentClip.CurrentImage;
        public int CurrentFrameIndex => _currentClip.CurrentFrame;
        public bool IsCurrentCompleted => _currentClip.IsCompleted;

        public PlayerAnimationController(Dictionary<PlayerAnimationState, AnimationClip> clips,
                                         PlayerAnimationState initialState = PlayerAnimationState.Idle)
        {
            _clips = clips ?? throw new ArgumentNullException(nameof(clips));
            if (!_clips.ContainsKey(initialState))
                throw new ArgumentException($"Initial state '{initialState}' not found in clips.", nameof(initialState));
            _currentState = initialState;
            _currentClip = _clips[initialState];
        }

        public void Play(PlayerAnimationState state)
        {
            if (state == _currentState) return;
            if (!_clips.ContainsKey(state)) return;
            _currentState = state;
            _currentClip = _clips[state];
            _currentClip.Reset();
        }

        public void RestartCurrent()
        {
            _currentClip.Reset();
        }

        public void Update(float deltaTime)
        {
            _currentClip.Update(deltaTime);
        }

        public AnimationClip? GetClip(PlayerAnimationState state)
        {
            return _clips.TryGetValue(state, out var clip) ? clip : null;
        }

        public void Dispose()
        {
            _clips.Clear();
        }
    }
}