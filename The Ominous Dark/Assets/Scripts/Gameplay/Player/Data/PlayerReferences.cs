using System;
using NOS.Controllers;
using NOS.Player.Controller.Default;
using UnityEngine;

namespace NOS.Player.Data
{
    //Player static references
    
    [Serializable]
    public class PlayerReferences
    {
        #region Components

        [field: Header("Components")]
        [field: SerializeField]
        public ComponentsClass Components { get; set; } = new();

        [Serializable]
        public class ComponentsClass
        {
            public Transform transform;
            public Rigidbody rigidBody;
            public CapsuleCollider collider;
            public RigidbodyFloatingCapsule floatingCapsule;
        }

        #endregion Components

        #region Objects

        [field: Header("Objects")]
        [field: SerializeField]
        public ObjectsClass Objects { get; set; } = new();

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
            public GameObject hands;
            public GameObject handsItemSlot;
            [Space]
            public GameObject body;
        }

        #endregion Objects

        #region Scriptable Objects

        [field: Header("Scriptable Objects")]
        [field: SerializeField]
        public ScriptableObjectsClass ScriptableObjects { get; set; } = new();

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
                public PlayerControllerJumpScriptableObject jump;
                public PlayerControllerHeadBobbingDefaultScriptableObject headBobbing;
                public PlayerControllerDefaultInteractionScriptableObject interaction;
                public PlayerControllerDefaultInventoryScriptableObject inventory;
                public PlayerControllerDefaultStaminaScriptableObject stamina;
            }

            #endregion Default
        }

        #endregion Scriptable Objects
    }
}