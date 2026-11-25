using System.Collections.Generic;
using PetAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class RegisterScreen : Screen
    {
        private readonly GameState state;
        private InputField username;
        private InputField password;
        private InputField confirm;
        private Text error;

        public RegisterScreen(GameState state)
        {
            this.state = state;
        }

        protected override void BuildContent()
        {
            RectTransform card = CreateCard(new Vector2(700f, 700f));

            LabelAt(card, "Đăng ký", 44, UiTheme.TextDark, -36f, 620f, TextAnchor.MiddleCenter, FontStyle.Bold, 56f);
            LabelAt(card, "Tạo tài khoản nhà huấn luyện mới", 22, UiTheme.TextMuted, -96f, 620f, TextAnchor.MiddleCenter, FontStyle.Italic, 34f);

            username = InputAt(card, "Tên đăng nhập (3–50 ký tự)", false, -152f);
            password = InputAt(card, "Mật khẩu (6–100 ký tự)", true, -232f);
            confirm = InputAt(card, "Nhập lại mật khẩu", true, -312f);

            error = LabelAt(card, "", 20, UiTheme.Danger, -368f, 560f, TextAnchor.MiddleCenter, FontStyle.Normal, 40f);

            ButtonAt(card, "Tạo tài khoản", UiTheme.Primary, Submit, -420f, 460f, 68f, 30);
            ButtonAt(card, "Đã có tài khoản? Đăng nhập", UiTheme.Secondary, () => Manager.Back(), -502f, 460f, 60f, 24, UiTheme.TextLight);
            ButtonAt(card, "← Quay lại", UiTheme.PanelAlt, () => Manager.Reset(ScreenId.MainMenu), -570f, 220f, 54f, 22, UiTheme.TextDark);
        }

        protected override void OnShow()
        {
            error.text = "";
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

            if (pass != confirm.text)
            {
                error.text = "Hai mật khẩu không khớp";
                return;
            }

            error.text = "Đang tạo tài khoản...";

            if (state.UseBackend)
            {
                var registered = await state.Api.RegisterAsync(user, pass);
                if (!registered.Success)
                {
                    error.text = registered.Error;
                    return;
                }

                var loggedIn = await state.Api.LoginAsync(user, pass);
                if (!loggedIn.Success || loggedIn.Value == null)
                {
                    error.text = loggedIn.Error;
                    return;
                }

                state.SignIn(loggedIn.Value.username, loggedIn.Value.token);
                state.Pets.Clear();
                Manager.Reset(ScreenId.Hud);
                return;
            }

            // shortcut: chế độ demo không cần server, dùng cho Giai đoạn 2.
            state.SignIn(user, "demo-token");
            state.Pets.Clear();
            state.SeedDemoIfEmpty();
            Manager.Reset(ScreenId.Hud);
        }
    }
}
