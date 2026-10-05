using System;

namespace ElementalSpirit.Domain.Currency
{
    public class PlayerWallet
    {
        public int Gold { get; private set; }
        public int TotalGoldEarned { get; private set; }

        public PlayerWallet(int gold = 0)
        {
            Gold = Math.Max(0, gold);
            TotalGoldEarned = Gold;
        }

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            Gold += amount;
            TotalGoldEarned += amount;
        }

        public bool TrySpendGold(int amount)
        {
            if (amount <= 0 || Gold < amount) return false;
            Gold -= amount;
            return true;
        }

        public void LoadFrom(int gold, int? totalGoldEarned = null)
        {
            Gold = Math.Max(0, gold);
            TotalGoldEarned = Math.Max(Gold, totalGoldEarned ?? Gold);
        }
    }
}