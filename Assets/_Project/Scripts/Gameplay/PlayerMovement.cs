using UnityEngine;
using UnityEngine.InputSystem;

namespace PetAdventure.Gameplay
{
    /// <summary>Di chuyển top-down bằng WASD / phím mũi tên.</summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.5f;

        private Rigidbody2D body;
        private Vector2 input;
        private Vector2 facing = Vector2.down;

        public Vector2 Facing => facing;

        public bool IsMoving => input.sqrMagnitude > 0.01f;

        public bool InputEnabled { get; set; } = true;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Update()
        {
            if (!InputEnabled || Keyboard.current == null)
            {
                input = Vector2.zero;
                return;
            }

            Vector2 next = Vector2.zero;
            Keyboard keyboard = Keyboard.current;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) next.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) next.y -= 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) next.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) next.x += 1f;

            input = next.sqrMagnitude > 1f ? next.normalized : next;
            if (input.sqrMagnitude > 0.01f)
            {
                facing = input.normalized;
            }
        }

        private void FixedUpdate()
        {
            body.linearVelocity = input * moveSpeed;
        }
    }
}
