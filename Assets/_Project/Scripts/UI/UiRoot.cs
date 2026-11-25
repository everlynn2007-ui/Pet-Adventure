using PetAdventure.Core;
using PetAdventure.UI.Screens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace PetAdventure.UI
{
    /// <summary>Dựng Canvas, EventSystem và toàn bộ màn hình. Tách khỏi gameplay.</summary>
    public sealed class UiRoot
    {
        public Canvas Canvas { get; private set; }

        public ScreenManager Manager { get; private set; }

        public void Build(GameState state, Transform parent)
        {
            GameObject canvasObject = new GameObject("UI Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(parent, false);

            Canvas = canvasObject.GetComponent<Canvas>();
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            EnsureEventSystem(parent);

            Manager = new ScreenManager(state);
            Manager.Register(ScreenId.MainMenu, new MainMenuScreen(state));
            Manager.Register(ScreenId.Login, new LoginScreen(state));
            Manager.Register(ScreenId.Register, new RegisterScreen(state));
            Manager.Register(ScreenId.Hud, new HudScreen(state));
            Manager.Register(ScreenId.PetCollection, new PetCollectionScreen(state));
            Manager.Register(ScreenId.PetDetails, new PetDetailsScreen(state));
            Manager.Register(ScreenId.Inventory, new InventoryScreen());
            Manager.Register(ScreenId.Settings, new SettingsScreen(state));
            Manager.Register(ScreenId.Pause, new PauseScreen());

            Manager.BuildAll(Canvas.transform);
            Manager.Reset(ScreenId.MainMenu);
        }

        public void ShowInteractionPrompt(string text)
        {
            HudScreen hud = Manager.Get<HudScreen>(ScreenId.Hud);
            if (hud != null)
            {
                hud.SetInteractionPrompt(text);
            }
        }

        private static void EnsureEventSystem(Transform parent)
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.transform.SetParent(parent, false);
            InputSystemUIInputModule module = eventSystem.GetComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }
    }
}
