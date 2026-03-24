using System;
using Cysharp.Threading.Tasks;
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
        public PlayerControllerDefaultInventory(InputDataContainer input, PlayerConditions conditions, PlayerReferences references, PlayerValues values, PlayerDynamicReferences dynamicReferences, SettingsManager settingsManager)
        {
            _input = input;
            _conditions = conditions.Default;
            _values = values;
            _parameters = references.ScriptableObjects.Default.inventory;
            _itemSlot = references.Objects.handsItemSlot.transform;
            _dynamicReferences = dynamicReferences.Default.InventoryDefault;
            _settingsManager = settingsManager;

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
        private readonly SettingsManager _settingsManager;

        private const int InventorySize = 5;

        private int _previousSelectedItem;
        private float _itemThrowHoldTime;
        private float _throwForce01;

        //Transition
        //Define things that must be done//
        private bool _executeTransitionHide;
        private bool _executeTransitionPullOut;

        private Transform _transitioningItem;
        private Vector3 _currentTransitioningItemCalculations;

        #endregion Variables

        public override void Update()
        {
            ItemThrowUpdate();
            UpdateItemInHandsTransitionExecution();
        }

        #region Private methodes

        #region Add, Remove Item

        private void TryToPickUpItem(ItemBase itemToAdd)
        {
            //Try to add item to current slot
            if (!_dynamicReferences.GetCurrentItem())
            {
                AddItemToInventory(itemToAdd, _dynamicReferences.currentSelectedSlot);
                _dynamicReferences.OnSelectedSlotChange?.Invoke(); //Refresh hand after picking item//
                UpdateItemInHands();
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

        #endregion Add, Remove Item

        #region Changing Items

        private void UpdateScroll(Vector2 scrollDelta)
        {
            if (_conditions.cases.inventoryItemSwitchingIsBlocked) return;
            if (_conditions.cases.inventoryIsTransitioningItems) return;

            _previousSelectedItem = _dynamicReferences.currentSelectedSlot;
            
            //When next
            if (scrollDelta.y > 0)
            {
                _dynamicReferences.currentSelectedSlot++;

                //Loop
                if (_dynamicReferences.currentSelectedSlot > _dynamicReferences.currentInventoryItems.Length - 1)
                {
                    if (_settingsManager.CurrentSettings.game.InventoryItemScrollLoopsOnEnds)
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
                    if (_settingsManager.CurrentSettings.game.InventoryItemScrollLoopsOnEnds)
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
            if (_conditions.cases.inventoryIsTransitioningItems) return;

            _previousSelectedItem = _dynamicReferences.currentSelectedSlot;
            _dynamicReferences.currentSelectedSlot = selectedNumber;

            //Here Update Item//
            _dynamicReferences.OnSelectedSlotChange?.Invoke();
        }

        private void UpdateItemInHands()
        {
            if (_conditions.cases.inventoryIsTransitioningItems) return;
            if (_previousSelectedItem == _dynamicReferences.currentSelectedSlot && (_dynamicReferences.currentInventoryItems[_previousSelectedItem] && _dynamicReferences.currentInventoryItems[_previousSelectedItem].gameObject.activeSelf)) return;
            
            _conditions.cases.inventoryIsTransitioningItems = true;

            //Hide current item
            if (_dynamicReferences.currentInventoryItems[_previousSelectedItem])
            {
                _executeTransitionHide = true;
            }

            //Pull up new
            if (_dynamicReferences.GetCurrentItem())
            {
                _executeTransitionPullOut = true;
            }
        }

        private void UpdateItemInHandsTransitionExecution()
        {
            if (!_conditions.cases.inventoryIsTransitioningItems) return;

            //First hide thing in hand
            if (_executeTransitionHide)
            {
                if (!_transitioningItem)
                {
                    _transitioningItem = _dynamicReferences.currentInventoryItems[_previousSelectedItem].transform;
                }

                //Transition
                if (_transitioningItem.localPosition != _parameters.itemHideOffset)
                {
                    TransitItem(_parameters.itemHideOffset, _parameters.itemHidingTransitionSpeed);
                }
                else
                {
                    DeactivateItemInHand(_previousSelectedItem);
                    _transitioningItem = null;
                    _executeTransitionHide = false;
                }
            }
            else if (_executeTransitionPullOut) //Next try to bring new thing//
            {
                if (!_transitioningItem)
                {
                    _transitioningItem = _dynamicReferences.GetCurrentItem().transform;
                    _dynamicReferences.GetCurrentItem().transform.localPosition = _parameters.itemHideOffset;
                    ActivateItemInHand(_dynamicReferences.currentSelectedSlot);
                }

                if (_transitioningItem.localPosition != Vector3.zero)
                {
                    TransitItem(Vector3.zero, _parameters.itemPullOutTransitionSpeed);
                }
                else
                {
                    _transitioningItem = null;
                    _executeTransitionPullOut = false;
                }
            }
            else //When all done, reset//
            {
                _conditions.cases.inventoryIsTransitioningItems = false;
            }
        }

        private void TransitItem(Vector3 targetLocalPosition, float speed)
        {
            _transitioningItem.localPosition = Vector3.SmoothDamp(_transitioningItem.localPosition, targetLocalPosition, ref _currentTransitioningItemCalculations, speed, Mathf.Infinity, Time.deltaTime);

            //When close enough round value//
            if (Mathf.Approximately(_transitioningItem.localPosition.x, targetLocalPosition.x) && Mathf.Approximately(_transitioningItem.localPosition.y, targetLocalPosition.y) &&
                Mathf.Approximately(_transitioningItem.localPosition.z, targetLocalPosition.z))
            {
                _transitioningItem.localPosition = targetLocalPosition;
                //Reset calculation value//
                _currentTransitioningItemCalculations = Vector3.zero;
            }
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

        #endregion Changing Items

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
            if (_conditions.cases.inventoryIsTransitioningItems) return;

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

        #region Interactable slot

        private void InteractableItemSlotGiveItem()
        {
            RemoveItemFromInventory(_dynamicReferences.currentSelectedSlot, insertingToSlot: true);
        }

        private void InteractableItemSlotGetItem(ItemBase itemTaken)
        {
            //Interactable checks if hand is empty before
            TryToPickUpItem(itemTaken);
        }

        #endregion Interactable slot

        #endregion Private methodes
        
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