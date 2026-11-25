using UnityEngine;

namespace PetAdventure.Gameplay
{
    public interface IInteractable
    {
        string Prompt { get; }

        void Interact();
    }

    /// <summary>Sắp thứ tự vẽ theo trục Y để nhân vật đứng trước/sau vật thể đúng.</summary>
    public sealed class SortingByY : MonoBehaviour
    {
        [SerializeField] private int baseOrder = 0;
        [SerializeField] private bool continuous = true;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Apply();
        }

        private void LateUpdate()
        {
            if (continuous) Apply();
        }

        public void Apply()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingOrder = baseOrder - Mathf.RoundToInt(transform.position.y * 100f);
        }
    }
}
