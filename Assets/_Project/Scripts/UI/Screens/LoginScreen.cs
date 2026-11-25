using System.Collections.Generic;
using PetAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class LoginScreen : Screen
    {
        private readonly GameState state;
        private InputField username;
        private InputField password;
        private Text error;

        public LoginScreen(GameState state)
        {
            this.state = state;
        }

        protected override void BuildContent()
        {
            RectTransform card = CreateCard(new Vector2(700f, 620f));

            LabelAt(card, "Đăng nhập", 44, UiTheme.TextDark, -36f, 620f, TextAnchor.MiddleCenter, FontStyle.Bold, 56f);
            LabelAt(card, "Chào mừng trở lại Greenleaf Village", 22, UiTheme.TextMuted, -96f, 620f, TextAnchor.MiddleCenter, FontStyle.Italic, 34f);

            username = InputAt(card, "Tên đăng nhập", false, -156f);
            password = InputAt(card, "Mật khẩu", true, -236f);

            error = LabelAt(card, "", 20, UiTheme.Danger, -292f, 560f, TextAnchor.MiddleCenter, FontStyle.Normal, 40f);

            ButtonAt(card, "Đăng nhập", UiTheme.Primary, Submit, -344f, 460f, 68f, 30);
            ButtonAt(card, "Chưa có tài khoản? Đăng ký", UiTheme.Secondary, () => Manager.Navigate(ScreenId.Register), -426f, 460f, 60f, 24, UiTheme.TextLight);
            ButtonAt(card, "← Quay lại", UiTheme.PanelAlt, () => Manager.Reset(ScreenId.MainMenu), -494f, 220f, 54f, 22, UiTheme.TextDark);

            password.onEndEdit.AddListener(_ => Submit());
        }

        protected override void OnShow()
        {
            error.text = "";
            password.text = "";
        }

        private async void Submit()
        {
            string user = username.text.Trim();
            string pass = password.text;

            if (user.Length < 3 || user.Length > 50)
            {
                error.text = "Tên đăng nhập phải từ 3 đến 50 ký tự";
                return;
            }

            if (pass.Length < 6 || pass.Length > 100)
            {
                error.text = "Mật khẩu phải từ 6 đến 100 ký tự";
                return;
            }

            error.text = "Đang đăng nhập...";

            if (state.UseBackend)
            {
                var result = await state.Api.LoginAsync(user, pass);
                if (!result.Success || result.Value == null)
                {
                    error.text = result.Error;
                    return;
                }

                state.SignIn(result.Value.username, result.Value.token);
                var pets = await state.Api.GetCharactersAsync();
                if (pets.Success && pets.Value != null)
                {
                    List<PetAdventure.Pets.PetRuntimeData> loaded = new List<PetAdventure.Pets.PetRuntimeData>();
                    foreach (var dto in pets.Value) loaded.Add(dto.ToRuntime());
                    state.Pets.ReplaceAll(loaded);
                }

                Manager.Reset(ScreenId.Hud);
                return;
            }

            // shortcut: chế độ demo không cần server, dùng cho Giai đoạn 2.
            state.SignIn(user, "demo-token");
            state.SeedDemoIfEmpty();
            Manager.Reset(ScreenId.Hud);
        }
    }
}
