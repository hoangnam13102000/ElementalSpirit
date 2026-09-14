namespace ElementalSpirit.Domain.Enemy.AI
{
    public readonly struct EnemyBehaviorDecision
    {
        public EnemyBehaviorState State { get; }
        public float DirectionX { get; }

        public EnemyBehaviorDecision(EnemyBehaviorState state, float directionX = 0f)
        {
            State = state;
            DirectionX = directionX;
        }
    }
}
