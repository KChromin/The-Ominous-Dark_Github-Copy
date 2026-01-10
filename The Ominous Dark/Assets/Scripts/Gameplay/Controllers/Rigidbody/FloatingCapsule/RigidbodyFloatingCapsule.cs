using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOS.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [DefaultExecutionOrder(-46)]
    public class RigidbodyFloatingCapsule : MonoBehaviour
    {
        #region Variables

        #region Parameters

        //Is Updating
        [Header("Execution")]
        [SerializeField]
        private bool isExecutingForce = true;

        [Header("Parameters")]
        [SerializeField]
        private RigidbodyFloatingCapsuleParametersClass parameters;

        [Serializable]
        private class RigidbodyFloatingCapsuleParametersClass
        {
            [Header("Float Height")]
            public float floatHeight = 0.3f;

            [Header("Check Origin Offset")]
            public Vector3 checkOriginOffset = new(0, -0.3f, 0);

            [Header("Spring Parameters")]
            public float springStrength = 500;
            public float springDampingForceUp = 30;
            public float springDampingForceDown = 11;

            [Header("Maximal Forces")]
            public float maximalForceAcceleration = 100;
            public float maximalForceObjectsUnder = 40;

            [Header("Going Down Threshold")]
            public float goingDownVelocityThreshold = -0.1f;

            [Header("Grounding Parameters")]
            public float groundingLenght = 0.1f;

            [Header("Ground Cast Radius Offset")]
            public float groundCastRadiusOffset = -0.04f;

            [Header("Ground layers")]
            public LayerMask groundLayers;

            #region Debug

#if UNITY_EDITOR
            [Header("Ground layers")]
            public bool drawDebugRaycast;
            public bool drawDebugSpherecast;
            public bool skipSpherecast;
            public Color castColor = Color.red;
            public Color castColorHit = Color.yellow;
#endif

            #endregion Debug
        }

        #endregion Parameters

        //Local
        private Rigidbody _rigidBody;
        private Vector3 _currentRigidbodyVelocity;
        private bool _isGoingDown;

        private float _floatingLenght; //Half height + floating height

        private float _checkRadius;
        private float _checkLenght; //Half height + floating height + grounding
        private float _checkLenghtSphere;
        private Vector3 _originCenter;

        private RaycastHit _groundCheckHit;
        private bool _didGroundCheckSpherecastHit;

        private bool _holdExecutionThisFixedUpdate;

        #endregion Variables

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();

            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            float halfCapsuleHeight = capsuleCollider.height / 2;
            _originCenter = capsuleCollider.center + parameters.checkOriginOffset;
            float distanceToCheckFromOriginToGround = halfCapsuleHeight + parameters.floatHeight + parameters.checkOriginOffset.y;
            ;

            //Radius
            _checkRadius = capsuleCollider.radius + parameters.groundCastRadiusOffset;

            //Lenght
            _floatingLenght = distanceToCheckFromOriginToGround;
            _checkLenght = _floatingLenght + parameters.groundingLenght;
            _checkLenghtSphere = _checkLenght - _checkRadius;
        }

        private void FixedUpdate()
        {
            if (DoesGroundCheckHit())
            {
                _currentRigidbodyVelocity = _rigidBody.linearVelocity;
                CheckIfGoingDown();
                ExecuteSpringForce();
            }
        }

        private void CheckIfGoingDown()
        {
            _isGoingDown = _currentRigidbodyVelocity.y <= parameters.goingDownVelocityThreshold;
        }

        private void ExecuteSpringForce()
        {
            if (!isExecutingForce) return;

            if (_holdExecutionThisFixedUpdate)
            {
                _holdExecutionThisFixedUpdate = false;
                return;
            }

            Vector3 hitObjectLinearVelocity = Vector3.zero;
            Rigidbody hitObjectRigidBody = _groundCheckHit.rigidbody;
            float checkHitDistance;

            #region Assign by Check Type

            if (_didGroundCheckSpherecastHit)
            {
                checkHitDistance = _groundCheckHit.distance + _checkRadius;
            }
            else //Raycast
            {
                checkHitDistance = _groundCheckHit.distance;
            }

            #endregion Assign by Check Type

            if (hitObjectRigidBody)
            {
                hitObjectLinearVelocity = hitObjectRigidBody.linearVelocity;
            }

            float checkDirectionVelocity = Vector3.Dot(Vector3.down, _currentRigidbodyVelocity);
            float hitObjectDirectionVelocity = Vector3.Dot(Vector3.down, hitObjectLinearVelocity);

            float relativeVelocity = checkDirectionVelocity - hitObjectDirectionVelocity;

            float x = checkHitDistance - _floatingLenght;

            float dampingForce = parameters.springDampingForceUp;

            if (_isGoingDown)
            {
                dampingForce = parameters.springDampingForceDown;
            }

            float springForce = x * parameters.springStrength - relativeVelocity * dampingForce;

            springForce = Mathf.Clamp(springForce, -parameters.maximalForceAcceleration, parameters.maximalForceAcceleration);

            _rigidBody.AddForce(Vector3.down * springForce, ForceMode.Acceleration);

            //Apply force to objects under//
            if (hitObjectRigidBody)
            {
                Vector3 capsulePosition = transform.position;
                capsulePosition.y = 0;
                Vector3 objectPosition = _groundCheckHit.point;
                objectPosition.y = 0;
                Vector3 direction = Vector3.Normalize(capsulePosition - objectPosition);
                direction.y = -1;
                springForce = Mathf.Clamp(springForce, -parameters.maximalForceObjectsUnder, 0);
                hitObjectRigidBody.AddForce(direction * springForce, ForceMode.Force);
            }
        }

        private bool DoesGroundCheckHit()
        {
            Vector3 checkOriginPosition = transform.position + _originCenter;

            _didGroundCheckSpherecastHit = false;

#if UNITY_EDITOR
            //Spherecast
            if (!parameters.skipSpherecast && Physics.SphereCast(checkOriginPosition, _checkRadius, Vector3.down, out _groundCheckHit, _checkLenghtSphere, parameters.groundLayers, QueryTriggerInteraction.Ignore))
            {
                _didGroundCheckSpherecastHit = true;
                return true;
            }
#else
            if (Physics.SphereCast(checkOriginPosition, _checkRadius, Vector3.down, out _groundCheckHit, _checkLenghtSphere, parameters.groundLayers, QueryTriggerInteraction.Ignore))
            {
                _didGroundCheckSpherecastHit = true;
                return true;
            }
#endif

            //Raycast
            if (Physics.Raycast(checkOriginPosition, Vector3.down, out _groundCheckHit, _checkLenght, parameters.groundLayers, QueryTriggerInteraction.Ignore))
            {
                return true;
            }

            return false;
        }

        public void SuspendSpringExecution(float suspendingTimeInSeconds)
        {
            _ = SuspendForceExecutionForSomeTime(suspendingTimeInSeconds);
        }

        async UniTaskVoid SuspendForceExecutionForSomeTime(float suspendingTimeInSeconds)
        {
            isExecutingForce = false;
            await UniTask.WaitForSeconds(suspendingTimeInSeconds, cancellationToken: this.GetCancellationTokenOnDestroy());
            isExecutingForce = true;
        }

        public void SuspendExecutionNextFixedUpdate()
        {
            _holdExecutionThisFixedUpdate = true;
        }

        #region Debug

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            if (parameters.drawDebugRaycast)
            {
                //Raycast
                if (_didGroundCheckSpherecastHit)
                {
                    Gizmos.color = parameters.castColorHit;
                    Gizmos.DrawLine(transform.position + _originCenter, _groundCheckHit.point);
                }
                else
                {
                    Gizmos.color = parameters.castColor;
                    Gizmos.DrawLine(transform.position + _originCenter, transform.position + _originCenter + Vector3.down * _checkLenght);
                }
            }

            if (!parameters.drawDebugSpherecast) return;
            if (!_didGroundCheckSpherecastHit && _groundCheckHit.collider != null)
            {
                Gizmos.color = parameters.castColorHit;
                Gizmos.DrawLine(transform.position + _originCenter, _groundCheckHit.point);
                Gizmos.DrawWireSphere(transform.position + _originCenter + Vector3.down * _groundCheckHit.distance, _checkRadius);
            }
            else
            {
                Gizmos.color = parameters.castColor;
                Gizmos.DrawLine(transform.position + _originCenter, transform.position + _originCenter + Vector3.down * _checkLenghtSphere);
                Gizmos.DrawWireSphere(transform.position + (_originCenter + Vector3.down * _checkLenghtSphere), _checkRadius);
            }
        }
#endif

        #endregion Debug
    }
}