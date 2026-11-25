using System.Collections.Generic;
using UnityEngine;

namespace PetAdventure.Pets
{
    /// <summary>Danh mục loài linh thú. Đọc ScriptableObject trong Resources/Pets, thiếu asset thì dựng bản mặc định.</summary>
    public static class PetCatalog
    {
        private const string ResourcePath = "Pets";
        private static PetDefinition[] cached;
        private static PetDefinition[] fallback;

        public static IReadOnlyList<PetDefinition> All
        {
            get
            {
                if (cached == null)
                {
                    cached = Resources.LoadAll<PetDefinition>(ResourcePath);
                    if (cached == null || cached.Length == 0)
                    {
                        cached = Fallback;
                    }
                }

                return cached;
            }
        }

        public static PetDefinition Find(string petKey)
        {
            foreach (PetDefinition definition in All)
            {
                if (definition != null && definition.petKey == petKey)
                {
                    return definition;
                }
            }

            return null;
        }

        public static PetDefinition ForElement(PetElement element)
        {
            foreach (PetDefinition definition in All)
            {
                if (definition != null && definition.element == element)
                {
                    return definition;
                }
            }

            return All.Count > 0 ? All[0] : null;
        }

        public static Sprite IconFor(PetElement element)
        {
            PetDefinition definition = ForElement(element);
            if (definition != null && definition.icon != null)
            {
                return definition.icon;
            }

            return Core.SpriteFactory.PetIcon(element);
        }

        private static PetDefinition[] Fallback
        {
            get
            {
                if (fallback == null)
                {
                    fallback = new[]
                    {
                        Make("firefox", "FireFox", PetElement.Fire, "Tinh linh lửa nhỏ bé, luôn mang ý chí chiến đấu."),
                        Make("aqua", "Aqua", PetElement.Water, "Tinh linh nước, mang lại sự bình yên."),
                        Make("leafy", "Leafy", PetElement.Nature, "Tinh linh thiên nhiên, bảo vệ khu rừng xanh."),
                        Make("lumi", "Lumi", PetElement.Lightning, "Tinh linh sấm sét, nhanh nhẹn và tinh nghịch."),
                        Make("terran", "Terran", PetElement.Earth, "Tinh linh đất, vững chãi và mạnh mẽ.")
                    };
                }

                return fallback;
            }
        }

        private static PetDefinition Make(string key, string name, PetElement element, string description)
        {
            PetDefinition definition = ScriptableObject.CreateInstance<PetDefinition>();
            definition.hideFlags = HideFlags.DontSave;
            definition.Configure(key, name, element, description);
            return definition;
        }
    }
}
