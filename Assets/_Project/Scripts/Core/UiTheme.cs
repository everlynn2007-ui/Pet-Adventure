using PetAdventure.Pets;
using UnityEngine;

namespace PetAdventure.Core
{
    /// <summary>Bảng màu pastel có ngữ nghĩa. Mỗi nhóm nút/panel một màu, dễ phân biệt.</summary>
    public static class UiTheme
    {
        // Nền và khung
        public static readonly Color Cream = Hex("FFF8E9");
        public static readonly Color Panel = Hex("FFF0D2");
        public static readonly Color PanelAlt = Hex("F2D7A8");
        public static readonly Color Slot = Hex("EAD3A8");
        public static readonly Color Border = Hex("8B5E3C");

        // Chữ
        public static readonly Color TextDark = Hex("4A3728");
        public static readonly Color TextMuted = Hex("8A7561");
        public static readonly Color TextLight = Hex("FFFDF7");

        // Hành động
        public static readonly Color Primary = Hex("5FAE68");       // xanh lá: xác nhận, lưu, đăng nhập
        public static readonly Color PrimaryDark = Hex("3F7D47");
        public static readonly Color Secondary = Hex("6FA8DC");     // xanh dương: mở màn, quay lại
        public static readonly Color SecondaryDark = Hex("3F7CB0");
        public static readonly Color Accent = Hex("F0A05A");        // cam: bắt thú, chọn đồng hành
        public static readonly Color AccentDark = Hex("A85A22");
        public static readonly Color Danger = Hex("D96C6C");        // đỏ: thoát, về menu, lỗi
        public static readonly Color DangerDark = Hex("A44B4B");
        public static readonly Color Gold = Hex("E8B65A");          // vàng: tùy chọn, cấp độ

        // Dữ liệu
        public static readonly Color Dim = new Color(0.20f, 0.13f, 0.07f, 0.62f);
        public static readonly Color HpGreen = Hex("6FCF6F");
        public static readonly Color HpBack = Hex("D9C9B0");

        private static Font font;

        public static Font Font
        {
            get
            {
                if (font == null)
                {
                    font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                return font;
            }
        }

        public static Color ElementColor(PetElement element)
        {
            switch (element)
            {
                case PetElement.Fire: return Hex("F4784A");
                case PetElement.Water: return Hex("5AA9E6");
                case PetElement.Nature: return Hex("7FB77E");
                case PetElement.Lightning: return Hex("E4C55A");
                case PetElement.Earth: return Hex("B08968");
                default: return Primary;
            }
        }

        public static Color Hex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : "#" + hex, out Color color))
            {
                return color;
            }

            return Color.magenta;
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color Lighten(this Color color, float amount)
        {
            return Color.Lerp(color, Color.white, Mathf.Clamp01(amount));
        }

        public static Color Darken(this Color color, float amount)
        {
            return Color.Lerp(color, Color.black, Mathf.Clamp01(amount));
        }
    }
}
