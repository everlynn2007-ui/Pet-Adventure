using UnityEngine;

namespace PetAdventure.Pets
{
    /// <summary>Chỉ số dùng chung. Backend chỉ lưu Level và Hp, MaxHP suy ra ở client.</summary>
    public static class PetStats
    {
        public const int MinLevel = 1;
        public const int MaxLevel = 100;
        public const int MinHp = 0;
        public const int MaxHpLimit = 100000;
        public const int BaseHp = 100;
        public const int HpPerLevel = 50;

        public static int MaxHp(int level)
        {
            int clamped = Mathf.Clamp(level, MinLevel, MaxLevel);
            return BaseHp + HpPerLevel * (clamped - 1);
        }

        public static int ClampHp(int hp, int level)
        {
            return Mathf.Clamp(hp, MinHp, MaxHp(level));
        }
    }
}
