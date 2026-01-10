using System;
using NOS.GameManagers.Input;
using NOS.GameManagers.Settings;
using NOS.Item;
using NOS.Patterns.Controller;
using NOS.Player.Controller.Default;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller
{
    public class PlayerControllerDefaultInventory : ControllerBase
    {
        public PlayerControllerDefaultInventory(InputDataContainer input, PlayerConditions conditions, PlayerReferences references, PlayerValues values, PlayerDynamicReferences dynamicReferences, SettingsContainers settings)
        {
            _input = input;
            _conditions = conditions.Default;
            _values = values;
            _parameters = references.ScriptableObjects.Default.inventory;
            _itemSlot = references.Objects.handsItemSlot.transform;
            _dynamicReferences = dynamicReferences.Default.InventoryDefault;
            _settings = settings;

            //Set inventory size//
            _dynamicReferences.currentInventoryItems = new ItemBase[InventorySize];

            SubscribeToEvents();
        }

        #region Variables

        private readonly InputDataContainer _input;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly PlayerValues _values;
        private readonly PlayerControllerDefaultInventoryScriptableObject _parameters;
        private readonly Transform _itemSlot;
        private readonly PlayerDynamicReferences.DefaultDynamicReferencesClass.InventoryDefaultClass _dynamicReferences;
        private readonly SettingsContainers _settings;

        private const int InventorySize = 5;

        private int _previousSelectedItem;
        private float _itemThrowHoldTime;
        private float _throwForce01;

        #endregion Variables

        public override void Update()
        {
            ItemThrowUpdate();
        }

        #region Private methodes

        private void TryToPickUpItem(ItemBase itemToAdd)
        {
            //Try to add item to current slot
            if (!_dynamicReferences.GetCurrentItem())
            {
                AddItemToInventory(itemToAdd, _dynamicReferences.currentSelectedSlot);
                _dynamicReferences.OnSelectedSlotChange?.Invoke(); //Refresh hand after picking item//
                return;
            }

            //Try to add to empty slot, when not do not pick up
            for (int i = 0; i < InventorySize; i++)
            {
                if (!_dynamicReferences.currentInventoryItems[i])
                {
                    AddItemToInventory(itemToAdd, i);
                    return;
                }
            }

            //When loop did not find spot
#if UNITY_EDITOR
            Debug.Log("Inventory is full");
#endif
        }

        private void UpdateScroll(Vector2 scrollDelta)
        {
            if (_conditions.cases.inventoryItemSwitchingIsBlocked) return;

            //When next
            if (scrollDelta.y > 0)
            {
                _dynamicReferences.currentSelectedSlot++;

                //Loop
                if (_dynamicReferences.currentSelectedSlot > _dynamicReferences.currentInventoryItems.Length - 1)
                {
                    if (_settings.game.inventoryItemScrollLoopsOnEnds)
                    {
                        _dynamicReferences.currentSelectedSlot = 0;
                    }
                    else
                    {
                        _dynamicReferences.currentSelectedSlot = _dynamicReferences.currentInventoryItems.Length - 1;
                    }
                }
            }
            else //when prevoius
            {
                _dynamicReferences.currentSelectedSlot--;

                //Loop
                if (_dynamicReferences.currentSelectedSlot < 0)
                {
                    if (_settings.game.inventoryItemScrollLoopsOnEnds)
                    {
                        _dynamicReferences.currentSelectedSlot = _dynamicReferences.currentInventoryItems.Length - 1;
                    }
                    else
                    {
                        _dynamicReferences.currentSelectedSlot = 0;
                    }
                }
            }

            //Here Update Item//
            _dynamicReferences.OnSelectedSlotChange?.Invoke();
        }

        private void UpdateScrollNumpad(int selectedNumber)
        {
            if (_conditions.cases.inventoryItemSwitchingIsBlocked) return;

            _dynamicReferences.currentSelectedSlot = selectedNumber;

            //Here Update Item//
            _dynamicReferences.OnSelectedSlotChange?.Invoke();
        }

        //Process item after addition//
        private void AddItemToInventory(ItemBase itemToAdd, int slotNumber)
        {
            //Add to inventory
            _dynamicReferences.currentInventoryItems[slotNumber] = itemToAdd;

            itemToAdd.OnBeingAddedToInventory();

            Transform newItemTransform = itemToAdd.transform;

            newItemTransform.SetParent(_itemSlot);
            newItemTransform.localPosition = Vector3.zero;
            newItemTransform.localRotation = Quaternion.identity;
            newItemTransform.localScale = Vector3.one * _parameters.objectInHandScale;

            _dynamicReferences.OnPickedUpItemGUINotice?.Invoke(slotNumber);

            newItemTransform.gameObject.SetActive(false);
        }

        private void UpdateItemInHands()
        {
            if (_previousSelectedItem == _dynamicReferences.currentSelectedSlot && (_dynamicReferences.currentInventoryItems[_previousSelectedItem] && _dynamicReferences.currentInventoryItems[_previousSelectedItem].gameObject.activeSelf)) return;

            //Hide current item
            if (_dynamicReferences.currentInventoryItems[_previousSelectedItem])
            {
                DeactivateItemInHand(_previousSelectedItem);
            }

            //Pull up new
            if (_dynamicReferences.GetCurrentItem())
            {
                ActivateItemInHand(_dynamicReferences.currentSelectedSlot);
            }
            else //When null, that means it's empty, so just update ui
            {
            }

            _previousSelectedItem = _dynamicReferences.currentSelectedSlot;
        }

        private void RemoveItemFromInventory(int slotNumber, float throwForce01 = 0, bool insertingToSlot = false)
        {
            ItemBase itemToRemove = _dynamicReferences.currentInventoryItems[slotNumber];

            itemToRemove.CancelMainAction();
            itemToRemove.CancelSecondaryAction();

            if (insertingToSlot)
            {
                itemToRemove.OnBeingInsertedToSlot();
            }
            else
            {
                itemToRemove.OnBeingRemovedFromInventory(throwForce01);
            }

            Transform removedItemTransform = itemToRemove.transform;

            removedItemTransform.SetParent(_parameters.rootForItemsRemovedFromInventory);
            //  removedItemTransform.localScale = Vector3.one;
            //Instead call methode in item that makes it bigger

            removedItemTransform.gameObject.SetActive(true);

            //Remove item
            _dynamicReferences.currentInventoryItems[slotNumber] = null;

            _dynamicReferences.OnDroppedItemGUINotice?.Invoke(slotNumber);
        }

        private void ActivateItemInHand(int slotNumber)
        {
            _dynamicReferences.currentInventoryItems[slotNumber].gameObject.SetActive(true);
            _dynamicReferences.currentInventoryItems[slotNumber].SetItemInHandsActivity(true);
        }

        private void DeactivateItemInHand(int slotNumber)
        {
            _dynamicReferences.currentInventoryItems[slotNumber].CancelMainAction();
            _dynamicReferences.currentInventoryItems[slotNumber].CancelSecondaryAction();
            _dynamicReferences.currentInventoryItems[slotNumber].SetItemInHandsActivity(false);
            _dynamicReferences.currentInventoryItems[slotNumber].gameObject.SetActive(false);
        }

        #region Using items

        private void UseItemMainAction()
        {
            if (_conditions.cases.inventoryWantsToThrowItem)
            {
                ItemThrowCancel();
                return;
            }

            _dynamicReferences.GetCurrentItem()?.PerformMainAction();
        }

        private void UseItemSecondaryAction()
        {
            if (_conditions.cases.inventoryWantsToThrowItem)
            {
                ItemThrowCancel();
                return;
            }

            _dynamicReferences.GetCurrentItem()?.PerformSecondaryAction();
        }

        private void CancelItemMainAction()
        {
            _dynamicReferences.GetCurrentItem()?.CancelMainAction();
        }

        private void CancelItemSecondaryAction()
        {
            _dynamicReferences.GetCurrentItem()?.CancelSecondaryAction();
        }

        #endregion Using items

        #region Throw

        //Begin to throw
        private void ItemThrowBegin()
        {
            if (!_dynamicReferences.GetCurrentItem()) return;

            _conditions.cases.inventoryItemSwitchingIsBlocked = true;
            _conditions.cases.inventoryWantsToThrowItem = true;
        }

        //Update throw hold time
        private void ItemThrowUpdate()
        {
            if (_conditions.cases.inventoryWantsToThrowItem)
            {
                _itemThrowHoldTime += Time.deltaTime;
                _throwForce01 = _itemThrowHoldTime / _parameters.holdTimeBeforeMaximalForce;
                _dynamicReferences.OnThrowItemGUINotice?.Invoke(_throwForce01);
            }
        }

        //Execute Throw
        private void ItemThrowEnd()
        {
            if (!_conditions.cases.inventoryWantsToThrowItem) return;

            ItemBase targetItem = _dynamicReferences.GetCurrentItem();

            RemoveItemFromInventory(_dynamicReferences.currentSelectedSlot, _throwForce01);

            float throwForce;

            if (_itemThrowHoldTime <= _parameters.holdTimeUntilForceBeginToRise)
            {
                throwForce = _parameters.throwForceMinimal;
            }
            else
            {
                _itemThrowHoldTime = Mathf.Clamp(_itemThrowHoldTime, 0, _parameters.holdTimeBeforeMaximalForce);
                throwForce = Mathf.Lerp(_parameters.throwForceMinimal, _parameters.throwForceMaximal, _throwForce01);
            }

            _dynamicReferences.OnEndThrowItemGUINotice?.Invoke();

            ItemThrowExecute(targetItem, throwForce);

            ItemThrowReset();
        }

        //Throw is canceled by other input
        private void ItemThrowCancel()
        {
            if (!_conditions.cases.inventoryWantsToThrowItem) return;
            ItemThrowReset();
            _dynamicReferences.OnEndThrowItemGUINotice?.Invoke();
        }

        private void ItemThrowExecute(ItemBase targetItem, float throwForce)
        {
            Rigidbody currentObject = targetItem.GetComponent<Rigidbody>();

            Vector3 itemForwardDirection = currentObject.transform.forward;
            Vector3 playerVelocityAddition = _values.General.rigidBodyCurrentVelocity;

            float throwDirectionDot = Vector3.Dot(itemForwardDirection, playerVelocityAddition.normalized);

            if (throwDirectionDot <= 0)
            {
                playerVelocityAddition = Vector3.zero;
            }
            else
            {
                playerVelocityAddition *= throwDirectionDot * _parameters.throwPlayerVelocityMultiplier;
            }

            Vector3 itemRawThrowForce = itemForwardDirection * throwForce;

            currentObject.AddForce((itemRawThrowForce + playerVelocityAddition) * currentObject.mass, ForceMode.Impulse);
        }

        private void ItemThrowReset()
        {
            _conditions.cases.inventoryItemSwitchingIsBlocked = false;
            _conditions.cases.inventoryWantsToThrowItem = false;
            _itemThrowHoldTime = 0;
        }

        #endregion Throw

        #endregion Private methodes
        
        private void InteractableItemSlotGiveItem()
        {
            RemoveItemFromInventory(_dynamicReferences.currentSelectedSlot, insertingToSlot: true);
        }

        private void InteractableItemSlotGetItem(ItemBase itemTaken)
        {
            //Interactable checks if hand is empty before
            TryToPickUpItem(itemTaken);
        }

        #region Events

        private void SubscribeToEvents()
        {
            _input.OnPerformedInventoryScroll += UpdateScroll;

            _input.OnPerformedInventoryThrow += ItemThrowBegin;
            _input.OnCancelInventoryThrow += ItemThrowEnd;

            _input.OnPerformedInventoryNumpad1 += () => UpdateScrollNumpad(0);
            _input.OnPerformedInventoryNumpad2 += () => UpdateScrollNumpad(1);
            _input.OnPerformedInventoryNumpad3 += () => UpdateScrollNumpad(2);
            _input.OnPerformedInventoryNumpad4 += () => UpdateScrollNumpad(3);
            _input.OnPerformedInventoryNumpad5 += () => UpdateScrollNumpad(4);

            _input.OnPerformedInventoryActionMain += UseItemMainAction;
            _input.OnCancelInventoryActionMain += CancelItemMainAction;
            _input.OnPerformedInventoryActionSecondary += UseItemSecondaryAction;
            _input.OnCancelInventoryActionSecondary += CancelItemSecondaryAction;

            _dynamicReferences.OnTryingToPickupItem += TryToPickUpItem;
            _dynamicReferences.OnSelectedSlotChange += UpdateItemInHands;

            _dynamicReferences.OnTakeItemFromSlot += InteractableItemSlotGetItem;
            _dynamicReferences.OnInsertItemIntoSlot += InteractableItemSlotGiveItem;

            // _input.OnPerformedInventoryActionMain += 
            // _input.OnCancelInventoryActionMain += 

            // _input. OnPerformedInventoryActionSecondary += 
            // _input. OnCancelInventoryActionSecondary += 
        }


        public override void OnDestroy()
        {
            _input.OnPerformedInventoryScroll -= UpdateScroll;

            _input.OnPerformedInventoryThrow -= ItemThrowBegin;
            _input.OnCancelInventoryThrow -= ItemThrowEnd;

            _input.OnPerformedInventoryNumpad1 -= () => UpdateScrollNumpad(0);
            _input.OnPerformedInventoryNumpad2 -= () => UpdateScrollNumpad(1);
            _input.OnPerformedInventoryNumpad3 -= () => UpdateScrollNumpad(2);
            _input.OnPerformedInventoryNumpad4 -= () => UpdateScrollNumpad(3);
            _input.OnPerformedInventoryNumpad5 -= () => UpdateScrollNumpad(4);

            _input.OnPerformedInventoryActionMain -= UseItemMainAction;
            _input.OnCancelInventoryActionMain -= CancelItemMainAction;
            _input.OnPerformedInventoryActionSecondary -= UseItemSecondaryAction;
            _input.OnCancelInventoryActionSecondary -= CancelItemSecondaryAction;

            _dynamicReferences.OnTryingToPickupItem -= TryToPickUpItem;
            _dynamicReferences.OnSelectedSlotChange -= UpdateItemInHands;

            _dynamicReferences.OnTakeItemFromSlot -= InteractableItemSlotGetItem;
            _dynamicReferences.OnInsertItemIntoSlot -= InteractableItemSlotGiveItem;
        }

        #endregion Events
    }
}