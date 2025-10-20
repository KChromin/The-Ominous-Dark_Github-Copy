using System;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerMovementParameters", menuName = "ScriptableObjects/Player/Default/Movement")]
    public class PlayerControllerMovementScriptableObject : ScriptableObject
    {
        #region Movement Values

        [Header("Movement Values", order = 0)]
        [Header("Idle", order = 1)]
        public MovementValuesClass idleValues;

        [Header("Walk")]
        public MovementValuesClass walkValues;

        [Header("Run")]
        public MovementValuesClass runValues;

        [Header("Crouch Idle")]
        public MovementValuesClass crouchIdleValues;

        [Header("Crouch Walk")]
        public MovementValuesClass crouchWalkValues;

        [Header("Factors")]
        [Range(0f, 1f)]
        public float factorMoveSideways = 1;
        [Range(0f, 1f)]
        public float factorMoveBackwards = 1;

        [Header("Slope Handling")]
        [Range(0f, 100f)]
        public float slopeSlidingSpeed = 10;
        [Range(0f, 1f)]
        public float slopeSlidingMovementMultiplier = 0.1f;

        #endregion Movement Values
        
        #region Movement Value Class

        [Serializable]
        public class MovementValuesClass
        {
            [Header("Max Speed")]
            public float maxSpeed = 5f;

            [Header("Acceleration Speed")]
            public float accelerationSpeed = 10;
            public AnimationCurve accelerationFactorFromDot = new()
            {
                keys = new Keyframe[]
                {
                    new(0, 2),
                    new(0.5f, 1),
                    new(1, 1)
                },
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };

            [Header("Max Acceleration Force")]
            public float maxAccelerationForce = 40f;
            public AnimationCurve maxAccelerationForceFactorFromDot = new()
            {
                keys = new Keyframe[]
                {
                    new(0, 2),
                    new(0.5f, 1),
                    new(1, 1)
                },
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };
        }

        #endregion Movement Value Class
    }
}