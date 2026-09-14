namespace ElementalSpirit.Domain.Enemy.AI
{
    public interface IEnemyTarget
    {
        float X { get; }
        float Y { get; }
        int Width { get; }
        int Height { get; }
        bool IsAlive { get; }
    }
}
