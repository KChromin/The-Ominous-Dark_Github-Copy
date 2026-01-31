using System;
using Cysharp.Threading.Tasks;
using NOS.Controllers.Interactions;
using NOS.GameplayManagers;
using NOS.Player.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace NOS.Item
{
    public abstract class ItemBase : InteractableBase
    {
        #region Variables

        [Header("Item is currently active in hands")]
        public bool currentlyInHands;

        [Header("Item Parameters")]
        public ItemScriptableObject itemParameters;

        private PlayerDynamicReferences.DefaultDynamicReferencesClass.InventoryDefaultClass _dynamicReferences;

        private Rigidbody[] _rigidbody;

        public Action<GameObject> OnDisableItem;

        private struct ItemRigidbodySettingsSaveStruct
        {
            public ItemRigidbodySettingsSaveStruct(bool isKinematic, RigidbodyInterpolation interpolationType)
            {
                IsKinematic = isKinematic;
                Interpolation = interpolationType;
            }

            public bool IsKinematic { get; private set; }
            public RigidbodyInterpolation Interpolation { get; private set; }
        }

        private ItemRigidbodySettingsSaveStruct[] _rigidbodyStartSettings;

        private Collider[] _collider;
        private bool[] _colliderStartTriggerState;

        private MeshRenderer[] _renderers;
        private ShadowCastingMode[] _renderersShadowModes;

        private float _currentSizeResetTime;
        private const float SizeResetTimeInSecondsMinForce = 0.15f;
        private const float SizeResetTimeInSecondsMaxForce = 0.02f;
        private const float SizeResetRoundingValue = 0.999f;
        private const float PlayerCollisionDisableTime = 0.42f;

        private bool _needResetToCorrectSize;
        private Vector3 _smoothResetCalculations;

        private bool _isPerformingMainAction;
        private bool _isPerformingSecondaryAction;

        #endregion Variables

        #region Private Methodes

        private void Awake()
        {
            _dynamicReferences = GameplayReferenceManager.Instance.PlayerDynamicReferences.Default.InventoryDefault;

            SetupComponentsAndSaveStartValues();
        }

        private void SetupComponentsAndSaveStartValues()
        {
            #region Rigidbody

            _rigidbody = GetComponentsInChildren<Rigidbody>();
            _rigidbodyStartSettings = new ItemRigidbodySettingsSaveStruct[_rigidbody.Length];

            for (int i = 0; i < _rigidbodyStartSettings.Length; i++)
            {
                _rigidbodyStartSettings[i] = new ItemRigidbodySettingsSaveStruct(_rigidbody[i].isKinematic, _rigidbody[i].interpolation);
            }

            #endregion Rigidbody

            #region Colliders

            _collider = GetComponentsInChildren<Collider>();
            _colliderStartTriggerState = new bool [_collider.Length];

            for (int i = 0; i < _colliderStartTriggerState.Length; i++)
            {
                _colliderStartTriggerState[i] = _collider[i].isTrigger;
            }

            #endregion Colliders

            #region MeshRenderers

            _renderers = GetComponentsInChildren<MeshRenderer>();
            _renderersShadowModes = new ShadowCastingMode[_renderers.Length];
            for (int i = 0; i < _renderersShadowModes.Length; i++)
            {
                _renderersShadowModes[i] = _renderers[i].shadowCastingMode;
            }

            #endregion MeshRenderers
        }

        protected override void Interact()
        {
            _dynamicReferences.OnTryingToPickupItem?.Invoke(this);
        }

        private void LateUpdate()
        {
            if (_needResetToCorrectSize)
            {
                transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref _smoothResetCalculations, _currentSizeResetTime);

                if (transform.localScale is { x: >= SizeResetRoundingValue, y: >= SizeResetRoundingValue, z: >= SizeResetRoundingValue })
                {
                    transform.localScale = Vector3.one;
                    _needResetToCorrectSize = false;
                }
            }
        }

        private async UniTaskVoid DisablePlayerCollisions(float disableTimeInSeconds)
        {
            SetLayerMasks(LayerMaskSetOptions.NoPlayerCollision);
            await UniTask.WaitForSeconds(disableTimeInSeconds);
            SetLayerMasks(LayerMaskSetOptions.Default);
        }

        private void SetComponentActivityStates(bool setActive)
        {
            if (setActive)
            {
                //Enable Rigidbodies
                for (int i = 0; i < _rigidbody.Length; i++)
                {
                    _rigidbody[i].isKinematic = _rigidbodyStartSettings[i].IsKinematic;
                    _rigidbody[i].interpolation = _rigidbodyStartSettings[i].Interpolation;
                }

                //Enable Colliders
                for (int i = 0; i < _collider.Length; i++)
                {
                    _collider[i].isTrigger = _colliderStartTriggerState[i];
                }

                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].shadowCastingMode = _renderersShadowModes[i];
                }
            }
            else
            {
                //Disable Rigidbodies
                for (int i = 0; i < _rigidbody.Length; i++)
                {
                    _rigidbody[i].isKinematic = true;
                    _rigidbody[i].interpolation = RigidbodyInterpolation.None;
                }

                //Disable Colliders
                for (int i = 0; i < _collider.Length; i++)
                {
                    _collider[i].isTrigger = true;
                }

                //Disable Shadows
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].shadowCastingMode = ShadowCastingMode.Off;
                }
            }
        }

        public void PerformMainAction()
        {
            if (_isPerformingSecondaryAction) return;

            switch (itemParameters.itemMainActionInteractionType)
            {
                default:
                case ItemActionType.Disabled:
                    return;
                case ItemActionType.OnClick:
                    ExecuteMainAction();
                    break;
                case ItemActionType.OnHold:
                    _isPerformingMainAction = true;
                    ExecuteMainAction();
                    break;
            }
        }

        public void PerformSecondaryAction()
        {
            if (_isPerformingMainAction) return;

            switch (itemParameters.itemSecondaryActionInteractionType)
            {
                default:
                case ItemActionType.Disabled:
                    return;
                case ItemActionType.OnClick:
                    ExecuteSecondaryAction();
                    break;
                case ItemActionType.OnHold:
                    _isPerformingSecondaryAction = true;
                    ExecuteSecondaryAction();
                    break;
            }
        }

        protected abstract void ExecuteMainAction();
        protected abstract void ExecuteSecondaryAction();

        public virtual void CancelMainAction()
        {
            if (itemParameters.itemMainActionInteractionType == ItemActionType.OnHold)
            {
                _isPerformingSecondaryAction = false;
            }
        }

        public virtual void CancelSecondaryAction()
        {
            if (itemParameters.itemSecondaryActionInteractionType == ItemActionType.OnHold)
            {
                _isPerformingSecondaryAction = false;
            }
        }

        protected virtual void DisableItem()
        {
        }

        protected override void OnDisable()
        {
            base.OnDisable();
           OnDisableItem?.Invoke(gameObject);
        }

        #endregion Private Methodes

        #region Public Methodes

        public void SetItemInHandsActivity(bool newState)
        {
            currentlyInHands = newState;
        }

        //When inventory adds this item//
        public void OnBeingAddedToInventory()
        {
            //When picked up it is not usable
            InteractableSettings.CanBeUsed = false;
            _needResetToCorrectSize = false;
            UpdateSelectionEvenWhenCannotBeUsed = false;
            SetComponentActivityStates(false);
        }

        public void OnBeingRemovedFromInventory(float throwForce01)
        {
            //When dropped it is again
            InteractableSettings.CanBeUsed = true;

            SetComponentActivityStates(true);

            _currentSizeResetTime = Mathf.Lerp(SizeResetTimeInSecondsMinForce, SizeResetTimeInSecondsMaxForce, throwForce01);
            _needResetToCorrectSize = true;

            DisablePlayerCollisions(PlayerCollisionDisableTime).Forget();
        }

        public void OnBeingInsertedToSlot()
        {
            //When dropped it is again
            DisableItem();
            InteractableSettings.CanBeUsed = false;
            SetComponentActivityStates(false);
            _needResetToCorrectSize = false;
            UpdateSelectionEvenWhenCannotBeUsed = true;
        }
        
        #endregion Public Methodes
    }

    public enum ItemActionType
    {
        Disabled,
        OnClick,
        OnHold
    }
}