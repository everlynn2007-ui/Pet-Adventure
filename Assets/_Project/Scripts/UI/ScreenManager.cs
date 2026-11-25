using System.Collections.Generic;
using PetAdventure.Core;
using UnityEngine;

namespace PetAdventure.UI
{
    public enum ScreenId
    {
        MainMenu = 0,
        Login = 1,
        Register = 2,
        Hud = 3,
        PetCollection = 4,
        PetDetails = 5,
        Inventory = 6,
        Settings = 7,
        Pause = 8
    }

    /// <summary>Điều hướng giữa các màn hình, có lịch sử để quay lại.</summary>
    public sealed class ScreenManager
    {
        private readonly List<KeyValuePair<ScreenId, Screen>> ordered = new List<KeyValuePair<ScreenId, Screen>>();
        private readonly Dictionary<ScreenId, Screen> lookup = new Dictionary<ScreenId, Screen>();
        private readonly List<ScreenId> history = new List<ScreenId>();

        public GameState State { get; }

        public ScreenId Current { get; private set; }

        public ScreenManager(GameState state)
        {
            State = state;
        }

        public void Register(ScreenId id, Screen screen)
        {
            ordered.Add(new KeyValuePair<ScreenId, Screen>(id, screen));
            lookup[id] = screen;
        }

        public void BuildAll(Transform parent)
        {
            foreach (KeyValuePair<ScreenId, Screen> pair in ordered)
            {
                pair.Value.Build(parent, this);
            }
        }

        public void Reset(ScreenId id)
        {
            history.Clear();
            SwitchTo(id);
        }

        public void Navigate(ScreenId id)
        {
            history.Add(Current);
            SwitchTo(id);
        }

        public void Back()
        {
            if (history.Count == 0)
            {
                return;
            }

            int last = history.Count - 1;
            ScreenId previous = history[last];
            history.RemoveAt(last);
            SwitchTo(previous);
        }

        public bool CanGoBack => history.Count > 0;

        public int ScreenCount => ordered.Count;

        public bool Has(ScreenId id) => lookup.ContainsKey(id);

        public T Get<T>(ScreenId id) where T : Screen
        {
            return lookup.TryGetValue(id, out Screen screen) ? screen as T : null;
        }

        private void SwitchTo(ScreenId id)
        {
            if (!lookup.ContainsKey(id))
            {
                Debug.LogError($"Screen chưa đăng ký: {id}");
                return;
            }

            if (lookup.TryGetValue(Current, out Screen current) && current != null && Current != id)
            {
                current.Hide();
            }

            Current = id;
            lookup[id].Show();
        }
    }
}
