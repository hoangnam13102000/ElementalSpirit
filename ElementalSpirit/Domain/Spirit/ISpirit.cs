namespace ElementalSpirit.Domain.Spirit
{
    using PlayerEntity = ElementalSpirit.Domain.Player.Player;

    public interface ISpirit
    {
        string Id { get; }
        string Name { get; }
        string Element { get; }
        int Level { get; }
        float CooldownRemaining { get; }
        float CooldownDuration { get; }
        float ActiveRemaining { get; }
        bool IsActive { get; }
        bool CanActivate { get; }
        void Activate(PlayerEntity player);
        void Update(float deltaTime, PlayerEntity player);
        void Upgrade();
    }
}