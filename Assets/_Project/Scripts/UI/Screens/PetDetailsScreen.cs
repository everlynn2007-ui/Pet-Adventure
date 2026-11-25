using PetAdventure.Core;
using PetAdventure.Pets;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class PetDetailsScreen : Screen
    {
        private readonly GameState state;

        private Image portrait;
        private Text nameLabel;
        private Text elementLabel;
        private Text levelLabel;
        private Text description;
        private Text status;
        private InputField renameField;
        private InputField levelField;
        private InputField hpField;
        private StatBar hpBar;
        private Button companionButton;
        private PetRuntimeData current;

        public PetDetailsScreen(GameState state)
        {
            this.state = state;
        }

        protected override void BuildContent()
        {
            RectTransform card = CreateCard(new Vector2(1200f, 780f));

            LabelAt(card, "Chi tiết linh thú", 40, UiTheme.TextDark, -22f, 700f, TextAnchor.MiddleCenter, FontStyle.Bold, 52f);

            Image frame = UiFactory.Panel(card, "PortraitFrame", 20, UiTheme.Cream, UiTheme.Border);
            UiFactory.Place(frame.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(56f, -120f), new Vector2(320f, 320f));

            portrait = UiFactory.Icon(frame.transform, SpriteFactory.PetIcon(PetElement.Fire), 280f);
            UiFactory.Center(portrait.rectTransform, new Vector2(280f, 280f));

            nameLabel = PlaceLeft(card, "", 40, UiTheme.TextDark, -110f, 46f, FontStyle.Bold);
            elementLabel = PlaceLeft(card, "", 24, UiTheme.TextMuted, -164f, 32f, FontStyle.Normal);
            levelLabel = PlaceLeft(card, "", 22, UiTheme.TextMuted, -204f, 30f, FontStyle.Normal);

            hpBar = StatBar.Create(card, UiTheme.HpGreen, 24f, true);
            UiFactory.Place(hpBar.Root, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(410f, -238f), new Vector2(600f, 24f));

            description = UiFactory.Label(card, "", 22, UiTheme.TextMuted, TextAnchor.UpperLeft);
            UiFactory.Place(description.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(410f, -292f), new Vector2(720f, 110f));

            Text renameCaption = UiFactory.Label(card, "Đổi tên", 20, UiTheme.TextDark, TextAnchor.MiddleLeft, FontStyle.Bold);
            UiFactory.Place(renameCaption.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(410f, -420f), new Vector2(400f, 28f));

            renameField = UiFactory.Input(card, "Tên mới (2–50 ký tự)", false, 22);
            UiFactory.Place((RectTransform)renameField.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(410f, -452f), new Vector2(420f, 58f));

            Button saveName = UiFactory.Button(card, "Lưu tên", UiTheme.Primary, UiTheme.TextLight, Rename, 22);
            UiFactory.Place((RectTransform)saveName.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(846f, -452f), new Vector2(170f, 58f));

            levelField = UiFactory.Input(card, "Cấp", false, 22);
            UiFactory.Place((RectTransform)levelField.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(410f, -512f), new Vector2(170f, 54f));

            hpField = UiFactory.Input(card, "HP", false, 22);
            UiFactory.Place((RectTransform)hpField.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(596f, -512f), new Vector2(170f, 54f));

            Button saveStats = UiFactory.Button(card, "Lưu chỉ số", UiTheme.Secondary, UiTheme.TextLight, SaveStats, 20);
            UiFactory.Place((RectTransform)saveStats.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(782f, -512f), new Vector2(234f, 54f));

            companionButton = UiFactory.Button(card, "Đi theo", UiTheme.Accent, UiTheme.TextDark, SetCompanion, 24);
            UiFactory.Place((RectTransform)companionButton.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(410f, -582f), new Vector2(280f, 64f));

            Button back = UiFactory.Button(card, "← Quay lại", UiTheme.PanelAlt, UiTheme.TextDark, () => Manager.Back(), 24);
            UiFactory.Place((RectTransform)back.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(710f, -582f), new Vector2(210f, 64f));

            status = UiFactory.Label(card, "", 20, UiTheme.PrimaryDark, TextAnchor.MiddleLeft, FontStyle.Bold);
            UiFactory.Place(status.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(410f, -660f), new Vector2(720f, 34f));
        }

        private static Text PlaceLeft(RectTransform card, string text, int fontSize, Color color,
            float y, float height, FontStyle style)
        {
            Text label = UiFactory.Label(card, text, fontSize, color, TextAnchor.MiddleLeft, style);
            UiFactory.Place(label.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(410f, y), new Vector2(720f, height));
            return label;
        }

        protected override void OnShow()
        {
            status.text = "";
            current = state.Pets.Get(state.SelectedPetId);
            if (current == null)
            {
                Manager.Back();
                return;
            }

            Refresh();
        }

        private void Refresh()
        {
            portrait.sprite = PetCatalog.IconFor(current.element);
            nameLabel.text = current.name;
            elementLabel.text = current.element.DisplayName();
            elementLabel.color = UiTheme.ElementColor(current.element).Darken(0.2f);
            levelLabel.text = "Cấp " + current.level;
            description.text = current.description;
            hpBar.Set(current.hp, current.MaxHp);
            renameField.text = current.name;
            levelField.text = current.level.ToString();
            hpField.text = current.hp.ToString();

            bool isCompanion = state.CompanionPetId == current.id;
            Text companionText = companionButton.GetComponentInChildren<Text>();
            if (companionText != null)
            {
                companionText.text = isCompanion ? "Đang đi theo" : "Đi theo";
            }
        }

        private async void Rename()
        {
            if (current == null) return;

            string newName = (renameField.text ?? "").Trim();
            if (newName.Length < 2 || newName.Length > 50)
            {
                status.color = UiTheme.Danger;
                status.text = "Tên phải từ 2 đến 50 ký tự";
                return;
            }

            if (state.UseBackend && state.IsLoggedIn)
            {
                var result = await state.Api.UpdateCharacterAsync(current.id, newName, current.element.Code(),
                    current.level, current.hp);
                if (!result.Success || result.Value == null)
                {
                    status.color = UiTheme.Danger;
                    status.text = result.Error;
                    return;
                }

                Apply(result.Value);
            }
            else if (!state.Pets.Rename(current.id, newName))
            {
                status.color = UiTheme.Danger;
                status.text = "Tên phải từ 2 đến 50 ký tự";
                return;
            }

            status.color = UiTheme.PrimaryDark;
            status.text = "Đã đổi tên thành " + current.name;
            state.Pets.RaiseChanged();
            Refresh();
        }

        private async void SaveStats()
        {
            if (current == null) return;

            if (!int.TryParse(levelField.text, out int level) || !int.TryParse(hpField.text, out int hp))
            {
                status.color = UiTheme.Danger;
                status.text = "Cấp và HP phải là số";
                return;
            }

            if (level < PetStats.MinLevel || level > PetStats.MaxLevel || hp < PetStats.MinHp || hp > PetStats.MaxHpLimit)
            {
                status.color = UiTheme.Danger;
                status.text = "Cấp 1–100, HP 0–100000";
                return;
            }

            if (state.UseBackend && state.IsLoggedIn)
            {
                var result = await state.Api.UpdateCharacterAsync(current.id, current.name, current.element.Code(), level, hp);
                if (!result.Success || result.Value == null)
                {
                    status.color = UiTheme.Danger;
                    status.text = result.Error;
                    return;
                }

                Apply(result.Value);
            }
            else
            {
                state.Pets.UpdateStats(current.id, level, hp);
            }

            status.color = UiTheme.PrimaryDark;
            status.text = "Đã lưu: cấp " + current.level + ", HP " + current.hp;
            state.Pets.RaiseChanged();
            Refresh();
        }

        private void Apply(PetAdventure.Networking.CharacterResponseDto dto)
        {
            current.name = dto.name;
            current.element = PetElementInfo.Parse(dto.@class);
            current.level = dto.level;
            current.hp = dto.hp;
            current.accountId = dto.accountId;
        }

        private void SetCompanion()
        {
            if (current == null) return;

            state.CompanionPetId = current.id;
            GameBootstrap.Instance?.Companion?.SetElement(current.element);
            status.color = UiTheme.PrimaryDark;
            status.text = current.name + " sẽ đi theo bạn trong làng";
            Refresh();
        }
    }
}
