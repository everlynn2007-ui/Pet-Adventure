using System.Collections.Generic;

namespace PetAdventure.Core
{
    public sealed class InventoryItem
    {
        public string Name;
        public string Description;
        public string IconKey;
        public int Count;

        public InventoryItem(string name, string description, string iconKey, int count)
        {
            Name = name;
            Description = description;
            IconKey = iconKey;
            Count = count;
        }
    }

    /// <summary>Túi đồ lưu local. ASM 1 không có bảng item nên không đồng bộ lên server.</summary>
    public static class InventoryData
    {
        public static List<InventoryItem> CreateDefault()
        {
            return new List<InventoryItem>
            {
                new InventoryItem("Thức ăn thú", "Hồi 30 HP cho một linh thú.", "food", 5),
                new InventoryItem("Đá tiến hoá", "Dùng để tiến hoá linh thú ở cấp 10.", "gem", 2),
                new InventoryItem("Bóng bắt thú", "Bắt linh thú hoang trong làng.", "ball", 8),
                new InventoryItem("Thảo dược", "Hồi 10 HP, dùng ngoài trận.", "herb", 12)
            };
        }
    }
}
