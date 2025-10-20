using System;
using NOS.Controller;
using NOS.Player.Controller.Default;
using NOS.Player.Controller.General;
using UnityEngine;

namespace NOS.Player.Data
{
    [Serializable]
    public class PlayerReferences
    {
        #region Components

        [Header("Components")]
        public ComponentsClass components;

        [Serializable]
        public class ComponentsClass
        {
            public Transform transform;
            public Rigidbody rigidBody;
            public CapsuleCollider collider;
        }

        #endregion Components

        #region Objects

        [Header("Objects")]
        public ObjectsClass objects;

        [Serializable]
        public class ObjectsClass
        {
            public GameObject playerBase;
            public GameObject orientation;
            [Space]
            public GameObject head;
            public GameObject headTruck;
            public GameObject headBobbing;
            public GameObject headPivot;
            [Space]
            public GameObject body;
        }

        #endregion Objects

        #region Scriptable Objects

        [Header("Scriptable Objects")]
        public ScriptableObjectsClass scriptableObjects;

        [Serializable]
        public class ScriptableObjectsClass
        {
            #region General

            [field: SerializeField]
            public GeneralScriptableObjectsClass General { get; set; }

            [Serializable]
            public class GeneralScriptableObjectsClass
            {
            }

            #endregion General

            #region Default

            [field: SerializeField]
            public DefaultScriptableObjectsClass Default { get; set; }

            [Serializable]
            public class DefaultScriptableObjectsClass
            {
                public PlayerControllerLookScriptableObject look;
                public PlayerControllerCheckersDefaultScriptableObject checkers;
                public PlayerControllerMovementScriptableObject movement;
                public PlayerControllerCrouchScriptableObject crouch;
            }

            #endregion Default
        }

        #endregion Scriptable Objects
    }
}