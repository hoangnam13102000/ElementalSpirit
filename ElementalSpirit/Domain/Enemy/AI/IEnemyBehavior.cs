namespace ElementalSpirit.Domain.Enemy.AI
{
    public interface IEnemyBehavior
    {
        EnemyBehaviorDecision Update(
            float deltaTime,
            float enemyCenterX,
            float enemyCenterY,
            IEnemyTarget? target);
    }
}
