using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerJumpParameters", menuName = "ScriptableObjects/Player/Default/Jump")]
    public class PlayerControllerJumpScriptableObject : ScriptableObject
    {
        [Header("Jump Power")]
        [Range(0f, 10f)]
        public float jumpPower;

        [Header("Jump Cooldown Time")]
        [Range(0f, 2f)]
        public float jumpCooldownTime = 0.2f;

        [Header("Coyote Jump Time")]
        [Range(0f, 1f)]
        public float coyoteJumpTime = 0.1f;

        [Header("Floating Capsule Suspension Time")]
        [Range(0f, 1f)]
        public float floatingCapsuleSuspensionTime = 0.05f;

        [Header("Gravity boost after jump")]
        [Range(0f, 10f)]
        public float gravityBoostAfterJump = 0.05f;
        [Range(0f, 10f)]
        public float gravityBoostAfterJumpTime = 0.05f;

        [Header("Failed jump case reset time")]
        public float failedJumpCaseResetTimeInSeconds = 0.08f;
    }
}