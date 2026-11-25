using PetAdventure.Core;
using PetAdventure.Pets;
using UnityEngine;

namespace PetAdventure.Gameplay
{
    /// <summary>Linh thú đi theo người chơi, giữ khoảng cách và không đẩy người chơi.</summary>
    public sealed class PetCompanion : MonoBehaviour
    {
        [SerializeField] private float followDistance = 1.3f;
        [SerializeField] private float maxSpeed = 6f;

        private Transform target;
        private SpriteRenderer spriteRenderer;

        public void Initialize(Transform followTarget, PetElement element)
        {
            target = followTarget;
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            SetElement(element);
        }

        public void SetElement(PetElement element)
        {
            if (spriteRenderer == null) return;

            PetDefinition definition = PetCatalog.ForElement(element);
            spriteRenderer.sprite = definition != null && definition.overworldSprite != null
                ? definition.overworldSprite
                : SpriteFactory.PetWorld(element);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 delta = target.position - transform.position;
            float distance = delta.magnitude;
            if (distance > followDistance)
            {
                float step = maxSpeed * Time.deltaTime * Mathf.Clamp01(distance - followDistance + 0.4f);
                transform.position += delta.normalized * step;
            }

            if (spriteRenderer != null && Mathf.Abs(delta.x) > 0.05f)
            {
                spriteRenderer.flipX = delta.x < 0f;
            }
        }
    }
}
