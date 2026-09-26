using System.Drawing;

namespace ElementalSpirit.Domain.Loot
{
    public enum LootType
    {
        Gold,
        Equipment
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
        public string? EquipmentId { get; }

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        public static LootDrop CreateGold(float x, float y, int amount) =>
            new(LootType.Gold, x, y, amount, null);

        public static LootDrop CreateEquipment(float x, float y, string equipmentId) =>
            new(LootType.Equipment, x, y, 0, equipmentId);

        private LootDrop(LootType type, float x, float y, int goldAmount, string? equipmentId)
        {
            Type = type;
            X = x;
            Y = y;
            GoldAmount = goldAmount;
            EquipmentId = equipmentId;
        }
    }
}