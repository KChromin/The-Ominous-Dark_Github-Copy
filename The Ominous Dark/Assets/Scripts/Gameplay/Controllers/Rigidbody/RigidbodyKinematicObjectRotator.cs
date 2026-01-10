using System;
using UnityEngine;

namespace NOS.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyKinematicObjectRotator : MonoBehaviour
    {
        [SerializeField]
        private Vector3 rotationVector;

        private Rigidbody _rigidBody;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Quaternion rotation = _rigidBody.rotation;
            rotation *= Quaternion.Euler(rotationVector);
            
            _rigidBody.MoveRotation(rotation);
        }
    }
}