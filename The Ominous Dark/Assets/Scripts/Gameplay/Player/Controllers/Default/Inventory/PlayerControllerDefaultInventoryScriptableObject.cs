using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerDefaultInventoryParameters", menuName = "ScriptableObjects/Player/Default/Inventory")]
    public class PlayerControllerDefaultInventoryScriptableObject : ScriptableObject
    {
        [Header("Scale of objects in hands")]
        public float objectInHandScale = 0.08f;

        [Header("Dropped Items Root")]
        public Transform rootForItemsRemovedFromInventory;

        [Header("Item Throwing")]
        public float holdTimeBeforeMaximalForce = 2f;
        public float holdTimeUntilForceBeginToRise = 0.2f; //For basic item dropping//
        public float throwForceMinimal = 1;
        public float throwForceMaximal = 5;
        public float throwPlayerVelocityMultiplier = 0.2f;

        [Header("Swapping Item")]
        public float itemHidingTransitionSpeed = 0.3f;
        public float itemPullOutTransitionSpeed = 0.3f;
        public Vector3 itemHideOffset = new (0, -0.1f, -0.02f);
    }
}