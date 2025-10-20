using System;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerCrouchParameters", menuName = "ScriptableObjects/Player/Default/Crouch")]
    public class PlayerControllerCrouchScriptableObject : ScriptableObject
    {
        [Header("Stand")]
        public CrouchParametersClass standValues;

        [Header("Crouch")]
        public CrouchParametersClass crouchValues;

        [Header("Rounding Value")]
        public float roundingValue = 0.001f;

        [Serializable]
        public class CrouchParametersClass
        {
            public float updateTime = 0.1f;
            [Space]
            public float headHeight = 1.6f;
            public float colliderHeight = 1.8f;
            public float colliderCenterY = 1.05f;
        }
    }
}