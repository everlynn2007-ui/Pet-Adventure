using System;

namespace PetAdventure.Pets
{
    public enum PetElement
    {
        Fire = 0,
        Water = 1,
        Nature = 2,
        Lightning = 3,
        Earth = 4
    }

    public static class PetElementInfo
    {
        public static readonly PetElement[] All =
        {
            PetElement.Fire, PetElement.Water, PetElement.Nature,
            PetElement.Lightning, PetElement.Earth
        };

        public static string DisplayName(this PetElement element)
        {
            switch (element)
            {
                case PetElement.Fire: return "Hệ Lửa";
                case PetElement.Water: return "Hệ Nước";
                case PetElement.Nature: return "Hệ Lá";
                case PetElement.Lightning: return "Hệ Sấm";
                case PetElement.Earth: return "Hệ Đất";
                default: return "Không rõ";
            }
        }

        public static string Code(this PetElement element)
        {
            return element.ToString();
        }

        public static PetElement Parse(string code)
        {
            return Enum.TryParse(code, true, out PetElement value) && Enum.IsDefined(typeof(PetElement), value)
                ? value
                : PetElement.Fire;
        }
    }
}
