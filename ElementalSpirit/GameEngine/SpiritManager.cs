using System.Collections.Generic;
using ElementalSpirit.Domain.Spirit;
using ElementalSpirit.Factories;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.GameEngine
{
    using PlayerEntity = ElementalSpirit.Domain.Player.Player;

    public class SpiritManager : ISpiritManager
    {
        public const int MaxEquipped = 2;
        private readonly List<ISpirit> _unlocked = new();
        private readonly ISpirit?[] _equipped = new ISpirit?[MaxEquipped];

        public IReadOnlyList<ISpirit> Unlocked => _unlocked;
        public IReadOnlyList<ISpirit?> Equipped => _equipped;

        public SpiritManager()
        {
            Unlock(SpiritFactory.Create(SpiritType.Earth));
            Unlock(SpiritFactory.Create(SpiritType.Fire));
            Unlock(SpiritFactory.Create(SpiritType.Water));
            Unlock(SpiritFactory.Create(SpiritType.Wind));
            Equip(0, _unlocked[0]);
            Equip(1, _unlocked[1]);
        }

        public void Unlock(ISpirit spirit)
        {
            if (_unlocked.Exists(s => s.Id == spirit.Id)) return;
            _unlocked.Add(spirit);
        }

        public bool Equip(int slot, ISpirit spirit)
        {
            if (slot < 0 || slot >= MaxEquipped) return false;
            if (!_unlocked.Contains(spirit)) return false;
            for (int i = 0; i < MaxEquipped; i++)
                if (i != slot && _equipped[i]?.Id == spirit.Id) return false;
            _equipped[slot] = spirit;
            return true;
        }

        public void Unequip(int slot)
        {
            if (slot < 0 || slot >= MaxEquipped) return;
            _equipped[slot] = null;
        }

        public bool TryActivate(int slot, PlayerEntity player)
        {
            if (slot < 0 || slot >= MaxEquipped) return false;
            var spirit = _equipped[slot];
            if (spirit == null || !spirit.CanActivate) return false;
            spirit.Activate(player);
            return true;
        }

        public void Update(float deltaTime, PlayerEntity player)
        {
            foreach (var spirit in _unlocked)
                spirit.Update(deltaTime, player);
        }
    }
}