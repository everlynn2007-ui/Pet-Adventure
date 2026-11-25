using PetAdventure.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetAdventure.Gameplay
{
    /// <summary>Tìm vật thể gần nhất có thể tương tác và xử lý phím E.</summary>
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float radius = 1.5f;
        [SerializeField] private Vector2 offset = new Vector2(0f, 0.4f);

        private readonly Collider2D[] hits = new Collider2D[16];
        private IInteractable current;

        private void Update()
        {
            IInteractable nearest = FindNearest();
            if (!ReferenceEquals(nearest, current))
            {
                current = nearest;
                GameBootstrap.Instance?.Ui.ShowInteractionPrompt(current != null ? current.Prompt : "");
            }

            if (current != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                current.Interact();
            }
        }

        private void OnDisable()
        {
            GameBootstrap.Instance?.Ui.ShowInteractionPrompt("");
        }

        private IInteractable FindNearest()
        {
            Vector2 origin = (Vector2)transform.position + offset;
            int count = Physics2D.OverlapCircleNonAlloc(origin, radius, hits);

            IInteractable nearest = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = hits[i];
                if (hit == null) continue;

                IInteractable candidate = hit.GetComponentInParent<IInteractable>();
                if (candidate == null) continue;

                float distance = Vector2.SqrMagnitude((Vector2)hit.transform.position - origin);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = candidate;
                }
            }

            return nearest;
        }
    }
}
