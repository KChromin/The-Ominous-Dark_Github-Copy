using System;
using NOS.GameplayManagers;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    [RequireComponent(typeof(Rigidbody))]
    public class InteractableRigidbodyPusher : InteractableBase
    {
        [Space, SerializeField]
        private float pushForce = 1f;

        [Space, SerializeField]
        private PushModes pushMode;

        [Space, SerializeField]
        private float maximalSpeed = 2f;

        private enum PushModes
        {
            Direction,
            Point
        }

        private Rigidbody _rigidBody;
        private GameplayReferenceManager _referenceManager;
        private Transform _playerTransform;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _referenceManager = GameplayReferenceManager.Instance;
            _playerTransform = _referenceManager.PlayerTransform;
        }

        protected override void Interact()
        {
            if (_rigidBody.linearVelocity.magnitude > maximalSpeed) return;

            Vector3 playerPosition = _playerTransform.position;
            playerPosition.y = 0;
            Vector3 objectPosition = transform.position;
            objectPosition.y = 0;

            Vector3 playerPushDirection = Vector3.Normalize(objectPosition - playerPosition);

            switch (pushMode)
            {
                default:
                case PushModes.Direction:
                    _rigidBody.AddForce(playerPushDirection * pushForce, ForceMode.Impulse);
                    break;
                case PushModes.Point:
                    Vector3 hitPoint = _referenceManager.PlayerDynamicReferences.Default.InteractionDefault.Hit.point;
                    _rigidBody.AddForceAtPosition(playerPushDirection * pushForce, hitPoint, ForceMode.VelocityChange);
                    break;
            }
        }
    }
}