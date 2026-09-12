using System.Collections.Generic;
using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface IProjectileManager
    {
        IReadOnlyList<Projectile> Projectiles { get; }
        void Add(Projectile projectile);
        void Update(float deltaTime);
        void Clear();
    }
}
