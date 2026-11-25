using PetAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class SettingsScreen : Screen
    {
        private readonly GameState state;
        private Text volumeValue;
        private Slider volumeSlider;
        private Toggle fullscreenToggle;
        private Toggle backendToggle;

        public SettingsScreen(GameState state)
        {
            this.state = state;
        }

        protected override void BuildContent()
        {
            RectTransform card = CreateCard(new Vector2(760f, 640f));

            LabelAt(card, "Cài đặt", 44, UiTheme.TextDark, -36f, 620f, TextAnchor.MiddleCenter, FontStyle.Bold, 56f);
            LabelAt(card, "Tuỳ chọn hiển thị và âm thanh", 22, UiTheme.TextMuted, -96f, 620f, TextAnchor.MiddleCenter, FontStyle.Italic, 32f);

            Text volumeCaption = UiFactory.Label(card, "Âm lượng", 24, UiTheme.TextDark, TextAnchor.MiddleLeft, FontStyle.Bold);
            UiFactory.Place(volumeCaption.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -160f), new Vector2(560f, 32f));

            volumeSlider = UiFactory.Slider(card, state.MasterVolume);
            UiFactory.Place((RectTransform)volumeSlider.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -200f), new Vector2(560f, 40f));
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

            volumeValue = UiFactory.Label(card, Mathf.RoundToInt(state.MasterVolume * 100f) + "%", 22, UiTheme.TextMuted, TextAnchor.MiddleRight);
            UiFactory.Place(volumeValue.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -252f), new Vector2(560f, 30f));

            fullscreenToggle = UiFactory.Toggle(card, state.Fullscreen, "Toàn màn hình", 24);
            UiFactory.Place((RectTransform)fullscreenToggle.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -306f), new Vector2(560f, 44f));
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

            backendToggle = UiFactory.Toggle(card, state.UseBackend, "Dùng server thật (localhost:5080)", 20);
            UiFactory.Place((RectTransform)backendToggle.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -360f), new Vector2(560f, 44f));
            backendToggle.onValueChanged.AddListener(value => state.UseBackend = value);

            Text hint = UiFactory.Label(card, "Tắt = chế độ demo, không cần server.", 17, UiTheme.TextMuted, TextAnchor.MiddleCenter, FontStyle.Italic);
            UiFactory.Place(hint.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -404f), new Vector2(560f, 26f));

            ButtonAt(card, "← Quay lại", UiTheme.PanelAlt, () => Manager.Back(), -452f, 260f, 58f, 24, UiTheme.TextDark);
            ButtonAt(card, "Về Main Menu", UiTheme.Danger, () => Manager.Reset(ScreenId.MainMenu), -528f, 260f, 58f, 24, UiTheme.TextLight);
        }

        protected override void OnShow()
        {
            volumeSlider.SetValueWithoutNotify(state.MasterVolume);
            volumeValue.text = Mathf.RoundToInt(state.MasterVolume * 100f) + "%";
            fullscreenToggle.SetIsOnWithoutNotify(state.Fullscreen);
            backendToggle.SetIsOnWithoutNotify(state.UseBackend);
        }

        private void OnVolumeChanged(float value)
        {
            state.MasterVolume = Mathf.Clamp01(value);
            AudioListener.volume = state.MasterVolume;
            volumeValue.text = Mathf.RoundToInt(state.MasterVolume * 100f) + "%";
        }

        private void OnFullscreenChanged(bool value)
        {
            state.Fullscreen = value;
            UnityEngine.Screen.fullScreen = value;
        }
    }
}
