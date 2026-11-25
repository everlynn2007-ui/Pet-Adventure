using System.Collections.Generic;
using PetAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class InventoryScreen : Screen
    {
        private readonly List<InventoryItem> items = InventoryData.CreateDefault();
        private RectTransform content;
        private Text selectedName;
        private Text selectedDescription;

        protected override void BuildContent()
        {
            RectTransform card = CreateCard(new Vector2(1240f, 820f));

            LabelAt(card, "Túi đồ", 44, UiTheme.TextDark, -22f, 600f, TextAnchor.MiddleCenter, FontStyle.Bold, 54f);

            Button back = UiFactory.Button(card, "← Quay lại", UiTheme.Secondary, UiTheme.TextLight, () => Manager.Back(), 22);
            UiFactory.Place((RectTransform)back.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(28f, -22f), new Vector2(190f, 52f));

            ScrollRect scroll = UiFactory.Scroll(card, out content);
            UiFactory.Fill((RectTransform)scroll.transform, 32f, 200f, 32f, 96f);

            GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(200f, 200f);
            grid.spacing = new Vector2(18f, 18f);
            grid.padding = new RectOffset(16, 16, 16, 16);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            grid.childAlignment = TextAnchor.UpperCenter;

            Image detail = UiFactory.Panel(card, "Detail", 16, UiTheme.Cream, UiTheme.Border);
            UiFactory.Place(detail.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 28f), new Vector2(1120f, 150f));

            selectedName = UiFactory.Label(detail.transform, "Chọn một vật phẩm", 26, UiTheme.TextDark, TextAnchor.MiddleLeft, FontStyle.Bold);
            UiFactory.Place(selectedName.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(24f, -14f), new Vector2(1060f, 34f));

            selectedDescription = UiFactory.Label(detail.transform, "Túi đồ lưu cục bộ, không đồng bộ lên server.", 20, UiTheme.TextMuted, TextAnchor.UpperLeft);
            UiFactory.Place(selectedDescription.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(24f, -56f), new Vector2(1060f, 78f));

            BuildSlots();
        }

        private void BuildSlots()
        {
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItem item = items[i];
                Image slot = UiFactory.Panel(content, "Slot_" + item.Name, 16, UiTheme.Slot, UiTheme.Border);
                Button button = slot.gameObject.AddComponent<Button>();
                button.targetGraphic = slot;
                button.onClick.AddListener(() => Show(item));

                Image icon = UiFactory.Icon(slot.transform, SpriteFactory.ItemIcon(item.IconKey), 96f);
                UiFactory.Place(icon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                    new Vector2(0f, -18f), new Vector2(96f, 96f));

                Text name = UiFactory.Label(slot.transform, item.Name, 18, UiTheme.TextDark, TextAnchor.MiddleCenter, FontStyle.Bold);
                UiFactory.Place(name.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                    new Vector2(0f, 16f), new Vector2(180f, 46f));

                Text count = UiFactory.Label(slot.transform, "x" + item.Count, 18, UiTheme.TextMuted, TextAnchor.MiddleRight, FontStyle.Bold);
                UiFactory.Place(count.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f),
                    new Vector2(-10f, -10f), new Vector2(70f, 26f));
            }

            for (int i = items.Count; i < 10; i++)
            {
                Image empty = UiFactory.Panel(content, "Empty_" + i, 16, UiTheme.PanelAlt.WithAlpha(0.5f), UiTheme.Border.WithAlpha(0.4f));
                empty.raycastTarget = false;
            }
        }

        private void Show(InventoryItem item)
        {
            selectedName.text = item.Name + "  x" + item.Count;
            selectedDescription.text = item.Description;
        }
    }
}
