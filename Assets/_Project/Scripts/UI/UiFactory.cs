using System;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.Core
{
    /// <summary>Hàm dựng UI dùng lại cho mọi màn hình.</summary>
    public static class UiFactory
    {
        public static RectTransform Node(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static void Fill(RectTransform rt, float left = 0f, float bottom = 0f, float right = 0f, float top = 0f)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        public static void Place(RectTransform rt, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
        }

        public static void Center(RectTransform rt, Vector2 size)
        {
            Place(rt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, size);
        }

        public static Image RawImage(Transform parent, Sprite sprite, Color color, string name = "Image")
        {
            RectTransform rt = Node(name, parent);
            Image image = rt.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = Image.Type.Simple;
            return image;
        }

        public static Image Panel(Transform parent, string name = "Panel", int radius = 14)
        {
            return Panel(parent, name, radius, UiTheme.Panel, UiTheme.Border);
        }

        public static Image Panel(Transform parent, string name, int radius, Color fill, Color border)
        {
            RectTransform rt = Node(name, parent);
            Image image = rt.gameObject.AddComponent<Image>();
            // Màu nằm ở Image.color để Unity UI luôn áp dụng đúng, kể cả với sprite sinh lúc chạy.
            image.sprite = SpriteFactory.Panel(Color.white, border, Mathf.Max(radius, 2));
            image.type = Image.Type.Sliced;
            image.color = fill;
            return image;
        }

        public static Text Label(Transform parent, string text, int fontSize, Color color,
            TextAnchor anchor = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
        {
            RectTransform rt = Node("Label", parent);
            Text label = rt.gameObject.AddComponent<Text>();
            label.font = UiTheme.Font;
            label.text = text;
            label.fontSize = fontSize;
            label.color = color;
            label.alignment = anchor;
            label.fontStyle = style;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            return label;
        }

        public static Button Button(Transform parent, string caption, Color fill, Color textColor,
            Action onClick, int fontSize = 28)
        {
            Image background = Panel(parent, "Button", 14, fill, fill.Darken(0.18f));
            Button button = background.gameObject.AddComponent<Button>();
            button.targetGraphic = background;

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(0.75f, 0.75f, 0.75f, 1f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            Text label = Label(background.transform, caption, fontSize, textColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            Fill(label.rectTransform, 8, 4, 8, 4);

            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            return button;
        }

        public static InputField Input(Transform parent, string placeholder, bool password = false, int fontSize = 26)
        {
            Image background = Panel(parent, "Input", 12, UiTheme.Cream, UiTheme.Border);
            InputField field = background.gameObject.AddComponent<InputField>();
            field.targetGraphic = background;

            Text text = Label(background.transform, "", fontSize, UiTheme.TextDark, TextAnchor.MiddleLeft);
            Fill(text.rectTransform, 18, 2, 18, 2);
            text.supportRichText = false;

            Text hint = Label(background.transform, placeholder, fontSize, UiTheme.TextMuted.WithAlpha(0.65f), TextAnchor.MiddleLeft);
            Fill(hint.rectTransform, 18, 2, 18, 2);
            hint.fontStyle = FontStyle.Italic;

            field.textComponent = text;
            field.placeholder = hint;
            field.lineType = InputField.LineType.SingleLine;
            field.contentType = password ? InputField.ContentType.Password : InputField.ContentType.Standard;
            field.characterLimit = 0;
            return field;
        }

        public static Slider Slider(Transform parent, float value)
        {
            RectTransform root = Node("Slider", parent);
            Slider slider = root.gameObject.AddComponent<Slider>();

            Image background = Panel(root, "Background", 10, UiTheme.HpBack, UiTheme.Border);
            Fill(background.rectTransform, 0, 8, 0, 8);
            background.raycastTarget = true;

            RectTransform fillArea = Node("Fill Area", root);
            Fill(fillArea, 8, 8, 8, 8);

            Image fill = RawImage(fillArea, SpriteFactory.White, UiTheme.Primary, "Fill");
            Fill(fill.rectTransform);

            slider.fillRect = fill.rectTransform;
            slider.targetGraphic = background;
            slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = Mathf.Clamp01(value);
            return slider;
        }

        public static Toggle Toggle(Transform parent, bool value, string label, int fontSize = 24)
        {
            RectTransform root = Node("Toggle", parent);
            Toggle toggle = root.gameObject.AddComponent<Toggle>();

            Image box = Panel(root, "Box", 8, UiTheme.Cream, UiTheme.Border);
            Place(box.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), Vector2.zero, new Vector2(34f, 34f));

            Image check = RawImage(box.transform, SpriteFactory.White, UiTheme.Primary, "Check");
            Place(check.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(20f, 20f));

            toggle.targetGraphic = box;
            toggle.graphic = check;
            toggle.isOn = value;

            Text text = Label(root, label, fontSize, UiTheme.TextDark, TextAnchor.MiddleLeft);
            Fill(text.rectTransform, 50, 0, 0, 0);
            return toggle;
        }

        public static ScrollRect Scroll(Transform parent, out RectTransform content)
        {
            RectTransform root = Node("Scroll", parent);
            ScrollRect scroll = root.gameObject.AddComponent<ScrollRect>();

            RectTransform viewport = Node("Viewport", root);
            Fill(viewport);
            viewport.gameObject.AddComponent<RectMask2D>();

            content = Node("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;

            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 40f;
            return scroll;
        }

        public static RectTransform Stack(Transform parent, float width, int spacing = 16)
        {
            RectTransform rt = Node("Stack", parent);
            VerticalLayoutGroup layout = rt.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = rt.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            rt.sizeDelta = new Vector2(width, 0f);
            return rt;
        }

        public static LayoutElement FixedHeight(RectTransform rt, float height)
        {
            LayoutElement element = rt.GetComponent<LayoutElement>();
            if (element == null) element = rt.gameObject.AddComponent<LayoutElement>();
            element.preferredHeight = height;
            element.minHeight = height;
            return element;
        }

        public static Image Icon(Transform parent, Sprite sprite, float size)
        {
            Image image = RawImage(parent, sprite, Color.white, "Icon");
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.rectTransform.sizeDelta = new Vector2(size, size);
            return image;
        }

        public static Image Divider(Transform parent, Color color, float height = 2f)
        {
            Image image = RawImage(parent, SpriteFactory.White, color, "Divider");
            image.raycastTarget = false;
            image.rectTransform.sizeDelta = new Vector2(0f, height);
            return image;
        }

        public static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}
