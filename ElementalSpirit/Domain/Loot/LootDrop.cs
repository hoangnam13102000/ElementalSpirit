using System.Drawing;

namespace ElementalSpirit.Domain.Loot
{
    public enum LootType
    {
        Gold,
        Boots,
        HealthPotion
    }

    /// <summary>
    /// Đại diện cho 1 vật phẩm rơi ra tại chỗ quái chết, nằm chờ trên mặt đất
    /// cho tới khi player đi tới nhặt (va chạm bounds). Không tự động cộng
    /// vào ví/túi đồ ngay lúc quái chết.
    /// </summary>
    public class LootDrop
    {
        public LootType Type { get; }
        public float X { get; }
        public float Y { get; }
        public int Width => 18;
        public int Height => 18;
        public int GoldAmount { get; }
        public int HealPercent { get; }

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        public static LootDrop CreateGold(float x, float y, int amount) =>
            new(LootType.Gold, x, y, amount);

        public static LootDrop CreateHealthPotion(float x, float y, int healPercent = 10) =>
            new(LootType.HealthPotion, x, y, 0, healPercent);

        public static LootDrop CreateBoots(float x, float y) =>
            new(LootType.Boots, x, y, 0);

        private LootDrop(
            LootType type,
            float x,
            float y,
            int goldAmount,
            int healPercent = 0)
        {
            Type = type;
            X = x;
            Y = y;
            GoldAmount = goldAmount;
            HealPercent = healPercent;
        }
    }
}