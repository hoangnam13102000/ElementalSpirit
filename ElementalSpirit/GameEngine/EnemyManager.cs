using System.Collections.Generic;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.GameEngine
{
    public class EnemyManager : IEnemyManager
    {
        private readonly List<Enemy> _enemies = new();
        public IReadOnlyList<Enemy> Enemies => _enemies;

        public void Add(Enemy enemy) => _enemies.Add(enemy);

        public void Update(float deltaTime, float groundY = 0f, float minX = 0f, float maxX = float.MaxValue)
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var e = _enemies[i];
                e.Update(deltaTime, groundY);
                e.ResolveGroundCollision(groundY);
                e.ClampHorizontalBounds(minX, maxX);

                if (!e.IsAlive && e.IsDeathAnimationComplete)
                {
                    e.OnDeath();
                    _enemies.RemoveAt(i);
                }
            }
        }

        public void Clear() => _enemies.Clear();
    }
}