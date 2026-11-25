using System.Collections.Generic;
using PetAdventure.Pets;
using UnityEngine;

namespace PetAdventure.Core
{
    /// <summary>Sinh sprite placeholder bằng code để dự án chạy được khi chưa có art thật.</summary>
    public static class SpriteFactory
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();
        private static Sprite white;

        public static Sprite White
        {
            get
            {
                if (white == null)
                {
                    Color[] pixels = new Color[16];
                    for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                    white = ToSprite("white", 4, 4, pixels, 100f, Vector4.zero);
                }

                return white;
            }
        }

        public static Sprite Panel(Color fill, Color border, int radius = 14, int thickness = 2)
        {
            string key = "panel:" + ColorUtility.ToHtmlStringRGBA(fill) + ColorUtility.ToHtmlStringRGBA(border) + radius + ":" + thickness;
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int size = 64;
            Color[] pixels = new Color[size * size];
            float half = size * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float qx = Mathf.Abs(x + 0.5f - half) - (half - radius);
                    float qy = Mathf.Abs(y + 0.5f - half) - (half - radius);
                    float outside = new Vector2(Mathf.Max(qx, 0f), Mathf.Max(qy, 0f)).magnitude;
                    float distance = outside + Mathf.Min(Mathf.Max(qx, qy), 0f) - radius;
                    float alpha = Mathf.Clamp01(0.5f - distance);
                    Color color = distance > -thickness ? border : fill;
                    color.a *= alpha;
                    pixels[y * size + x] = color;
                }
            }

            float edge = radius + 2f;
            Sprite sprite = ToSprite(key, size, size, pixels, 100f, new Vector4(edge, edge, edge, edge));
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite PetIcon(PetElement element)
        {
            string key = "petIcon:" + element;
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int size = 96;
            Color[] pixels = new Color[size * size];
            Color main = UiTheme.ElementColor(element);
            Color light = main.Lighten(0.55f);
            Color dark = main.Darken(0.30f);

            Circle(pixels, size, size, 30, 62, 12, dark);
            Circle(pixels, size, size, 66, 62, 12, dark);
            Circle(pixels, size, size, 30, 62, 7, light);
            Circle(pixels, size, size, 66, 62, 7, light);

            Ellipse(pixels, size, size, 48, 34, 30, 24, main);
            Ellipse(pixels, size, size, 48, 30, 18, 14, light);

            Circle(pixels, size, size, 48, 56, 26, main);
            Ellipse(pixels, size, size, 48, 50, 17, 12, light);

            Circle(pixels, size, size, 38, 60, 5, Color.white);
            Circle(pixels, size, size, 58, 60, 5, Color.white);
            Circle(pixels, size, size, 39, 59, 2, UiTheme.TextDark);
            Circle(pixels, size, size, 59, 59, 2, UiTheme.TextDark);
            Circle(pixels, size, size, 48, 49, 3, dark);

            Circle(pixels, size, size, 22, 30, 6, main);
            Circle(pixels, size, size, 74, 30, 6, main);

            Sprite sprite = ToSprite(key, size, size, pixels, 96f, Vector4.zero, FilterMode.Bilinear);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite PetWorld(PetElement element)
        {
            string key = "petWorld:" + element;
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int size = 32;
            Color[] pixels = new Color[size * size];
            Color main = UiTheme.ElementColor(element);
            Color dark = main.Darken(0.25f);

            Circle(pixels, size, size, 10, 22, 4, dark);
            Circle(pixels, size, size, 22, 22, 4, dark);
            Ellipse(pixels, size, size, 16, 11, 9, 8, main);
            Circle(pixels, size, size, 16, 20, 8, main);
            Circle(pixels, size, size, 13, 21, 2, Color.white);
            Circle(pixels, size, size, 19, 21, 2, Color.white);
            Circle(pixels, size, size, 13, 20, 1, Color.black);
            Circle(pixels, size, size, 19, 20, 1, Color.black);

            Sprite sprite = ToSprite(key, size, size, pixels, 32f, Vector4.zero);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite Player()
        {
            const string key = "player";
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int w = 32;
            const int h = 48;
            Color[] pixels = new Color[w * h];
            Color skin = UiTheme.Hex("F2C6A0");
            Color hair = UiTheme.Hex("5A4030");
            Color tunic = UiTheme.Hex("6FA8DC");
            Color pants = UiTheme.Hex("4A6FA5");

            Rect(pixels, w, h, 9, 0, 14, 10, pants);
            Rect(pixels, w, h, 17, 0, 22, 10, pants);
            Rect(pixels, w, h, 8, 10, 24, 28, tunic);
            Rect(pixels, w, h, 6, 12, 8, 24, tunic);
            Rect(pixels, w, h, 24, 12, 26, 24, tunic);
            Circle(pixels, w, h, 16, 36, 9, skin);
            Ellipse(pixels, w, h, 16, 42, 10, 6, hair);
            Rect(pixels, w, h, 7, 38, 25, 41, hair);
            Circle(pixels, w, h, 13, 35, 2, UiTheme.TextDark);
            Circle(pixels, w, h, 19, 35, 2, UiTheme.TextDark);
            Rect(pixels, w, h, 14, 31, 18, 32, UiTheme.Hex("C98A7A"));

            Sprite sprite = ToSprite(key, w, h, pixels, 32f, Vector4.zero);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite Ground()
        {
            const string key = "ground";
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int w = 960;
            const int h = 540;
            Color[] pixels = new Color[w * h];
            Color grassA = UiTheme.Hex("8CCF6B");
            Color grassB = UiTheme.Hex("7DC35E");
            Color stone = UiTheme.Hex("C9C3B0");
            Color stoneDark = UiTheme.Hex("B0AA98");
            Color water = UiTheme.Hex("63B8E8");
            Color waterDark = UiTheme.Hex("4A9BCB");

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool alt = ((x / 16) + (y / 16)) % 2 == 0;
                    pixels[y * w + x] = alt ? grassA : grassB;
                }
            }

            Rect(pixels, w, h, 0, 250, w, 292, stone);
            Rect(pixels, w, h, 430, 0, 474, h, stone);

            var rng = new System.Random(20261125);
            for (int i = 0; i < 900; i++)
            {
                int sx = rng.Next(w);
                int sy = rng.Next(h);
                bool onPath = (sy >= 252 && sy <= 290) || (sx >= 432 && sx <= 472);
                if (!onPath) continue;
                int r = rng.Next(2, 5);
                Circle(pixels, w, h, sx, sy, r, stoneDark);
            }

            Ellipse(pixels, w, h, 760, 140, 98, 62, waterDark);
            Ellipse(pixels, w, h, 760, 145, 88, 54, water);

            for (int i = 0; i < 260; i++)
            {
                int fx = rng.Next(w);
                int fy = rng.Next(h);
                bool onPath = (fy >= 246 && fy <= 296) || (fx >= 426 && fx <= 478);
                if (onPath) continue;
                if (Mathf.Abs(fx - 760) < 100 && Mathf.Abs(fy - 145) < 62) continue;
                Color petal = rng.Next(3) == 0 ? UiTheme.Hex("FFE9A8") : Color.white;
                Circle(pixels, w, h, fx, fy, 1, petal);
            }

            Sprite sprite = ToSprite(key, w, h, pixels, 16f, Vector4.zero);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite House(int variant)
        {
            string key = "house:" + variant;
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int size = 96;
            Color[] pixels = new Color[size * size];
            Color wall = UiTheme.Hex("F5E2C0");
            Color wallDark = UiTheme.Hex("E0C9A2");
            Color roof = variant == 0 ? UiTheme.Hex("C9553F") : variant == 1 ? UiTheme.Hex("D97742") : UiTheme.Hex("B4553F");
            Color door = UiTheme.Hex("8B5E3C");
            Color window = UiTheme.Hex("8ED4F0");

            Rect(pixels, size, size, 10, 4, 86, 52, wallDark);
            Rect(pixels, size, size, 12, 6, 84, 50, wall);

            for (int row = 0; row < 40; row++)
            {
                int inset = Mathf.Max(0, 42 - row);
                Rect(pixels, size, size, inset, 50 + row, size - inset, 51 + row, roof);
            }

            Rect(pixels, size, size, 40, 6, 56, 30, door);
            Rect(pixels, size, size, 20, 24, 34, 40, window);
            Rect(pixels, size, size, 62, 24, 76, 40, window);
            Rect(pixels, size, size, 46, 8, 50, 12, UiTheme.Gold);

            Sprite sprite = ToSprite(key, size, size, pixels, 32f, Vector4.zero);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite Tree()
        {
            const string key = "tree";
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int w = 48;
            const int h = 64;
            Color[] pixels = new Color[w * h];
            Color trunk = UiTheme.Hex("8B5E3C");
            Color leafA = UiTheme.Hex("5FA855");
            Color leafB = UiTheme.Hex("77C167");

            Rect(pixels, w, h, 21, 0, 27, 26, trunk);
            Circle(pixels, w, h, 24, 42, 18, leafA);
            Circle(pixels, w, h, 16, 34, 12, leafA);
            Circle(pixels, w, h, 32, 34, 12, leafA);
            Circle(pixels, w, h, 20, 46, 10, leafB);
            Circle(pixels, w, h, 30, 44, 8, leafB);

            Sprite sprite = ToSprite(key, w, h, pixels, 32f, Vector4.zero);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite Bush()
        {
            const string key = "bush";
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int size = 32;
            Color[] pixels = new Color[size * size];
            Color leaf = UiTheme.Hex("4F8F46");
            Color leafLight = UiTheme.Hex("6AAE5C");
            Circle(pixels, size, size, 12, 12, 9, leaf);
            Circle(pixels, size, size, 21, 12, 8, leaf);
            Circle(pixels, size, size, 16, 18, 9, leafLight);

            Sprite sprite = ToSprite(key, size, size, pixels, 32f, Vector4.zero);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite Fence()
        {
            const string key = "fence";
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int w = 48;
            const int h = 24;
            Color[] pixels = new Color[w * h];
            Color wood = UiTheme.Hex("A9794B");
            Rect(pixels, w, h, 0, 8, w, 12, wood);
            Rect(pixels, w, h, 4, 2, 9, 22, wood);
            Rect(pixels, w, h, 20, 2, 25, 22, wood);
            Rect(pixels, w, h, 38, 2, 43, 22, wood);

            Sprite sprite = ToSprite(key, w, h, pixels, 32f, Vector4.zero);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite ItemIcon(string kind)
        {
            string key = "item:" + kind;
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int size = 48;
            Color[] pixels = new Color[size * size];
            switch (kind)
            {
                case "food":
                    Ellipse(pixels, size, size, 24, 18, 16, 9, UiTheme.Hex("C9A227"));
                    Ellipse(pixels, size, size, 24, 22, 12, 7, UiTheme.Hex("F2D06B"));
                    Circle(pixels, size, size, 24, 30, 5, UiTheme.Hex("D96C6C"));
                    break;
                case "gem":
                    Circle(pixels, size, size, 24, 24, 14, UiTheme.Hex("5AA9E6"));
                    Circle(pixels, size, size, 20, 28, 5, Color.white);
                    break;
                case "ball":
                    Circle(pixels, size, size, 24, 24, 15, UiTheme.Hex("D96C6C"));
                    Rect(pixels, size, size, 9, 21, 39, 27, Color.white);
                    Circle(pixels, size, size, 24, 24, 5, UiTheme.TextDark);
                    break;
                case "herb":
                    Ellipse(pixels, size, size, 18, 20, 8, 14, UiTheme.Hex("6AAE5C"));
                    Ellipse(pixels, size, size, 30, 26, 8, 14, UiTheme.Hex("7DC35E"));
                    break;
                default:
                    Rect(pixels, size, size, 10, 10, 38, 38, UiTheme.Accent);
                    break;
            }

            Sprite sprite = ToSprite(key, size, size, pixels, 48f, Vector4.zero);
            Cache[key] = sprite;
            return sprite;
        }

        // ---- primitives ----

        private static void Rect(Color[] pixels, int w, int h, int x0, int y0, int x1, int y1, Color color)
        {
            x0 = Mathf.Clamp(x0, 0, w);
            x1 = Mathf.Clamp(x1, 0, w);
            y0 = Mathf.Clamp(y0, 0, h);
            y1 = Mathf.Clamp(y1, 0, h);

            for (int y = y0; y < y1; y++)
            {
                for (int x = x0; x < x1; x++)
                {
                    pixels[y * w + x] = color;
                }
            }
        }

        private static void Ellipse(Color[] pixels, int w, int h, int cx, int cy, int rx, int ry, Color color)
        {
            int minX = Mathf.Max(0, cx - rx);
            int maxX = Mathf.Min(w - 1, cx + rx);
            int minY = Mathf.Max(0, cy - ry);
            int maxY = Mathf.Min(h - 1, cy + ry);
            float invRx = 1f / Mathf.Max(1, rx);
            float invRy = 1f / Mathf.Max(1, ry);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float nx = (x - cx) * invRx;
                    float ny = (y - cy) * invRy;
                    if (nx * nx + ny * ny <= 1f)
                    {
                        pixels[y * w + x] = color;
                    }
                }
            }
        }

        private static void Circle(Color[] pixels, int w, int h, int cx, int cy, int radius, Color color)
        {
            Ellipse(pixels, w, h, cx, cy, radius, radius, color);
        }

        private static Sprite ToSprite(string key, int w, int h, Color[] pixels, float pixelsPerUnit, Vector4 border,
            FilterMode filter = FilterMode.Point)
        {
            Texture2D texture = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = filter,
                wrapMode = TextureWrapMode.Clamp,
                name = key
            };
            texture.SetPixels(pixels);
            texture.Apply(false, false);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f),
                pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
            sprite.name = key;
            return sprite;
        }
    }
}
