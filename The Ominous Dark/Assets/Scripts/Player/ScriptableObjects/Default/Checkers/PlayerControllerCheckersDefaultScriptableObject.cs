using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerCheckersDefaultParameters", menuName = "ScriptableObjects/Player/Default/Checkers")]
    public class PlayerControllerCheckersDefaultScriptableObject : ScriptableObject
    {
        #region Ground Check

        [Header("Ground Check")]
        public float groundCheckDistance;
        public float groundCheckRadius;
        
        #endregion Ground Check

        #region Slope Check

        [Header("Slope Check")]
        public float slopeCheckMaxSlope;
        public Vector3 slopeCheckRaycastOffset = new (0,0.02f,0);
        
        #endregion Slope Check
        
        #region Celling Check

        [Header("Celling Check")]
        public float cellingCheckDistance;
        public float cellingCheckRadius;
        
        #endregion Celling Check

        #region Layer Masks

        [Header("Layer Masks")]
        public LayerMask groundLayer;

        #endregion Layer Masks

        #region Check Position Offset

        public Vector3 checkPositionOffsetFromOriginForGroundCheck = new(0, 1.025f, 0);
        public Vector3 checkPositionOffsetFromOriginForCellingCheck = new(0, 0.5f, 0);

        #endregion Check Position Offset

        #region Debug

#if UNITY_EDITOR

        [Header("Debug Gizmos", order = 0)]
        [Header("Ground", order = 1)]
        public bool debugGroundCheckDraw;
        public Color debugGroundCheckColorHit = Color.green;
        public Color debugGroundCheckColor = Color.red;

        [Header("Celling")]
        public bool debugCellingCheckDraw;
        public Color debugCellingCheckColorHit = Color.darkOliveGreen;
        public Color debugCellingCheckColor = Color.orangeRed;

#endif

        #endregion Debug
    }
}