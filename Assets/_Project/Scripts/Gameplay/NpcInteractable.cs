using PetAdventure.Core;
using UnityEngine;

namespace PetAdventure.Gameplay
{
    public sealed class NpcInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string npcName = "Trưởng làng";
        [SerializeField] private string[] lines =
        {
            "Chào mừng đến Greenleaf Village!",
            "Linh thú của bạn sẽ đi theo nếu bạn chọn trong Bộ sưu tập."
        };

        private int index;

        public string Prompt => "Nhấn E để nói chuyện với " + npcName;

        public void Configure(string speaker, string[] dialogue)
        {
            npcName = speaker;
            lines = dialogue;
        }

        public void Interact()
        {
            if (lines == null || lines.Length == 0) return;

            string line = lines[index % lines.Length];
            index++;
            GameBootstrap.Instance?.ShowDialogue(npcName, line);
        }
    }
}
