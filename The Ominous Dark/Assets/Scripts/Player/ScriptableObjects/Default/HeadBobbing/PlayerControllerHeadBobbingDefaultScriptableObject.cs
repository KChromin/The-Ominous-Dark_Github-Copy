using System;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerHeadBobbingDefaultScriptableObject", menuName = "ScriptableObjects/Player/Default/HeadBobbing")]
    public class PlayerControllerHeadBobbingDefaultScriptableObject : ScriptableObject
    {
        [Header("Movement Values", order = 0)]
        [Header("Idle", order = 1)]
        public HeadBobbingDefaultValuesClass idleValues;

        [Header("Walk")]
        public HeadBobbingDefaultValuesClass walkValues;

        [Header("Run")]
        public HeadBobbingDefaultValuesClass runValues;

        [Header("Crouch Idle")]
        public HeadBobbingDefaultValuesClass crouchIdleValues;

        [Header("Crouch Walk")]
        public HeadBobbingDefaultValuesClass crouchWalkValues;

        [Header("Disabled")]
        public HeadBobbingDefaultValuesClass disabledValues;

        [Header("Smoothing")]
        public float transitionSmoothingTime = 0.02f;

        [Header("Reset Time")]
        public float timeToFullResetInSeconds = 0.1f;

        #region Head Bobbing Class

        [Serializable]
        public class HeadBobbingDefaultValuesClass
        {
            [Header("HeadBobbing Speed")]
            public float headBobbingSpeed = 2;

            [Header("Bobbing Ranges")]
            [Range(0f, 2f)]
            public float headBobbingMaxHeight = 0.02f;
            [Range(0f, 2f)]
            public float headBobbingMaxWidth = 0.02f;

            [Header("HeadBobbing Height by Width")]
            public AnimationCurve headBobbingHeightCurve = new()
            {
                keys = new Keyframe[]
                {
                    new(0, 0, 0, 0),
                    new(1, 1, 2f, 2f)
                },

                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };
        }

        #endregion Head Bobbing Class
    }
}