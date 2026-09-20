using System;
using System.Drawing;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Presentation.Assets
{
    public sealed class PortalAnimationController : IDisposable
    {
        private readonly AnimationClip _clip;

        public Image CurrentImage => _clip.CurrentImage;

        public PortalAnimationController(Image[] frames, float fps = 8f, bool isLooping = true)
        {
            if (frames == null || frames.Length == 0)
                throw new ArgumentException("Portal animation requires at least one frame.", nameof(frames));

            _clip = new AnimationClip(frames, fps, isLooping);
        }

        public void Update(float deltaTime)
        {
            _clip.Update(deltaTime);
        }

        public void Restart()
        {
            _clip.Reset();
        }

        public void Dispose()
        {
            _clip.Dispose();
        }
    }
}
