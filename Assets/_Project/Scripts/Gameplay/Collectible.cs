using PetAdventure.Core;
using UnityEngine;

namespace PetAdventure.Gameplay
{
    public sealed class Collectible : MonoBehaviour, IInteractable
    {
        [SerializeField] private string itemName = "Hoa may mắn";

        public string Prompt => "Nhấn E để nhặt " + itemName;

        public void Interact()
        {
            GameState state = GameBootstrap.Instance?.State;
            if (state != null) state.CollectedItems++;

            GameBootstrap.Instance?.ShowDialogue("Túi đồ",
                "Đã nhặt " + itemName + ". Tổng cộng: " + (state != null ? state.CollectedItems : 0));

            Destroy(gameObject);
        }
    }
}
