using UnityEngine;

namespace CameraSystem
{
    /// <summary>
    /// Deadzone-based camera follow system that keeps the player centered
    /// but only moves when the player approaches screen edges.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class FollowCamera : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("The transform to follow (usually the player)")]
        [SerializeField] private Transform target;

        [Header("Camera Settings")]
        [Tooltip("Use a ScriptableObject preset for camera settings")]
        [SerializeField] private CameraStatsData cameraStats;

        [Header("Manual Override (if no preset)")]
        [Tooltip("Deadzone width as percentage of screen width (0-1)")]
        [SerializeField] private float deadzoneWidth = 0.3f;
        [Tooltip("Deadzone height as percentage of screen height (0-1)")]
        [SerializeField] private float deadzoneHeight = 0.3f;
        [Tooltip("How quickly camera follows target (higher = faster)")]
        [SerializeField] private float smoothSpeed = 5f;
        [Tooltip("Look ahead in direction of player velocity")]
        [SerializeField] private bool useVelocityLookahead = false;
        [Tooltip("Distance to look ahead based on player velocity")]
        [SerializeField] private float lookaheadAmount = 2f;

        [Header("World Bounds (Optional)")]
        [Tooltip("Clamp camera position to world boundaries")]
        [SerializeField] private bool useBounds = false;
        [Tooltip("Minimum world position for camera")]
        [SerializeField] private Vector2 minBounds = new Vector2(-100, -100);
        [Tooltip("Maximum world position for camera")]
        [SerializeField] private Vector2 maxBounds = new Vector2(100, 100);

        [Header("Debug")]
        [Tooltip("Draw deadzone gizmos in Scene view")]
        [SerializeField] private bool showDebugGizmos = true;

        private Camera cam;
        private Vector3 targetVelocity = Vector3.zero;
        private Vector3 lastTargetPosition;

        // Properties for accessing current settings (from preset or manual)
        private float DeadzoneWidth => cameraStats != null ? cameraStats.deadzoneWidth : deadzoneWidth;
        private float DeadzoneHeight => cameraStats != null ? cameraStats.deadzoneHeight : deadzoneHeight;
        private float SmoothSpeed => cameraStats != null ? cameraStats.smoothSpeed : smoothSpeed;
        private bool UseVelocityLookahead => cameraStats != null ? cameraStats.useVelocityLookahead : useVelocityLookahead;
        private float LookaheadAmount => cameraStats != null ? cameraStats.lookaheadAmount : lookaheadAmount;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Start()
        {
            if (target != null)
            {
                lastTargetPosition = target.position;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Calculate target velocity for lookahead
            targetVelocity = (target.position - lastTargetPosition) / Time.deltaTime;
            lastTargetPosition = target.position;

            // Get current camera position
            Vector3 currentCameraPos = transform.position;
            Vector3 targetWorldPos = target.position;

            // Apply velocity-based lookahead if enabled
            if (UseVelocityLookahead)
            {
                targetWorldPos += (Vector3)(Vector2)targetVelocity.normalized * LookaheadAmount;
            }

            // Convert target world position to viewport coordinates
            Vector3 viewportPos = cam.WorldToViewportPoint(targetWorldPos);

            // Calculate deadzone boundaries in viewport space (0-1)
            float deadzoneLeft = 0.5f - DeadzoneWidth / 2f;
            float deadzoneRight = 0.5f + DeadzoneWidth / 2f;
            float deadzoneBottom = 0.5f - DeadzoneHeight / 2f;
            float deadzoneTop = 0.5f + DeadzoneHeight / 2f;

            // Calculate how far outside the deadzone the target is
            Vector3 deltaViewport = Vector3.zero;

            if (viewportPos.x < deadzoneLeft)
            {
                deltaViewport.x = viewportPos.x - deadzoneLeft;
            }
            else if (viewportPos.x > deadzoneRight)
            {
                deltaViewport.x = viewportPos.x - deadzoneRight;
            }

            if (viewportPos.y < deadzoneBottom)
            {
                deltaViewport.y = viewportPos.y - deadzoneBottom;
            }
            else if (viewportPos.y > deadzoneTop)
            {
                deltaViewport.y = viewportPos.y - deadzoneTop;
            }

            // Convert viewport delta to world space delta
            Vector3 deltaWorld = cam.ViewportToWorldPoint(deltaViewport) - cam.ViewportToWorldPoint(Vector3.zero);
            deltaWorld.z = 0; // Keep camera Z unchanged

            // Calculate target camera position
            Vector3 targetCameraPos = currentCameraPos + deltaWorld;

            // Smoothly move camera to target position
            Vector3 smoothedPosition = Vector3.Lerp(currentCameraPos, targetCameraPos, SmoothSpeed * Time.deltaTime);

            // Apply world bounds if enabled
            if (useBounds)
            {
                // Calculate camera viewport bounds in world space
                float cameraHeight = cam.orthographicSize * 2;
                float cameraWidth = cameraHeight * cam.aspect;

                float minX = minBounds.x + cameraWidth / 2;
                float maxX = maxBounds.x - cameraWidth / 2;
                float minY = minBounds.y + cameraHeight / 2;
                float maxY = maxBounds.y - cameraHeight / 2;

                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
            }

            // Keep original Z position
            smoothedPosition.z = transform.position.z;
            transform.position = smoothedPosition;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmos || cam == null || target == null) return;

            // Draw deadzone rectangle
            float cameraHeight = cam.orthographicSize * 2;
            float cameraWidth = cameraHeight * cam.aspect;

            float deadzoneWorldWidth = cameraWidth * DeadzoneWidth;
            float deadzoneWorldHeight = cameraHeight * DeadzoneHeight;

            Vector3 deadzoneCenter = transform.position;
            deadzoneCenter.z = 0;

            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireCube(deadzoneCenter, new Vector3(deadzoneWorldWidth, deadzoneWorldHeight, 0.1f));

            // Draw camera bounds if enabled
            if (useBounds)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Vector3 boundsCenter = new Vector3(
                    (minBounds.x + maxBounds.x) / 2,
                    (minBounds.y + maxBounds.y) / 2,
                    0
                );
                Vector3 boundsSize = new Vector3(
                    maxBounds.x - minBounds.x,
                    maxBounds.y - minBounds.y,
                    0.1f
                );
                Gizmos.DrawWireCube(boundsCenter, boundsSize);
            }

            // Draw player position indicator
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target.position, 0.5f);
        }

        // Public API for runtime control
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                lastTargetPosition = target.position;
            }
        }

        public void SetCameraStats(CameraStatsData stats)
        {
            cameraStats = stats;
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            useBounds = true;
        }

        public void DisableBounds()
        {
            useBounds = false;
        }
    }
}
