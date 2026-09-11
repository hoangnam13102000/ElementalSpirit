using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.GameEngine
{
    public class CollisionManager
    {
        public void CheckCollisions(ProjectileManager projectileManager, EnemyManager enemyManager)
        {
            var projectiles = projectileManager.Projectiles;
            var enemies = enemyManager.Enemies;

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var p = projectiles[i];
                if (!p.IsAlive) continue;

                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var e = enemies[j];
                    if (!e.IsAlive) continue;

                    if (p.Bounds.IntersectsWith(e.Bounds))
                    {
                        e.TakeDamage(p.Damage);
                        p.Kill(); // đạn biến mất sau khi trúng
                        break;
                    }
                }
            }
        }
    }
}