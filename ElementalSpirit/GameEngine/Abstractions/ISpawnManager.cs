using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface ISpawnManager
    {
        void SetSpawnArea(float screenWidth, float screenHeight);
        void Spawn(SpawnData data);
        Enemy SpawnSingle(EnemyType type, float x, float y);
    }
}
