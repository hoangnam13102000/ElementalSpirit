using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ElementalSpirit.Domain.Player;

namespace ElementalSpirit.Presentation.Assets
{
    public static class GoldAnimationLoader
    {
        private const int FrameCount = 30;
        private const float FramesPerSecond = 15f;

        public static AnimationClip? CreateClip()
        {
            var frames = new List<Image>(FrameCount);
            for (int frameNumber = 1; frameNumber <= FrameCount; frameNumber++)
            {
                var image = AssetLoader.Get(Path.Combine(
                    "Loot", "coin", "Gold", $"Gold_{frameNumber}.png"));
                if (image != null)
                    frames.Add(image);
            }

            return frames.Count > 0
                ? new AnimationClip(frames.ToArray(), FramesPerSecond, isLooping: true)
                : null;
        }
    }
}
