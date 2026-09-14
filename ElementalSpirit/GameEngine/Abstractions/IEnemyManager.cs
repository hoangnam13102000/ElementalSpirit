using System.Collections.Generic;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Enemy.AI;
using ElementalSpirit.Domain.Stage;
using System.Drawing;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface IEnemyManager
    {
        IReadOnlyList<Enemy> Enemies { get; }
        void Add(Enemy enemy);
        void SetTerrain(IReadOnlyList<TerrainPlatform> platforms, RectangleF playArea);
        void SetWalls(IReadOnlyList<TerrainWall> walls, RectangleF playArea);
        void Update(
            float deltaTime,
            float groundY = 0f,
            float minX = 0f,
            float maxX = float.MaxValue,
            IEnemyTarget? target = null);
        void Clear();
    }
}