using PetAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class PauseScreen : Screen
    {
        protected override void BuildContent()
        {
            RectTransform card = CreateCard(new Vector2(600f, 520f));

            LabelAt(card, "Tạm dừng", 44, UiTheme.TextDark, -40f, 520f, TextAnchor.MiddleCenter, FontStyle.Bold, 56f);
            LabelAt(card, "Khu vực Greenleaf Village", 20, UiTheme.TextMuted, -104f, 520f, TextAnchor.MiddleCenter, FontStyle.Italic, 30f);

            ButtonAt(card, "Tiếp tục", UiTheme.Primary, ContinueGame, -160f, 380f, 68f, 28);
            ButtonAt(card, "Cài đặt", UiTheme.Secondary, () => Manager.Navigate(ScreenId.Settings), -244f, 380f, 68f, 28, UiTheme.TextLight);
            ButtonAt(card, "Về Main Menu", UiTheme.Danger, ToMainMenu, -328f, 380f, 68f, 28, UiTheme.TextLight);
        }

        protected override void OnShow()
        {
            Time.timeScale = 0f;
        }

        private void ContinueGame()
        {
            Time.timeScale = 1f;
            Manager.Back();
        }

        private void ToMainMenu()
        {
            Time.timeScale = 1f;
            Manager.Reset(ScreenId.MainMenu);
        }
    }
}
