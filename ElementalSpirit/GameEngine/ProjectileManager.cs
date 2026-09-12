using System.Collections.Generic;
using ElementalSpirit.Domain.Projectile;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.GameEngine
{
    public class ProjectileManager : IProjectileManager
    {
        private readonly List<Projectile> _projectiles = new();

        public IReadOnlyList<Projectile> Projectiles => _projectiles;

        public void Add(Projectile projectile)
        {
            _projectiles.Add(projectile);
        }

        public void Update(float deltaTime)
        {
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                var p = _projectiles[i];
                p.Update(deltaTime);

                if (!p.IsAlive)
                    _projectiles.RemoveAt(i);
            }
        }

        public void Clear()
        {
            _projectiles.Clear();
        }
    }
}