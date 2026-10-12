using System.Drawing;

namespace ElementalSpirit.Domain.Enemy
{
    public interface IGorgonBossAnimationController
    {
        GorgonAnimationState CurrentState { get; }
        Image CurrentImage { get; }
        int CurrentFrameIndex { get; }
        bool IsCurrentCompleted { get; }

        void Play(GorgonAnimationState state, bool forceReset = false);
        void Update(float deltaTime);
    }
}
