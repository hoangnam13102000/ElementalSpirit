using System;
using System.Linq;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Enemy.AI;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;
using System.Collections.Generic;
using System.Drawing;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.GameEngine
{
    public class SpawnManager : ISpawnManager
    {
        private readonly IEnemyManager _enemyManager;
        private readonly Random _random = new();

        private float _spawnMinY = 120f;
        private float _spawnMaxY = 580f;
        private float _spawnX = 1280f;
        private IReadOnlyList<TerrainPlatform> _platforms = Array.Empty<TerrainPlatform>();
        private RectangleF _playArea;
        private IEnemyTarget? _safetyTarget;
        private float _safeRadius;

        public SpawnManager(IEnemyManager enemyManager)
        {
            _enemyManager = enemyManager ?? throw new ArgumentNullException(nameof(enemyManager));
        }

        public void SetSpawnArea(float screenWidth, float screenHeight)
        {
            _spawnX = screenWidth - 30f;
            // ===== SUA: Chi spawn enemy trong dai "nen dat" (duong di), khop voi GameManager.GroundTop/GroundBottom =====
            _spawnMinY = screenHeight * 0.64f;
            _spawnMaxY = screenHeight * 0.78f;
            // ============================================================================================================
        }

        public void SetTerrain(IReadOnlyList<TerrainPlatform> platforms, RectangleF playArea)
        {
            _platforms = platforms ?? throw new ArgumentNullException(nameof(platforms));
            _playArea = playArea;
        }

        public void SetSpawnSafetyTarget(IEnemyTarget target, float safeRadius)
        {
            _safetyTarget = target ?? throw new ArgumentNullException(nameof(target));
            _safeRadius = Math.Max(0f, safeRadius);
        }

        public void Spawn(SpawnData data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var position = ChooseSpawnPosition();

                Enemy enemy = EnemyFactory.Create(data.EnemyType, position.X, position.Y);
                _enemyManager.Add(enemy);
            }
        }

        private (float X, float Y) ChooseSpawnPosition()
        {
            (float X, float Y) fallback = (0f, 0f);
            float farthestDistance = float.MinValue;

            for (int attempt = 0; attempt < 24; attempt++)
            {
                var platform = ChooseSpawnPlatform();
                float minX = _playArea.Left + _playArea.Width * platform.MinXRatio;
                float maxX = _playArea.Left + _playArea.Width * platform.MaxXRatio;
                float x = minX + (float)_random.NextDouble() * Math.Max(1f, maxX - minX - 40f);
                float y = platform.GetAbsoluteTopY(_playArea) - 32f;
                float distance = DistanceToSafetyTarget(x + 20f, y + 16f);

                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    fallback = (x, y);
                }

                if (distance >= _safeRadius)
                    return (x, y);
            }

            return fallback;
        }

        private float DistanceToSafetyTarget(float x, float y)
        {
            if (_safetyTarget == null) return float.MaxValue;
            float targetX = _safetyTarget.X + _safetyTarget.Width / 2f;
            float targetY = _safetyTarget.Y + _safetyTarget.Height / 2f;
            return MathF.Sqrt(MathF.Pow(x - targetX, 2f) + MathF.Pow(y - targetY, 2f));
        }

        private TerrainPlatform ChooseSpawnPlatform()
        {
            if (_platforms.Count == 0)
            {
                return new TerrainPlatform(
                    "FallbackGround", 0f, 1f, _spawnMaxY / Math.Max(1f, _playArea.Height), 1f);
            }

            var spawnablePlatforms = _platforms
                .Where(platform => !IsPitPlatform(platform))
                .ToArray();

            if (spawnablePlatforms.Length == 0)
                return _platforms[0];

            return spawnablePlatforms[_random.Next(spawnablePlatforms.Length)];
        }

        private static bool IsPitPlatform(TerrainPlatform platform)
        {
            return platform.Name.Contains("Pit", StringComparison.OrdinalIgnoreCase);
        }

        public Enemy SpawnSingle(EnemyType type, float x, float y)
        {
            var enemy = EnemyFactory.Create(type, x, y);
            _enemyManager.Add(enemy);
            return enemy;
        }
    }
}