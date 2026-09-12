using System;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;

namespace ElementalSpirit.GameEngine
{
    public class SpawnManager
    {
        private readonly EnemyManager _enemyManager;
        private readonly Random _random = new();

        private float _spawnMinY = 120f;
        private float _spawnMaxY = 580f;
        private float _spawnX = 1280f;

        public SpawnManager(EnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
        }

        public void SetSpawnArea(float screenWidth, float screenHeight)
        {
            _spawnX = screenWidth - 30f;
            // ===== SUA: Chi spawn enemy trong dai "nen dat" (duong di), khop voi GameManager.GroundTop/GroundBottom =====
            _spawnMinY = screenHeight * 0.64f;
            _spawnMaxY = screenHeight * 0.78f;
            // ============================================================================================================
        }

        public void Spawn(SpawnData data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                float y = _spawnMinY + (float)_random.NextDouble() * (_spawnMaxY - _spawnMinY);

                float x = _spawnX + i * 45f;

                Enemy enemy = EnemyFactory.Create(data.EnemyType, x, y);
                _enemyManager.Add(enemy);
            }
        }

        public Enemy SpawnSingle(EnemyType type, float x, float y)
        {
            var enemy = EnemyFactory.Create(type, x, y);
            _enemyManager.Add(enemy);
            return enemy;
        }
    }
}