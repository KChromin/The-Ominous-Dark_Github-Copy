using UnityEngine;

namespace NOS.Controllers
{
    [CreateAssetMenu(fileName = "RigidbodyFloatingCapsuleParameters", menuName = "ScriptableObjects/Controllers/Rigidbody/FloatingCapsule")]
    public class RigidbodyFloatingCapsuleScriptableObject : ScriptableObject
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
}