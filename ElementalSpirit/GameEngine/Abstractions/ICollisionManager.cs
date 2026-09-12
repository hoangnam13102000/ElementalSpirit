namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface ICollisionManager
    {
        void CheckCollisions(IProjectileManager projectileManager, IEnemyManager enemyManager);
    }
}
