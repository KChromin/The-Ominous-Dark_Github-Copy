using NOS.GameManagers.Audio;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerFootstepsScriptableObject", menuName = "ScriptableObjects/Player/Default/Footsteps")]
    public class PlayerControllerFootstepsScriptableObject : ScriptableObject
    {
        [Header("Sound Settings")]
        public SoundInstanceReference soundReference;

        [Header("Landing Minimum Force")]
        public float landingSoundMinimumForce = -5;
    }

    public enum FootstepsTypes
    {
        WalkBasic,
        RunBasic,
        LandBasic,
        CrouchWalkBasic
    }
}