using ElementalSpirit.Data;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Inventory;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Services
{
    public enum PurchaseResult
    {
        Success,
        NotFound,
        AlreadyOwned,
        NotEnoughGold
    }

    public class ShopService : IShopService
    {
        public PurchaseResult TryBuy(string itemId, PlayerWallet wallet, Inventory inventory)
        {
            var template = EquipmentCatalog.Find(itemId);
            if (template == null) return PurchaseResult.NotFound;
            if (inventory.Owns(itemId)) return PurchaseResult.AlreadyOwned;
            if (!wallet.TrySpendGold(template.PriceGold))
                return PurchaseResult.NotEnoughGold;

            inventory.Add(template.Clone());
            return PurchaseResult.Success;
        }
    }
}