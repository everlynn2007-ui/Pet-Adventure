using System.Collections.Generic;
using PetAdventure.Core;
using PetAdventure.Pets;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class PetCollectionScreen : Screen
    {
        private readonly GameState state;
        private RectTransform content;
        private Text countLabel;

        public PetCollectionScreen(GameState state)
        {
            this.state = state;
        }

        protected override void BuildContent()
        {
            RectTransform card = CreateCard(new Vector2(1560f, 920f));

            LabelAt(card, "Bộ sưu tập linh thú", 44, UiTheme.TextDark, -26f, 900f, TextAnchor.MiddleCenter, FontStyle.Bold, 56f);
            countLabel = LabelAt(card, "", 22, UiTheme.TextMuted, -88f, 900f, TextAnchor.MiddleCenter, FontStyle.Normal, 30f);

            Button back = UiFactory.Button(card, "← Quay lại", UiTheme.Secondary, UiTheme.TextLight,
                () => Manager.Back(), 22);
            UiFactory.Place((RectTransform)back.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(28f, -24f), new Vector2(190f, 54f));

            Button catchPet = UiFactory.Button(card, "＋ Bắt thú", UiTheme.Accent, UiTheme.TextDark, CatchPet, 22);
            UiFactory.Place((RectTransform)catchPet.transform, new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-28f, -24f), new Vector2(190f, 54f));

            ScrollRect scroll = UiFactory.Scroll(card, out content);
            UiFactory.Fill((RectTransform)scroll.transform, 32f, 32f, 32f, 132f);

            GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(350f, 330f);
            grid.spacing = new Vector2(20f, 20f);
            grid.padding = new RectOffset(14, 14, 14, 14);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
            grid.childAlignment = TextAnchor.UpperCenter;

            state.Pets.Changed += OnPetsChanged;
        }

        protected override async void OnShow()
        {
            if (state.UseBackend && state.IsLoggedIn)
            {
                var result = await state.Api.GetCharactersAsync();
                if (result.Success && result.Value != null)
                {
                    List<PetRuntimeData> loaded = new List<PetRuntimeData>();
                    foreach (var dto in result.Value) loaded.Add(dto.ToRuntime());
                    state.Pets.ReplaceAll(loaded);
                }
                else
                {
                    countLabel.text = "Không tải được danh sách: " + result.Error;
                }
            }

            Rebuild();
        }

        private void OnPetsChanged()
        {
            if (Root.gameObject.activeSelf)
            {
                Rebuild();
            }
        }

        private void Rebuild()
        {
            UiFactory.ClearChildren(content);

            foreach (PetRuntimeData pet in state.Pets.Pets)
            {
                PetCardView.Build(content, pet, OpenDetails);
            }

            countLabel.text = state.Pets.Count + " linh thú • chạm để xem chi tiết";

            if (state.Pets.Count == 0)
            {
                LabelAt(content, "Chưa có linh thú nào. Bấm '＋ Bắt thú'.", 24, UiTheme.TextMuted, -20f, 600f);
            }
        }

        private async void CatchPet()
        {
            PetElement element = PetElementInfo.All[Random.Range(0, PetElementInfo.All.Length)];
            PetDefinition definition = PetCatalog.ForElement(element);
            if (definition == null) return;

            if (state.UseBackend && state.IsLoggedIn)
            {
                countLabel.text = "Đang bắt linh thú...";
                var result = await state.Api.CreateCharacterAsync(definition.displayName, element.Code());
                if (!result.Success || result.Value == null)
                {
                    countLabel.text = "Lỗi: " + result.Error;
                    return;
                }

                state.Pets.AddOrReplace(result.Value.ToRuntime());
            }
            else
            {
                state.Pets.Add(definition);
            }

            PetRuntimeData last = state.Pets.Pets[state.Pets.Count - 1];
            state.SelectedPetId = last.id;
            countLabel.text = "Đã bắt " + last.name + " (" + element.DisplayName() + ")";
        }

        private void OpenDetails(PetRuntimeData pet)
        {
            state.SelectedPetId = pet.id;
            Manager.Navigate(ScreenId.PetDetails);
        }
    }
}
