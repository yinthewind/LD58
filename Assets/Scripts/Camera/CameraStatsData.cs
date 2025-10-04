using UnityEngine;

namespace CameraSystem
{
    /// <summary>
    /// ScriptableObject for camera configuration presets.
    /// Allows creating reusable camera settings (tight follow, loose follow, cinematic, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "Camera Stats", menuName = "Camera/Camera Stats Data")]
    public class CameraStatsData : ScriptableObject
    {
        [Header("Deadzone Settings")]
        [Tooltip("Deadzone width as percentage of screen width (0-1)")]
        [Range(0.1f, 0.8f)]
        public float deadzoneWidth = 0.3f;

        [Tooltip("Deadzone height as percentage of screen height (0-1)")]
        [Range(0.1f, 0.8f)]
        public float deadzoneHeight = 0.3f;

        [Header("Follow Settings")]
        [Tooltip("How quickly camera follows target (higher = faster)")]
        [Range(1f, 20f)]
        public float smoothSpeed = 5f;

        [Tooltip("Look ahead in direction of player velocity")]
        public bool useVelocityLookahead = false;

        [Tooltip("Distance to look ahead based on player velocity")]
        [Range(0f, 10f)]
        public float lookaheadAmount = 2f;

        [Header("Preset Info")]
        [TextArea(2, 4)]
        [Tooltip("Description of this camera preset")]
        public string description = "Standard camera follow settings";

        /// <summary>
        /// Create a preset with specific settings
        /// </summary>
        public static CameraStatsData CreatePreset(string presetName, float deadzoneW, float deadzoneH, float smooth, string desc = "")
        {
            var preset = CreateInstance<CameraStatsData>();
            preset.name = presetName;
            preset.deadzoneWidth = deadzoneW;
            preset.deadzoneHeight = deadzoneH;
            preset.smoothSpeed = smooth;
            preset.description = desc;
            return preset;
        }
    }
}
