using PetAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class MainMenuScreen : Screen
    {
        private readonly GameState state;

        public MainMenuScreen(GameState state)
        {
            this.state = state;
        }

        protected override void BuildContent()
        {
            Image backdrop = UiFactory.RawImage(Root, SpriteFactory.Ground(), Color.white, "Backdrop");
            UiFactory.Fill(backdrop.rectTransform);
            backdrop.raycastTarget = false;

            Image shade = UiFactory.Panel(Root, "Shade", 2, UiTheme.Dim, UiTheme.Dim);
            UiFactory.Fill(shade.rectTransform);

            RectTransform card = CreateCard(new Vector2(780f, 640f), dim: false);

            LabelAt(card, "PET ADVENTURE", 64, UiTheme.TextDark, -56f, 700f, TextAnchor.MiddleCenter, FontStyle.Bold, 80f);
            LabelAt(card, "Thế Giới Thú Cưng", 28, UiTheme.TextMuted, -136f, 700f, TextAnchor.MiddleCenter, FontStyle.Italic, 40f);

            Image divider = UiFactory.Divider(card, UiTheme.Border.WithAlpha(0.35f), 3f);
            UiFactory.Place(divider.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -190f), new Vector2(420f, 3f));

            ButtonAt(card, "Chơi", UiTheme.Primary, Play, -230f, 440f, 72f, 30, UiTheme.TextLight);
            ButtonAt(card, "Đăng nhập", UiTheme.Secondary, () => Manager.Reset(ScreenId.Login), -316f, 440f, 72f, 30, UiTheme.TextLight);
            ButtonAt(card, "Cài đặt", UiTheme.Gold, () => Manager.Navigate(ScreenId.Settings), -402f, 440f, 72f, 30, UiTheme.TextDark);
            ButtonAt(card, "Thoát", UiTheme.Danger, Quit, -488f, 440f, 72f, 30, UiTheme.TextLight);

            Text hint = LabelAt(card, "GAM106 • ASM 1 • Unity 2D", 18, UiTheme.TextMuted, -590f, 700f, TextAnchor.MiddleCenter);
            hint.fontStyle = FontStyle.Italic;
        }

        private void Play()
        {
            Manager.Reset(state.IsLoggedIn ? ScreenId.Hud : ScreenId.Login);
        }

        protected override void OnShow()
        {
            Time.timeScale = 1f;
        }

        private static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
