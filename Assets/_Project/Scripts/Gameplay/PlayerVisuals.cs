using UnityEngine;

namespace PetAdventure.Gameplay
{
    /// <summary>
    /// Hiệu ứng placeholder cho sprite con của người chơi: lật hướng và nhún nhẹ khi đi.
    /// Gắn lên object con "Visual" (có SpriteRenderer), không gắn lên object có Rigidbody2D.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private float bobAmplitude = 0.06f;
        [SerializeField] private float bobSpeed = 10f;

        private SpriteRenderer spriteRenderer;
        private PlayerMovement movement;
        private Vector3 basePosition;
        private float phase;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            movement = GetComponentInParent<PlayerMovement>();
            basePosition = transform.localPosition;
        }

        private void LateUpdate()
        {
            if (movement == null || spriteRenderer == null) return;

            if (Mathf.Abs(movement.Facing.x) > 0.1f)
            {
                spriteRenderer.flipX = movement.Facing.x < 0f;
            }

            bool moving = movement.IsMoving;
            phase = moving ? phase + Time.deltaTime * bobSpeed : 0f;
            float offset = moving ? Mathf.Abs(Mathf.Sin(phase)) * bobAmplitude : 0f;
            transform.localPosition = basePosition + new Vector3(0f, offset, 0f);
        }
    }
}
