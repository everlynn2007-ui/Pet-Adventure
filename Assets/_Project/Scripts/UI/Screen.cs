using System;
using PetAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI
{
    /// <summary>Khung chung của một màn hình. Không chứa logic gameplay.</summary>
    public abstract class Screen
    {
        protected ScreenManager Manager { get; private set; }
        protected RectTransform Root { get; private set; }

        public void Build(Transform parent, ScreenManager manager)
        {
            Manager = manager;
            Root = UiFactory.Node(GetType().Name, parent);
            UiFactory.Fill(Root);
            BuildContent();
            Root.gameObject.SetActive(false);
        }

        protected abstract void BuildContent();

        public virtual void Show()
        {
            Root.gameObject.SetActive(true);
            Root.SetAsLastSibling();
            OnShow();
        }

        public virtual void Hide()
        {
            OnHide();
            Root.gameObject.SetActive(false);
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected RectTransform CreateCard(Vector2 size, bool dim = true, int radius = 24)
        {
            if (dim)
            {
                Image shade = UiFactory.Panel(Root, "Dim", 2, UiTheme.Dim, UiTheme.Dim);
                UiFactory.Fill(shade.rectTransform);
            }

            Image card = UiFactory.Panel(Root, "Card", radius);
            UiFactory.Center(card.rectTransform, size);
            return card.rectTransform;
        }

        protected static Text LabelAt(RectTransform parent, string text, int fontSize, Color color,
            float y, float width, TextAnchor anchor = TextAnchor.MiddleCenter,
            FontStyle style = FontStyle.Normal, float height = 0f)
        {
            Text label = UiFactory.Label(parent, text, fontSize, color, anchor, style);
            UiFactory.Place(label.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, y), new Vector2(width, height > 0f ? height : fontSize + 12f));
            return label;
        }

        protected static Button ButtonAt(RectTransform parent, string caption, Color fill,
            Action action, float y, float width = 420f, float height = 68f, int fontSize = 28,
            Color? textColor = null)
        {
            Button button = UiFactory.Button(parent, caption, fill, textColor ?? UiTheme.TextLight, action, fontSize);
            UiFactory.Place((RectTransform)button.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, y), new Vector2(width, height));
            return button;
        }

        protected static InputField InputAt(RectTransform parent, string placeholder, bool password,
            float y, float width = 460f, float height = 64f)
        {
            InputField field = UiFactory.Input(parent, placeholder, password);
            UiFactory.Place((RectTransform)field.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, y), new Vector2(width, height));
            return field;
        }
    }
}
