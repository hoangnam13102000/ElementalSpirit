using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Domain.Enemy.AI;
using ElementalSpirit.Factories;
using System.Collections.Generic;
using System.Drawing;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface ISpawnManager
    {
        void SetSpawnArea(float screenWidth, float screenHeight);
        void SetTerrain(IReadOnlyList<TerrainPlatform> platforms, RectangleF playArea);
        void SetSpawnSafetyTarget(IEnemyTarget target, float safeRadius);
        void Spawn(SpawnData data);
        Enemy SpawnSingle(EnemyType type, float x, float y);
    }
}
