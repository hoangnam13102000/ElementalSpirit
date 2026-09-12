using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Spirit;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Services
{
    public class UpgradeService : IUpgradeService
    {
        public int GetUpgradeCost(ISpirit spirit)
        {
            if (spirit.Level >= 5) return 0;
            return spirit.Level switch
            {
                1 => 5,
                2 => 10,
                3 => 18,
                4 => 30,
                _ => 0
            };
        }

        public bool CanUpgrade(ISpirit spirit, PlayerWallet wallet)
        {
            if (spirit.Level >= 5) return false;
            return wallet.SpiritShards >= GetUpgradeCost(spirit);
        }

        public bool TryUpgrade(ISpirit spirit, PlayerWallet wallet)
        {
            if (!CanUpgrade(spirit, wallet)) return false;
            int cost = GetUpgradeCost(spirit);
            if (!wallet.TrySpendSpiritShards(cost)) return false;
            spirit.Upgrade();
            return true;
        }
    }
}