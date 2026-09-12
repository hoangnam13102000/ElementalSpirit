using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Spirit;

namespace ElementalSpirit.Services.Abstractions
{
    public interface IUpgradeService
    {
        int GetUpgradeCost(ISpirit spirit);
        bool CanUpgrade(ISpirit spirit, PlayerWallet wallet);
        bool TryUpgrade(ISpirit spirit, PlayerWallet wallet);
    }
}
