using NOS.GameManagers.Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerDefaultStaminaParameters", menuName = "ScriptableObjects/Player/Default/Stamina")]
    public class PlayerControllerDefaultStaminaScriptableObject : ScriptableObject
    {
        [Header("Max Stamina")]
        public float maximalStamina = 100;

        [Header("Stamina Costs")]
        public float staminaCostPerSecondRunning = 10f;
        public float staminaCostPerJump = 20f;

        [Header("Regeneration")]
        public float staminaRegenerationBase = 15f;
        public float staminaRegenerationAfterFullyDepleting = 5f;

        [Header("Overlay")]
        public float staminaOverlayFadingInStartValue = 40;
        public float staminaOverlaySmoothTransitionTime = 0.2f;
        
        [Header("Depletion Recovery")]
        public float staminaRecoveryValueAfterDepletion = 30f;

        [Header("Sound Settings")]
        public SoundInstanceReference soundReference;
        public Vector3 soundLocalOffsetFromParent;



    }
}
