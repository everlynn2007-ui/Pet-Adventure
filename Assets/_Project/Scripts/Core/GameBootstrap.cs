using System;
using System.Collections;
using PetAdventure.Gameplay;
using PetAdventure.UI;
using PetAdventure.UI.Screens;
using UnityEngine;

namespace PetAdventure.Core
{
    /// <summary>Điểm vào của game. Gắn trên một GameObject trong scene.</summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        public GameState State { get; private set; }

        public UiRoot Ui { get; private set; }

        public PlayerMovement Player { get; private set; }

        public PetCompanion Companion { get; private set; }

        private void Awake()
        {
            Instance = this;
            Initialize();
        }

        public void Initialize()
        {
            if (State != null)
            {
                return;
            }

            Application.targetFrameRate = 60;
            State = new GameState();
            State.SeedDemoIfEmpty();

            Ui = new UiRoot();
            Ui.Build(State, transform);

            WorldBuilder.BuildWorld(transform, State);
            GameObject playerObject = WorldBuilder.CreatePlayer(transform);
            Player = playerObject.GetComponent<PlayerMovement>();
            Companion = WorldBuilder.CreateCompanion(transform, State, playerObject.transform);
            CreateCamera(playerObject.transform);

            AudioListener.volume = State.MasterVolume;
            UnityEngine.Screen.fullScreen = State.Fullscreen;
        }

        public void ShowDialogue(string speaker, string text)
        {
            Ui.Manager.Get<HudScreen>(ScreenId.Hud)?.ShowDialogue(speaker, text);
        }

        public Coroutine RunDelayed(float seconds, Action action)
        {
            return StartCoroutine(Delayed(seconds, action));
        }

        private static IEnumerator Delayed(float seconds, Action action)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action?.Invoke();
        }

        private static void CreateCamera(Transform follow)
        {
            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(follow.position.x, follow.position.y, -10f);

            Camera camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6.75f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = UiTheme.Hex("9BD7E8");

            CameraFollow followComponent = cameraObject.AddComponent<CameraFollow>();
            followComponent.SetTarget(follow, WorldBuilder.BoundsMin, WorldBuilder.BoundsMax);
        }
    }
}
