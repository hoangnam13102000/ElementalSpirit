using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.Domain.Skill
{
    public interface IProjectileLoadout
    {
        ProjectileType CurrentProjectileType { get; }
        void SetProjectileType(ProjectileType projectileType);
    }
}
