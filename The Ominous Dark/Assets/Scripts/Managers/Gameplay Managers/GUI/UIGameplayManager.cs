using NOS.Controllers.Interactions;
using NOS.Item;
using NOS.Patterns.Singleton;
using NOS.Player;
using NOS.Player.Data;
using NOS.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NOS.GameplayManagers
{
    [DefaultExecutionOrder(-35)]
    public class UIGameplayManager : Singleton<UIGameplayManager>
    {
        #region Variables

        private UIDocument _gameplayUIDocument;
        private VisualElement _root;

        #region Croshair Area

        //Interaction
        private UIElementRadialFill _crossHairInteractionProgress;
        private VisualElement _crossHairInteractionOutline;
        private Label _interactionText;
        private ProgressBar _throwProgressBar;

        #endregion Croshair Area

        #region Inventory Area

        private VisualElement _inventoryItemSlot0;
        private VisualElement _inventoryItemSlotImage0;

        private VisualElement _inventoryItemSlot1;
        private VisualElement _inventoryItemSlotImage1;

        private VisualElement _inventoryItemSlot2;
        private VisualElement _inventoryItemSlotImage2;

        private VisualElement _inventoryItemSlot3;
        private VisualElement _inventoryItemSlotImage3;

        private VisualElement _inventoryItemSlot4;
        private VisualElement _inventoryItemSlotImage4;

        private Color _inventorySlotDefaultSelectionColor;
        private readonly Color _inventorySlotDefaultSelectionColorSelected = Color.orangeRed;

        #endregion Inventory Area

        #region Held Item Area

        private VisualElement _inventoryHeldItemArea;
        private Label _inventoryHeldItemName;
        private VisualElement _inventoryMainActionItemArea;
        private Label _inventoryHeldItemMainActionDescription;
        private VisualElement _inventorySecondaryActionItemArea;
        private Label _inventoryHeldItemSecondaryActionDescription;

        #endregion Held Item Area

        #region Stamina

        private ProgressBar _staminaBar;

        #endregion Stamina

        private PlayerDynamicReferences _dynamicReferences;

        #endregion Variables

        protected override void Awake()
        {
            base.Awake();

            _gameplayUIDocument = GetComponent<UIDocument>();
            _root = _gameplayUIDocument.rootVisualElement;

            #region CrossHair

            _interactionText = _root.Q<Label>("InteractionText");
            _interactionText.visible = false;
            _crossHairInteractionProgress = _root.Q<UIElementRadialFill>("CrossHairInteractionProgress");
            _crossHairInteractionProgress.visible = false;
            _crossHairInteractionOutline = _root.Q<VisualElement>("CrossHairInteractionOutline");
            _crossHairInteractionProgress.visible = false;
            _throwProgressBar = _root.Q<ProgressBar>("CrossHairThrowIndicator");
            _throwProgressBar.style.display = DisplayStyle.None;

            #endregion CrossHair

            #region Inventory

            _inventoryItemSlot0 = _root.Q<VisualElement>("InventoryItemSlot0");
            _inventoryItemSlotImage0 = _root.Q<VisualElement>("InventoryItemSlotImage0");

            _inventoryItemSlot1 = _root.Q<VisualElement>("InventoryItemSlot1");
            _inventoryItemSlotImage1 = _root.Q<VisualElement>("InventoryItemSlotImage1");

            _inventoryItemSlot2 = _root.Q<VisualElement>("InventoryItemSlot2");
            _inventoryItemSlotImage2 = _root.Q<VisualElement>("InventoryItemSlotImage2");

            _inventoryItemSlot3 = _root.Q<VisualElement>("InventoryItemSlot3");
            _inventoryItemSlotImage3 = _root.Q<VisualElement>("InventoryItemSlotImage3");

            _inventoryItemSlot4 = _root.Q<VisualElement>("InventoryItemSlot4");
            _inventoryItemSlotImage4 = _root.Q<VisualElement>("InventoryItemSlotImage4");

            _inventorySlotDefaultSelectionColor = _inventoryItemSlot0.style.unityBackgroundImageTintColor.value;

            #endregion Inventory

            #region Held Item Area

            _inventoryHeldItemArea = _root.Q<VisualElement>("InventoryHeldItemArea");
            _inventoryHeldItemName = _root.Q<Label>("InventoryHeldItemName");
            _inventoryMainActionItemArea = _root.Q<VisualElement>("InventoryMainActionContainer");
            _inventoryHeldItemMainActionDescription = _root.Q<Label>("InventoryHeldItemMainActionDescription");
            _inventorySecondaryActionItemArea = _root.Q<VisualElement>("InventorySecondaryActionContainer");
            _inventoryHeldItemSecondaryActionDescription = _root.Q<Label>("InventoryHeldItemSecondaryActionDescription");

            #endregion Held Item Area

            #region Stamina

            _staminaBar = _root.Q<ProgressBar>("StaminaBar");

            #endregion Stamina
        }

        private void Start()
        {
            _dynamicReferences = GameObject.FindWithTag("Player").GetComponent<PlayerMain>().GetPlayerDynamicReferences();
            _dynamicReferences.Default.InteractionDefault.OnSelectObjectChange += OnInteractionUIRefresh;

            //Stamina
            _dynamicReferences.Default.StaminaDefault.OnStaminaValueChange += SetStaminaBar;

            //Inventory
            _dynamicReferences.Default.InventoryDefault.OnSelectedSlotChange += InventoryUpdateCurrentSlot;
            _dynamicReferences.Default.InventoryDefault.OnSelectedSlotChange += () => InventoryHeldItemDescription(_dynamicReferences.Default.InventoryDefault.currentSelectedSlot);
            _dynamicReferences.Default.InventoryDefault.OnPickedUpItemGUINotice += InventorySetItemImageToSlot;
            _dynamicReferences.Default.InventoryDefault.OnPickedUpItemGUINotice += InventoryHeldItemDescription;
            _dynamicReferences.Default.InventoryDefault.OnDroppedItemGUINotice += InventoryRemoveItemImageFromSlot;
            _dynamicReferences.Default.InventoryDefault.OnDroppedItemGUINotice += InventoryHeldItemDescription;

            _dynamicReferences.Default.InventoryDefault.OnThrowItemGUINotice += SetThrowPower;
            _dynamicReferences.Default.InventoryDefault.OnEndThrowItemGUINotice += ResetThrowPower;

            InventoryUpdateCurrentSlot();
            InventoryHeldItemDescription(0);
        }

        private void Update()
        {
            SetInteractionProgress();
        }

        private void OnDisable()
        {
            _dynamicReferences.Default.InteractionDefault.OnSelectObjectChange -= OnInteractionUIRefresh;

            //Stamina
            _dynamicReferences.Default.StaminaDefault.OnStaminaValueChange -= SetStaminaBar;
            
            _dynamicReferences.Default.InventoryDefault.OnSelectedSlotChange -= InventoryUpdateCurrentSlot;
            _dynamicReferences.Default.InventoryDefault.OnSelectedSlotChange -= () => InventoryHeldItemDescription(_dynamicReferences.Default.InventoryDefault.currentSelectedSlot);
            _dynamicReferences.Default.InventoryDefault.OnPickedUpItemGUINotice -= InventorySetItemImageToSlot;
            _dynamicReferences.Default.InventoryDefault.OnPickedUpItemGUINotice -= InventoryHeldItemDescription;
            _dynamicReferences.Default.InventoryDefault.OnDroppedItemGUINotice -= InventoryRemoveItemImageFromSlot;
            _dynamicReferences.Default.InventoryDefault.OnDroppedItemGUINotice -= InventoryHeldItemDescription;

            _dynamicReferences.Default.InventoryDefault.OnThrowItemGUINotice -= SetThrowPower;
            _dynamicReferences.Default.InventoryDefault.OnEndThrowItemGUINotice -= ResetThrowPower;
        }

        #region Interaction

        private void OnInteractionUIRefresh()
        {
            SetInteractionText();
            SetInteractionIndicator();
        }

        private void SetInteractionProgress()
        {
            if (_dynamicReferences.Default.InteractionDefault?.currentSelectedInteractableObject && _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.InteractableSettings.CanBeUsed &&
                _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.InteractableSettings.InteractionParameters.InteractionMode != InteractionModes.OnClick)
            {
                _crossHairInteractionProgress.visible = true;
                _crossHairInteractionProgress.FillAmount = _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.GetCurrentProgress();
            }
            else
            {
                _crossHairInteractionProgress.visible = false;
                _crossHairInteractionProgress.FillAmount = 0;
            }
        }

        private void SetInteractionText()
        {
            if (_dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject && _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.InteractableSettings.CanBeUsed)
            {
                _interactionText.text = _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.GetCurrentLocalizationText();
                _interactionText.visible = true;
            }
            else
            {
                _interactionText.visible = false;
                _interactionText.text = "";
            }
        }

        private void SetInteractionIndicator()
        {
            _crossHairInteractionOutline.visible = _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject && _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.InteractableSettings.CanBeUsed;
        }

        #endregion Interaction

        #region Inventory

        #region Slots

        private void InventoryUpdateCurrentSlot()
        {
            InventoryDisableAllSlotsSelection();
            InventoryUpdateSelection();
        }

        private void InventoryDisableAllSlotsSelection()
        {
            _inventoryItemSlot0.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColor;
            _inventoryItemSlot1.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColor;
            _inventoryItemSlot2.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColor;
            _inventoryItemSlot3.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColor;
            _inventoryItemSlot4.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColor;
        }

        private void InventoryUpdateSelection()
        {
            int itemSlotNumber = _dynamicReferences.Default.InventoryDefault.currentSelectedSlot;

            switch (itemSlotNumber)
            {
                default:
                case 0:
                    _inventoryItemSlot0.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColorSelected;
                    return;
                case 1:
                    _inventoryItemSlot1.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColorSelected;
                    return;
                case 2:
                    _inventoryItemSlot2.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColorSelected;
                    return;
                case 3:
                    _inventoryItemSlot3.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColorSelected;
                    return;
                case 4:
                    _inventoryItemSlot4.style.unityBackgroundImageTintColor = _inventorySlotDefaultSelectionColorSelected;
                    return;
            }
        }

        private void InventorySetItemImageToSlot(int slotNumber)
        {
            Texture2D newSprite = _dynamicReferences.Default.InventoryDefault.currentInventoryItems[slotNumber].itemParameters.itemEquipmentPicture;

            switch (slotNumber)
            {
                default:
                case 0:
                    _inventoryItemSlotImage0.style.backgroundImage = newSprite;
                    return;
                case 1:
                    _inventoryItemSlotImage1.style.backgroundImage = newSprite;
                    return;
                case 2:
                    _inventoryItemSlotImage2.style.backgroundImage = newSprite;
                    return;
                case 3:
                    _inventoryItemSlotImage3.style.backgroundImage = newSprite;
                    return;
                case 4:
                    _inventoryItemSlotImage4.style.backgroundImage = newSprite;
                    return;
            }
        }

        private void InventoryRemoveItemImageFromSlot(int slotNumber)
        {
            switch (slotNumber)
            {
                default:
                case 0:
                    _inventoryItemSlotImage0.style.backgroundImage = null;
                    return;
                case 1:
                    _inventoryItemSlotImage1.style.backgroundImage = null;
                    return;
                case 2:
                    _inventoryItemSlotImage2.style.backgroundImage = null;
                    return;
                case 3:
                    _inventoryItemSlotImage3.style.backgroundImage = null;
                    return;
                case 4:
                    _inventoryItemSlotImage4.style.backgroundImage = null;
                    return;
            }
        }

        #endregion Slots

        #region Held Item

        private void InventoryHeldItemDescription(int itemSlot)
        {
            ItemBase currentItem = _dynamicReferences.Default.InventoryDefault.currentInventoryItems[itemSlot];

            if (!currentItem)
            {
                _inventoryHeldItemArea.style.display = DisplayStyle.None;
                return;
            }

            _inventoryHeldItemArea.style.display = DisplayStyle.Flex;

            //Update name
            _inventoryHeldItemName.text = currentItem.itemParameters.itemName.GetLocalizedString();

            if (currentItem.itemParameters.itemMainActionInteractionType == ItemActionType.Disabled)
            {
                _inventoryMainActionItemArea.style.display = DisplayStyle.None;
            }
            else
            {
                _inventoryMainActionItemArea.style.display = DisplayStyle.Flex;
                _inventoryHeldItemMainActionDescription.text = currentItem.itemParameters.itemMainActionDescription.GetLocalizedString();
            }

            if (currentItem.itemParameters.itemSecondaryActionInteractionType == ItemActionType.Disabled)
            {
                _inventorySecondaryActionItemArea.style.display = DisplayStyle.None;
            }
            else
            {
                _inventorySecondaryActionItemArea.style.display = DisplayStyle.Flex;
                _inventoryHeldItemSecondaryActionDescription.text = currentItem.itemParameters.itemSecondaryActionDescription.GetLocalizedString();
            }
        }

        #endregion Held Item


        private void SetThrowPower(float throwProgress01)
        {
            _throwProgressBar.style.display = DisplayStyle.Flex;

            if (throwProgress01 >= 0.05f)
            {
                _throwProgressBar.value = throwProgress01;
            }
        }

        private void ResetThrowPower()
        {
            _throwProgressBar.style.display = DisplayStyle.None;
            _throwProgressBar.value = 0;
        }

        #endregion Inventory

        private void SetStaminaBar(float currentStamina01)
        {
            _staminaBar.value = currentStamina01;
        }
    }
}