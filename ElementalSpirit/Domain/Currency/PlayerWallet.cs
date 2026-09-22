using System;

namespace ElementalSpirit.Domain.Currency
{
    public class PlayerWallet
    {
        public int Gold { get; private set; }
        public int SpiritShards { get; private set; }
        public int Crystals { get; private set; }

        public PlayerWallet(int gold = 0, int spiritShards = 0, int crystals = 0)
        {
            Gold = gold;
            SpiritShards = spiritShards;
            Crystals = crystals;
        }

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            Gold += amount;
        }

        public void AddSpiritShards(int amount)
        {
            if (amount <= 0) return;
            SpiritShards += amount;
        }

        public void AddCrystals(int amount)
        {
            if (amount <= 0) return;
            Crystals += amount;
        }

        public bool TrySpendGold(int amount)
        {
            if (amount <= 0 || Gold < amount) return false;
            Gold -= amount;
            return true;
        }

        public bool TrySpendSpiritShards(int amount)
        {
            if (amount <= 0 || SpiritShards < amount) return false;
            SpiritShards -= amount;
            return true;
        }

        public bool TrySpendCrystals(int amount)
        {
            if (amount <= 0 || Crystals < amount) return false;
            Crystals -= amount;
            return true;
        }

        public void LoadFrom(int gold, int spiritShards, int crystals)
        {
            Gold = Math.Max(0, gold);
            SpiritShards = Math.Max(0, spiritShards);
            Crystals = Math.Max(0, crystals);
        }
    }
}