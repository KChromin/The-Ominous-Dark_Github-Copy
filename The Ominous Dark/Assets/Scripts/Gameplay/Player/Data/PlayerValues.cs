using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.Player.Data
{
    [Serializable]
    public class PlayerValues
    {
        #region General

        //Components, Objects, et cetera//
        [field: SerializeField]
        public GeneralValuesClass General { get; set; } = new();

        [Serializable]
        public class GeneralValuesClass
        {
            public Vector3 playerPositionLastFrame; //For checking if position is changing

            [Space, Header("Components")]
            //Rigidbody
            public Vector3 rigidBodyCurrentVelocity; //Speed
            public float rigidBodyCurrentVelocityY; //Falling & Rising
            public float rigidBodyCurrentVelocityMagnitudeXZ; //Horizontal Speed
            public int rigidBodyMass; //Player Mass

            [Space, Header("Objects")]
            //Orientation
            public Vector3 orientationCurrentPosition;
            public Quaternion orientationCurrentRotation;
        }

        #endregion General

        #region Default

        //Values only for default class//
        [field: SerializeField]
        public DefaultValuesClass Default { get; set; } = new();

        [Serializable]
        public class DefaultValuesClass
        {
            public RaycastHit GroundCheckHit;
            public Vector3 slopeCheckNormal;
            public float slopeCheckAngle;
            public Vector3 slopeCheckDirectionNormal;
            public RaycastHit CellingCheckHit;
            [Space]
            public float currentStamina;
        }

        #endregion Default
    }
}