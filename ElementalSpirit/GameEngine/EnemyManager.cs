using System.Collections.Generic;
using ElementalSpirit.Domain.Enemy;

namespace ElementalSpirit.GameEngine
{
    public class EnemyManager
    {
        private readonly List<Enemy> _enemies = new();
        public IReadOnlyList<Enemy> Enemies => _enemies;

        public void Add(Enemy enemy) => _enemies.Add(enemy);

        public void Update(float deltaTime, float groundY = 0f)
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var e = _enemies[i];
                e.Update(deltaTime, groundY);
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