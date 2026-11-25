using PetAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI
{
    /// <summary>Thanh máu dùng lại ở HUD, thẻ linh thú và màn chi tiết.</summary>
    public sealed class StatBar
    {
        private readonly Image fill;
        private readonly Text label;

        public RectTransform Root { get; }

        private StatBar(Image background, Color fillColor, bool showLabel)
        {
            Root = background.rectTransform;

            fill = UiFactory.RawImage(background.transform, SpriteFactory.White, fillColor, "Fill");
            UiFactory.Fill(fill.rectTransform, 3f, 3f, 3f, 3f);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            fill.fillAmount = 1f;
            fill.raycastTarget = false;

            if (showLabel)
            {
                label = UiFactory.Label(background.transform, "", 16, UiTheme.TextDark, TextAnchor.MiddleCenter, FontStyle.Bold);
                UiFactory.Fill(label.rectTransform);
            }
        }

        public static StatBar Create(Transform parent, Color fillColor, float height = 20f, bool showLabel = true)
        {
            Image background = UiFactory.Panel(parent, "HpBar", Mathf.RoundToInt(height * 0.5f), UiTheme.HpBack, UiTheme.Border);
            background.rectTransform.sizeDelta = new Vector2(260f, height);
            background.raycastTarget = false;
            return new StatBar(background, fillColor, showLabel);
        }

        public void Set(int current, int max)
        {
            float ratio = max <= 0 ? 0f : Mathf.Clamp01((float)current / max);
            fill.fillAmount = ratio;
            fill.color = ratio > 0.5f ? UiTheme.HpGreen : ratio > 0.25f ? UiTheme.Gold : UiTheme.Danger;
            if (label != null)
            {
                label.text = current + "/" + max;
            }
        }
    }
}
