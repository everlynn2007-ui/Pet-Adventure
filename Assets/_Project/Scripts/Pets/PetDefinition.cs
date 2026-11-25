using UnityEngine;

namespace PetAdventure.Pets
{
    /// <summary>Dữ liệu tĩnh của một loài linh thú. Không chứa tiến trình người chơi.</summary>
    [CreateAssetMenu(menuName = "Pet Adventure/Pet Definition", fileName = "PetDefinition")]
    public sealed class PetDefinition : ScriptableObject
    {
        public string petKey = "firefox";
        public string displayName = "FireFox";
        public PetElement element = PetElement.Fire;

        [TextArea(2, 4)]
        public string description = "";

        public Sprite icon;
        public Sprite overworldSprite;

        public void Configure(string key, string name, PetElement petElement, string desc)
        {
            petKey = key;
            displayName = name;
            element = petElement;
            description = desc;
        }
    }
}
