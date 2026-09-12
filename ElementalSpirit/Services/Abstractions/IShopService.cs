using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Inventory;

namespace ElementalSpirit.Services.Abstractions
{
    public interface IShopService
    {
        PurchaseResult TryBuy(string itemId, PlayerWallet wallet, Inventory inventory);
    }
}
