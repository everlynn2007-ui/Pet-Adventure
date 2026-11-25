using System;
using UnityEngine;

namespace PetAdventure.Pets
{
    /// <summary>Dữ liệu một linh thú người chơi đang sở hữu. Tương ứng một dòng bảng Characters.</summary>
    [Serializable]
    public sealed class PetRuntimeData
    {
        public int id;
        public int accountId;
        public string name = "";
        public PetElement element = PetElement.Fire;
        public int level = 1;
        public int hp = PetStats.BaseHp;
        public string description = "";
        public string petKey = "";

        public int MaxHp => PetStats.MaxHp(level);

        public float HpRatio => MaxHp <= 0 ? 0f : Mathf.Clamp01((float)hp / MaxHp);

        public bool IsAtFullHp => hp >= MaxHp;

        public PetRuntimeData Clone()
        {
            return new PetRuntimeData
            {
                id = id,
                name = name,
                element = element,
                level = level,
                hp = hp,
                description = description,
                petKey = petKey
            };
        }

        public void ApplyLevelAndHp(int newLevel, int newHp)
        {
            level = Mathf.Clamp(newLevel, PetStats.MinLevel, PetStats.MaxLevel);
            hp = PetStats.ClampHp(newHp, level);
        }
    }
}
