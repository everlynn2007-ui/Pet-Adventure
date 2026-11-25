using System;
using PetAdventure.Core;
using PetAdventure.Pets;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI
{
    /// <summary>Thẻ linh thú trong màn Bộ sưu tập.</summary>
    public static class PetCardView
    {
        public static RectTransform Build(Transform parent, PetRuntimeData pet, Action<PetRuntimeData> onClick)
        {
            Image card = UiFactory.Panel(parent, "PetCard", 18);
            card.raycastTarget = true;

            Button button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = card;
            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick(pet));
            }

            Image icon = UiFactory.Icon(card.transform, PetCatalog.IconFor(pet.element), 116f);
            UiFactory.Place(icon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -14f), new Vector2(116f, 116f));

            Text name = UiFactory.Label(card.transform, pet.name, 24, UiTheme.TextDark, TextAnchor.MiddleCenter, FontStyle.Bold);
            UiFactory.Place(name.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -138f), new Vector2(310f, 32f));

            Color elementColor = UiTheme.ElementColor(pet.element);
            Image chip = UiFactory.Panel(card.transform, "ElementChip", 10, elementColor.Lighten(0.68f), elementColor);
            UiFactory.Place(chip.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -178f), new Vector2(172f, 34f));

            Text chipLabel = UiFactory.Label(chip.transform, pet.element.DisplayName(), 18, UiTheme.TextDark, TextAnchor.MiddleCenter, FontStyle.Bold);
            UiFactory.Fill(chipLabel.rectTransform);

            Text level = UiFactory.Label(card.transform, "Cấp " + pet.level, 20, UiTheme.TextMuted, TextAnchor.MiddleCenter);
            UiFactory.Place(level.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -222f), new Vector2(300f, 28f));

            StatBar bar = StatBar.Create(card.transform, UiTheme.HpGreen, 20f, true);
            UiFactory.Place(bar.Root, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -258f), new Vector2(270f, 20f));
            bar.Set(pet.hp, pet.MaxHp);

            return card.rectTransform;
        }
    }
}
