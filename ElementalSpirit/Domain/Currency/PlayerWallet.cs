namespace ElementalSpirit.Domain.Currency
{
    public class PlayerWallet
    {
        public int Gold { get; private set; }
        public int SpiritShards { get; private set; }
        public int Crystals { get; private set; }

        public PlayerWallet(int gold = 200, int spiritShards = 15, int crystals = 0)
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
    }
}