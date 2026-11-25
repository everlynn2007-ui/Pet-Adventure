using UnityEngine;

namespace PetAdventure.Gameplay
{
    /// <summary>Camera bám người chơi, giới hạn trong biên bản đồ.</summary>
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private Vector2 offset = new Vector2(0f, 0.4f);
        [SerializeField] private Vector2 min = new Vector2(-16f, -8.25f);
        [SerializeField] private Vector2 max = new Vector2(16f, 8.25f);

        private Transform target;
        private Vector3 velocity;

        public void SetTarget(Transform follow, Vector2 boundsMin, Vector2 boundsMax)
        {
            target = follow;
            min = boundsMin;
            max = boundsMax;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = new Vector3(
                Mathf.Clamp(target.position.x + offset.x, min.x, max.x),
                Mathf.Clamp(target.position.y + offset.y, min.y, max.y),
                -10f);

            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }
    }
}
