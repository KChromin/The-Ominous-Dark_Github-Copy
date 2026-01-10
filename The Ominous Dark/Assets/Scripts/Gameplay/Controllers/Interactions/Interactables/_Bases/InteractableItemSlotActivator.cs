using System;
using System.Collections.Generic;
using NOS.GameplayManagers;
using NOS.Item;
using NOS.Player.Data;
using UnityEngine;
using UnityEngine.Localization;

namespace NOS.Controllers.Interactions
{
    public abstract class InteractableItemSlotActivator : InteractableActivator
    {
        [field: Space, SerializeField]
        public InteractableItemSettingsClass ItemSlotSettings { get; set; } = new();

        [Header("Inserted Item")]
        [SerializeField]
        private ItemBase itemInSlot;

        private PlayerDynamicReferences.DefaultDynamicReferencesClass _dynamicReferences;
        private readonly List<ItemScriptableObject> _insertableItemsScriptableObjects = new();
        private readonly List<AdditionalObjectForSelectionClass> _additionalObjectsForSelection = new();

        //Localization string paths -- Dangerous!
        private readonly LocalizedString _localizationActionTake = new("InteractablesTable", "Action_Take");
        private readonly LocalizedString _localizationActionNeed = new("InteractablesTable", "Action_Need");


        [Serializable]
        private class AdditionalObjectForSelectionClass
        {
            public AdditionalObjectForSelectionClass(GameObject additionalObject)
            {
                ObjectTransform = additionalObject;
                OriginalLayer = ObjectTransform.layer;
            }

            [field: SerializeField]
            public GameObject ObjectTransform { get; private set; }

            [field: SerializeField]
            public int OriginalLayer { get; private set; }
        }

        private void Start()
        {
            _dynamicReferences = GameplayReferenceManager.Instance.PlayerDynamicReferences.Default;
            SetupInsertableItems();
        }

        private void SetupInsertableItems()
        {
            for (int i = 0; i < ItemSlotSettings.InsertableItemsPrefabs.Length; i++)
            {
                ItemScriptableObject itemParameters = ItemSlotSettings.InsertableItemsPrefabs[i].GetComponent<ItemBase>().itemParameters;

                if (!itemParameters)
                {
                    continue;
                }

                _insertableItemsScriptableObjects.Add(itemParameters);
            }

#if !UNITY_EDITOR
            //Clean GameObject Array for memory's sake//
            ItemSlotSettings.InsertableItemsPrefabs = null;
#endif
        }

        public override void BeginInteraction()
        {
            //Taking Inserted Item//
            if (itemInSlot && ItemSlotSettings.CanRetrieveItem && IsPlayerHandEmpty())
            {
                base.BeginInteraction();
            }
            //Inserting Item//
            else if (!itemInSlot && IsPlayerHoldingRightItem())
            {
                base.BeginInteraction();
            }
        }

        private bool IsPlayerHoldingRightItem()
        {
            ItemBase currentItem = _dynamicReferences.InventoryDefault.currentInventoryItems[_dynamicReferences.InventoryDefault.currentSelectedSlot];

            if (!currentItem)
            {
                return false;
            }

            for (int i = 0; i < _insertableItemsScriptableObjects.Count; i++)
            {
                if (currentItem.itemParameters == _insertableItemsScriptableObjects[i])
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPlayerHandEmpty()
        {
            ItemBase currentItem = _dynamicReferences.InventoryDefault.GetCurrentItem();

            return !currentItem;
        }

        private void SwitchGhostMeshes(bool newState)
        {
            for (int i = 0; i < ItemSlotSettings.GhostMeshes.Length; i++)
            {
                ItemSlotSettings.GhostMeshes[i].SetActive(newState);
            }
        }

        protected override void Interact()
        {
            //Taking Inserted Item//
            if (itemInSlot)
            {
                _dynamicReferences.InventoryDefault.OnTakeItemFromSlot?.Invoke(itemInSlot);

                itemInSlot = null;

                RemoveAdditionalObjectsForSelection();

                SwitchGhostMeshes(true);
            }
            //Inserting Item//
            else
            {
                ItemBase itemToInsert = _dynamicReferences.InventoryDefault.GetCurrentItem();
                _dynamicReferences.InventoryDefault.OnInsertItemIntoSlot?.Invoke();
                itemInSlot = itemToInsert;

                SwitchGhostMeshes(false);

                AddAdditionalObjectsForSelection();

                itemToInsert.transform.SetParent(ItemSlotSettings.ItemInsertionSlot);
                itemToInsert.transform.localPosition = Vector3.zero;
                itemToInsert.transform.localRotation = Quaternion.identity;
                itemToInsert.transform.localScale = Vector3.one;
            }

            RefreshSelection();
            base.Interact();
            OnActivation();
        }

        private void RefreshSelection()
        {
            if ((itemInSlot && ItemSlotSettings.CanRetrieveItem) || !itemInSlot)
            {
                SetLayerMaskAdditionalObjects(LayerMaskSetOptions.Select);
            }
        }

        private void AddAdditionalObjectsForSelection()
        {
            Transform[] itemInSlotTransforms = itemInSlot.GetComponentsInChildren<Transform>();

            for (int i = 0; i < itemInSlotTransforms.Length; i++)
            {
                _additionalObjectsForSelection.Add(new AdditionalObjectForSelectionClass(itemInSlotTransforms[i].gameObject));
            }
        }

        private void RemoveAdditionalObjectsForSelection()
        {
            _additionalObjectsForSelection.Clear();
        }

        protected override void SetLayerMasks(LayerMaskSetOptions option)
        {
            base.SetLayerMasks(option);
            SetLayerMaskAdditionalObjects(option);
        }

        protected void SetLayerMaskAdditionalObjects(LayerMaskSetOptions option)
        {
            switch (option)
            {
                case LayerMaskSetOptions.Default:
                    for (int i = 0; i < _additionalObjectsForSelection.Count; i++)
                    {
                        _additionalObjectsForSelection[i].ObjectTransform.layer = _additionalObjectsForSelection[i].OriginalLayer;
                    }

                    break;
                case LayerMaskSetOptions.Select:
                    for (int i = 0; i < _additionalObjectsForSelection.Count; i++)
                    {
                        _additionalObjectsForSelection[i].ObjectTransform.layer = SelectedMaskLayer;
                    }

                    break;
                case LayerMaskSetOptions.NoPlayerCollision:
                    for (int i = 0; i < _additionalObjectsForSelection.Count; i++)
                    {
                        _additionalObjectsForSelection[i].ObjectTransform.layer = NoPlayerCollisionLayer;
                    }

                    break;
            }
        }

        protected abstract void OnActivation();

        public override string GetCurrentLocalizationText()
        {
            string currentLocalizationText;

            if (itemInSlot && ItemSlotSettings.CanRetrieveItem)
            {
                currentLocalizationText = _localizationActionTake.GetLocalizedString() + " " + itemInSlot.itemParameters.itemName.GetLocalizedString();
            }
            else if (_insertableItemsScriptableObjects.Count == 1)
            {
                currentLocalizationText = _localizationActionNeed.GetLocalizedString() + " " + _insertableItemsScriptableObjects[0].itemName.GetLocalizedString();
            }
            else
            {
                currentLocalizationText = base.GetCurrentLocalizationText();
            }

            return currentLocalizationText;
        }
    }

    [Serializable]
    public class InteractableItemSettingsClass
    {
        [field: Header("Objects that Can Be Inserted")]
        [field: SerializeField]
        // At Runtime start it is converted to array of Scriptable Objects, and leaned from memory afterward
        // Made that way to be more comfortable to set up
        public GameObject[] InsertableItemsPrefabs { get; set; }

        [field: Space, Header("Item Insertion Slot")]
        [field: SerializeField]
        public Transform ItemInsertionSlot { get; set; }

        [field: Space, Header("Ghost Meshes")]
        [field: SerializeField]
        public GameObject[] GhostMeshes { get; set; }

        [field: Space, Header("If player can take object back")]
        [field: SerializeField]
        public bool CanRetrieveItem { get; set; } = true;
    }
}