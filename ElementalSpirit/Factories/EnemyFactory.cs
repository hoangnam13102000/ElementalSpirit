using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Enemy.NormalEnemy;

namespace ElementalSpirit.Factories
{
    public enum EnemyType
    {
        Slime
        
    }

    public static class EnemyFactory
    {
        public static Enemy Create(EnemyType type, float x, float y)
        {
            return type switch
            {
                EnemyType.Slime => new Slime(x, y),
                _ => throw new ArgumentException($"Unknown enemy type: {type}")
            };
        }
    }
}