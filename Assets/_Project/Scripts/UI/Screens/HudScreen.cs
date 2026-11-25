using PetAdventure.Core;
using PetAdventure.Pets;
using UnityEngine;
using UnityEngine.UI;

namespace PetAdventure.UI.Screens
{
    public sealed class HudScreen : Screen
    {
        private readonly GameState state;

        private Text playerName;
        private Text petName;
        private Text petMeta;
        private Image petIcon;
        private Text prompt;
        private StatBar hpBar;
        private GameObject dialoguePanel;
        private Text dialogueSpeaker;
        private Text dialogueText;
        private Coroutine dialogueRoutine;

        public HudScreen(GameState state)
        {
            this.state = state;
        }

        protected override void BuildContent()
        {
            Image card = UiFactory.Panel(Root, "PlayerCard", 18);
            UiFactory.Place(card.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(24f, -24f), new Vector2(440f, 172f));

            playerName = UiFactory.Label(card.transform, "", 28, UiTheme.TextDark, TextAnchor.MiddleLeft, FontStyle.Bold);
            UiFactory.Place(playerName.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(20f, -12f), new Vector2(400f, 34f));

            Text role = UiFactory.Label(card.transform, "Nhà huấn luyện", 18, UiTheme.TextMuted, TextAnchor.MiddleLeft, FontStyle.Italic);
            UiFactory.Place(role.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(20f, -46f), new Vector2(400f, 24f));

            petIcon = UiFactory.Icon(card.transform, SpriteFactory.PetIcon(PetElement.Fire), 56f);
            UiFactory.Place(petIcon.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(20f, -86f), new Vector2(56f, 56f));

            petName = UiFactory.Label(card.transform, "", 22, UiTheme.TextDark, TextAnchor.MiddleLeft, FontStyle.Bold);
            UiFactory.Place(petName.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(86f, -82f), new Vector2(330f, 28f));

            petMeta = UiFactory.Label(card.transform, "", 16, UiTheme.TextMuted, TextAnchor.MiddleLeft);
            UiFactory.Place(petMeta.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(86f, -110f), new Vector2(330f, 22f));

            hpBar = StatBar.Create(card.transform, UiTheme.HpGreen, 18f, true);
            UiFactory.Place(hpBar.Root, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(86f, -140f), new Vector2(300f, 18f));

            RectTransform actions = UiFactory.Node("Actions", Root);
            UiFactory.Place(actions, new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(-24f, 24f), new Vector2(600f, 64f));
            HorizontalLayoutGroup row = actions.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 12f;
            row.childAlignment = TextAnchor.MiddleRight;
            row.childControlWidth = false;
            row.childControlHeight = false;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = false;

            ActionButton(actions, "Linh thú", UiTheme.Secondary, UiTheme.TextLight, () => Manager.Navigate(ScreenId.PetCollection));
            ActionButton(actions, "Túi đồ", UiTheme.Accent, UiTheme.TextDark, () => Manager.Navigate(ScreenId.Inventory));
            ActionButton(actions, "Tạm dừng", UiTheme.Gold, UiTheme.TextDark, () => Manager.Navigate(ScreenId.Pause));

            Image promptPanel = UiFactory.Panel(Root, "Prompt", 12, UiTheme.Panel, UiTheme.Border);
            UiFactory.Place(promptPanel.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 268f), new Vector2(660f, 58f));
            prompt = UiFactory.Label(promptPanel.transform, "", 24, UiTheme.TextDark, TextAnchor.MiddleCenter);
            UiFactory.Fill(prompt.rectTransform, 12, 4, 12, 4);
            promptPanel.gameObject.SetActive(false);

            Image dialogue = UiFactory.Panel(Root, "Dialogue", 16, UiTheme.Panel, UiTheme.Border);
            UiFactory.Place(dialogue.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 96f), new Vector2(880f, 150f));
            dialoguePanel = dialogue.gameObject;

            dialogueSpeaker = UiFactory.Label(dialogue.transform, "", 24, UiTheme.PrimaryDark, TextAnchor.MiddleLeft, FontStyle.Bold);
            UiFactory.Place(dialogueSpeaker.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(28f, -16f), new Vector2(820f, 30f));

            dialogueText = UiFactory.Label(dialogue.transform, "", 24, UiTheme.TextDark, TextAnchor.UpperLeft);
            UiFactory.Place(dialogueText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(28f, -54f), new Vector2(820f, 82f));

            dialoguePanel.SetActive(false);

            state.Pets.Changed += Refresh;
        }

        private void ActionButton(RectTransform parent, string caption, Color fill, Color textColor,
            UnityEngine.Events.UnityAction action)
        {
            Button button = UiFactory.Button(parent, caption, fill, textColor, null, 26);
            LayoutElement element = button.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = 186f;
            element.preferredHeight = 62f;
            button.onClick.AddListener(action);
        }

        protected override void OnShow()
        {
            Time.timeScale = 1f;
            Refresh();
        }

        public void SetInteractionPrompt(string text)
        {
            bool visible = !string.IsNullOrEmpty(text) && Root.gameObject.activeSelf;
            prompt.transform.parent.gameObject.SetActive(visible);
            prompt.text = text ?? "";
        }

        public void ShowDialogue(string speaker, string text, float seconds = 4f)
        {
            dialogueSpeaker.text = speaker;
            dialogueText.text = text;
            dialoguePanel.SetActive(true);

            GameBootstrap bootstrap = GameBootstrap.Instance;
            if (bootstrap == null) return;

            if (dialogueRoutine != null)
            {
                bootstrap.StopCoroutine(dialogueRoutine);
            }

            dialogueRoutine = bootstrap.RunDelayed(seconds, () => dialoguePanel.SetActive(false));
        }

        private void Refresh()
        {
            playerName.text = string.IsNullOrEmpty(state.Username) ? "Khách" : state.Username;

            PetRuntimeData companion = state.Companion;
            if (companion == null)
            {
                petIcon.color = Color.white.WithAlpha(0.35f);
                petName.text = "Chưa chọn linh thú";
                petMeta.text = "Mở Bộ sưu tập để chọn";
                hpBar.Root.gameObject.SetActive(false);
                return;
            }

            petIcon.sprite = PetCatalog.IconFor(companion.element);
            petIcon.color = Color.white;
            petName.text = companion.name;
            petMeta.text = companion.element.DisplayName() + " • Cấp " + companion.level;
            hpBar.Root.gameObject.SetActive(true);
            hpBar.Set(companion.hp, companion.MaxHp);
        }
    }
}
