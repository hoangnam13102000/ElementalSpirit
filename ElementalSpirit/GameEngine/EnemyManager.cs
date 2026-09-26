using System.Collections.Generic;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Enemy.AI;
using ElementalSpirit.Domain.Stage;
using System.Drawing;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.GameEngine
{
    public class EnemyManager : IEnemyManager
    {
        private readonly List<Enemy> _enemies = new();
        private readonly TerrainCollisionResolver _terrainResolver = new();
        private readonly TerrainWallResolver _wallResolver = new();
        private IReadOnlyList<TerrainPlatform> _platforms = Array.Empty<TerrainPlatform>();
        private IReadOnlyList<TerrainWall> _walls = Array.Empty<TerrainWall>();
        private RectangleF _playArea;
        public IReadOnlyList<Enemy> Enemies => _enemies;

        public void Add(Enemy enemy) => _enemies.Add(enemy);

        public void SetTerrain(IReadOnlyList<TerrainPlatform> platforms, RectangleF playArea)
        {
            _platforms = platforms ?? throw new ArgumentNullException(nameof(platforms));
            _playArea = playArea;
        }

        public void SetWalls(IReadOnlyList<TerrainWall> walls, RectangleF playArea)
        {
            _walls = walls ?? throw new ArgumentNullException(nameof(walls));
            _playArea = playArea;
        }

        public void Update(
            float deltaTime,
            float groundY = 0f,
            float minX = 0f,
            float maxX = float.MaxValue,
            IEnemyTarget? target = null)
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var e = _enemies[i];
                float previousX = e.X;
                float previousFootY = e.Y + e.Height;
                e.Update(deltaTime, groundY, target);
                float resolvedX = _wallResolver.ResolveHorizontalPosition(
                    e.Bounds,
                    previousX,
                    _walls,
                    _playArea);
                e.RestoreHorizontalPosition(resolvedX);
                ResolveTerrainMovement(e, previousX, previousFootY, groundY);
                e.ClampHorizontalBounds(minX, maxX);

                if (!e.IsAlive && e.IsDeathAnimationComplete)
                {
                    e.OnDeath();
                    _enemies.RemoveAt(i);
                }
            }
        }

        public void Clear() => _enemies.Clear(); 

        private void ResolveTerrainMovement(
            Enemy enemy,
            float previousX,
            float previousFootY,
            float fallbackGroundY)
        {
            if (_platforms.Count == 0)
            {
                enemy.ResolveGroundCollision(fallbackGroundY);
                return;
            }

            float footX = enemy.X + enemy.Width / 2f;
            float currentFootY = enemy.Y + enemy.Height;
            if (_terrainResolver.TryGetSupportingGroundY(
                    _platforms, _playArea, footX, previousFootY, currentFootY, out float supportY))
            {
                enemy.ResolveGroundCollision(supportY);
                return;
            }

            // The final boss starts on an elevated platform. Let it step off
            // that platform and land on the main ground instead of pinning it
            // to the previous X position at the platform edge.
            if (enemy is GorgonBoss)
            {
                enemy.ResolveGroundCollision(fallbackGroundY);
                return;
            }

            enemy.RestoreHorizontalPosition(previousX);
            footX = enemy.X + enemy.Width / 2f;
            currentFootY = enemy.Y + enemy.Height;
            if (_terrainResolver.TryGetSupportingGroundY(
                    _platforms, _playArea, footX, previousFootY, currentFootY, out supportY))
            {
                enemy.ResolveGroundCollision(supportY);
            }
            else
            {
                enemy.ResolveGroundCollision(fallbackGroundY);
            }
        }
    }
}