using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerDefaultInteractionsParameters", menuName = "ScriptableObjects/Player/Default/Interaction")]
    public class PlayerControllerDefaultInteractionScriptableObject : ScriptableObject
    {
        [Header("Interaction Parameters")]
        public float interactionDistance = 2;
        public LayerMask interactableLayers;
        
        #region Debug

#if UNITY_EDITOR

        [Space(20), Header("Debug")]
        public string currentlySelectedObjectName;

#endif

        #endregion Debug
    }
}