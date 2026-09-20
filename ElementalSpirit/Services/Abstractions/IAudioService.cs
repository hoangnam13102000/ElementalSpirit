using System;

namespace ElementalSpirit.Services.Abstractions
{
    public interface IAudioService : IDisposable
    {
        void PlayStageTheme(int stageNumber);
        void Stop();
    }
}
