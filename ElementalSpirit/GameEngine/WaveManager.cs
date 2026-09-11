using System;
using ElementalSpirit.Domain.Stage;

namespace ElementalSpirit.GameEngine
{
    public enum WaveState
    {
        WaitingToStart,
        Spawning,
        WaitingForClear,
        StageCompleted
    }

    public class WaveManager
    {
        private readonly SpawnManager _spawnManager;
        private readonly EnemyManager _enemyManager;

        private StageData? _currentStage;
        private int _currentWaveIndex = -1;
        private WaveState _state = WaveState.WaitingToStart;

        private float _timer;
        private int _spawnIndex;
        private float _spawnTimer;

        public int CurrentWaveNumber => _currentWaveIndex + 1;
        public int TotalWaves => _currentStage?.Waves.Count ?? 0;
        public WaveState State => _state;
        public string StageName => _currentStage?.Name ?? "Unknown";
        public string StageBackgroundImageName => _currentStage?.BackgroundImageName ?? "";

        public event Action? OnStageCompleted;
        public event Action<int>? OnWaveStarted;

        public WaveManager(SpawnManager spawnManager, EnemyManager enemyManager)
        {
            _spawnManager = spawnManager;
            _enemyManager = enemyManager;
        }

        public void LoadStage(StageData stage)
        {
            _currentStage = stage;
            _currentWaveIndex = -1;
            _state = WaveState.WaitingToStart;
            _timer = 1.0f; // nghỉ 1 giây trước wave đầu
        }

        public void Update(float deltaTime)
        {
            if (_currentStage == null) return;

            switch (_state)
            {
                case WaveState.WaitingToStart:
                    _timer -= deltaTime;
                    if (_timer <= 0)
                        StartNextWave();
                    break;

                case WaveState.Spawning:
                    UpdateSpawning(deltaTime);
                    break;

                case WaveState.WaitingForClear:
                    if (_enemyManager.Enemies.Count == 0)
                    {
                        // Hết quái → sang wave tiếp hoặc hoàn thành
                        if (_currentWaveIndex >= _currentStage.Waves.Count - 1)
                        {
                            _state = WaveState.StageCompleted;
                            OnStageCompleted?.Invoke();
                        }
                        else
                        {
                            _state = WaveState.WaitingToStart;
                            _timer = 2.0f; // nghỉ giữa các wave
                        }
                    }
                    break;

                case WaveState.StageCompleted:
                    // Giữ nguyên, chờ người chơi quyết định
                    break;
            }
        }

        private void StartNextWave()
        {
            _currentWaveIndex++;
            if (_currentWaveIndex >= _currentStage!.Waves.Count)
            {
                _state = WaveState.StageCompleted;
                OnStageCompleted?.Invoke();
                return;
            }

            var wave = _currentStage.Waves[_currentWaveIndex];
            _spawnIndex = 0;
            _spawnTimer = 0f;
            _state = WaveState.Spawning;

            OnWaveStarted?.Invoke(CurrentWaveNumber);
        }

        private void UpdateSpawning(float deltaTime)
        {
            var wave = _currentStage!.Waves[_currentWaveIndex];

            if (_spawnIndex >= wave.Spawns.Count)
            {
                // Đã spawn hết nhóm trong wave này
                _state = WaveState.WaitingForClear;
                return;
            }

            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0)
            {
                var spawnData = wave.Spawns[_spawnIndex];
                _spawnManager.Spawn(spawnData);

                _spawnIndex++;
                _spawnTimer = spawnData.SpawnInterval;
            }
        }

        public void ForceNextWave()
        {
            // Dùng để test nhanh
            _enemyManager.Clear();
            _state = WaveState.WaitingToStart;
            _timer = 0.3f;
        }
    }
}