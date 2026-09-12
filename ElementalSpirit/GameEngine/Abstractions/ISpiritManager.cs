using System.Collections.Generic;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Spirit;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface ISpiritManager
    {
        IReadOnlyList<ISpirit> Unlocked { get; }
        IReadOnlyList<ISpirit?> Equipped { get; }
        void Unlock(ISpirit spirit);
        bool Equip(int slot, ISpirit spirit);
        void Unequip(int slot);
        bool TryActivate(int slot, Player player);
        void Update(float deltaTime, Player player);
    }
}
