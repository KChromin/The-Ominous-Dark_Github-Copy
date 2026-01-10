using System;
using NOS.Controllers.Interactions;
using NOS.Item;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.Player.Data
{
    //Dynamic references used by player controllers, to give data access for GameplayManagers

    [Serializable]
    public class PlayerDynamicReferences
    {
        [field: SerializeField]
        public DefaultDynamicReferencesClass Default { get; set; }

        [Serializable]
        public class DefaultDynamicReferencesClass
        {
            #region Movement

            [field: SerializeField]
            public InteractionDefaultClass MovementDefault { get; set; }

            [Serializable]
            public class MovementDefaultClass
            {
            }

            #endregion Movement

            #region Interaction

            [field: SerializeField]
            public InteractionDefaultClass InteractionDefault { get; set; }

            [Serializable]
            public class InteractionDefaultClass
            {
                public RaycastHit Hit;
                public InteractableBase currentSelectedInteractableObject;
                public Action OnSelectObjectChange;
            }

            #endregion Interaction

            #region Inventory

            [field: SerializeField]
            public InventoryDefaultClass InventoryDefault { get; set; }

            [Serializable]
            public class InventoryDefaultClass
            {
                //Slots
                public ItemBase[] currentInventoryItems;
                public int currentSelectedSlot;

                public ItemBase GetCurrentItem()
                {
                    return currentInventoryItems[currentSelectedSlot];
                }

                //Actions
                //When changing slot in hands//
                public Action OnSelectedSlotChange;

                //When interacted with item to pick up//
                public Action<ItemBase> OnTryingToPickupItem;

                public Action<int> OnPickedUpItemGUINotice;
                public Action<int> OnDroppedItemGUINotice;
                public Action<float> OnThrowItemGUINotice;
                public Action OnEndThrowItemGUINotice;

                //Interactable with item slot//
                public Action OnInsertItemIntoSlot; //Player removes it from the inventory
                public Action<ItemBase> OnTakeItemFromSlot; //Player adds it to the inventory
            }

            #endregion Inventory

            #region Stamina

            [field: SerializeField]
            public StaminaDefaultClass StaminaDefault { get; set; }

            [Serializable]
            public class StaminaDefaultClass
            {
                public Action<float> OnStaminaValueChange;
                public Action<float> OnStaminaOverlayUpdate;
            }


            #endregion Stamina
        }
    }
}