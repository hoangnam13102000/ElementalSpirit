using System;
using ElementalSpirit.Domain.Stage;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface IWaveManager
    {
        int CurrentWaveNumber { get; }
        int TotalWaves { get; }
        WaveState State { get; }
        string StageName { get; }
        string StageBackgroundImageName { get; }
        event Action? OnStageCompleted;
        event Action<int>? OnWaveStarted;
        void LoadStage(StageData stage);
        void Update(float deltaTime);
        void ForceNextWave();
        void RestoreState(WaveState state);
    }
}
