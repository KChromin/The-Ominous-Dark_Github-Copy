using System;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Header("Sliding from Steep Slope")]
        public MovementValuesClass slidingFromSteepSlopeValues;

        [Space]
        [Header("Max Speed Smooth change in time value")]
        public float maxSpeedUpdateInTimeValue = 0.2f;
        
        [Space]
        [Header("Factors")]
        [Range(0f, 1f)]
        public float factorMoveSideways = 1;
        [Range(0f, 1f)]
        public float factorMoveBackwards = 1;

        [Space]
        [Header("In Air Controls")]
        [Range(0f, 100f)]
        public float inAirControlAccelerationSpeed;
        [Range(0f, 10f)]
        public float inAirMaximalMomentumAdditionBaseValue = 0.2f;
        [Range(0f, 100f)]
        public float inAirMaximalMomentumAdditionJumpingInPlace = 1;
        [Range(0f, 1f)]
        public float inAirMomentumKeepMultiplier = 0.9f;

        [Space]
        [Header("Sliding")]
        [Range(0f, 1f)]
        public float slidingControlMultiplier = 0.1f;
        public float slidingMaxSpeedAccelerationSpeed = 1f;
        public float slidingMaxSpeedReductionSpeed = 1f;
        public float slidingTimeBeforeDisablingFloatingCapsule = 0.1f;

        [Space]
        [Header("Steep slope movement speed reduction")]
        public float steepSlopeMovementReductionThresholdStart = 15f;
        public float steepSlopeMovementReductionThresholdEnd = 45f;
        public float steepSlopeMovementReductionMaximalReductionMultiplier = 0.2f;
        
        [Space]
        [Header("Minimal Movement Thresholds")]
        public float minimalMovingThreshold = 0.1f;
        public float minimalRunningThreshold = 3f;

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