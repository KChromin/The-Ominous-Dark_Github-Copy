using System;
using NOS.Patterns.Controller;
using UnityEngine;

namespace NOS.Controller
{
    //Cannot find exact formula that Unity uses, so this is a simplified version for simple not physically correct use//
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyManualSimplifiedLinearDamping : EnableableControllerBaseMonoBehaviour
    {
        #region Variables

        [Header("Linear Damping Value")]
        [field: SerializeField]
        public float LinearDamping { get; set; }

        [Header("Damped Axes")]
        [SerializeField]
        private bool applyLinearDampingForHorizontalAxes = true;
        [SerializeField]
        private bool applyLinearDampingForVerticalAxis = true;

        //Local
        private Rigidbody _rigidBody;

        #endregion Variables

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!IsEnabled) return;

            Vector3 velocity = _rigidBody.linearVelocity;

            #region Options

            if (!applyLinearDampingForHorizontalAxes)
            {
                velocity.x = 0;
                velocity.z = 0;
            }

            if (!applyLinearDampingForVerticalAxis)
            {
                velocity.y = 0;
            }

            #endregion Options

            Vector3 dampingForce = -LinearDamping * velocity;
            _rigidBody.AddForce(dampingForce, ForceMode.Acceleration);
        }
    }
}