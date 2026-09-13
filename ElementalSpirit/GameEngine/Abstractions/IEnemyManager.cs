using System.Collections.Generic;
using ElementalSpirit.Domain.Enemy;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface IEnemyManager
    {
        IReadOnlyList<Enemy> Enemies { get; }
        void Add(Enemy enemy);
        void Update(float deltaTime, float groundY = 0f, float minX = 0f, float maxX = float.MaxValue);
        void Clear();
    }
}