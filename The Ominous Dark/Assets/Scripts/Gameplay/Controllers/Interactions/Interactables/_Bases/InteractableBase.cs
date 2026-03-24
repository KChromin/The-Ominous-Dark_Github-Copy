using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace NOS.Controllers.Interactions
{
    [DefaultExecutionOrder(20)]
    [SelectionBase]
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [field: Header("Interactable Settings"), SerializeField]
        public InteractableParametersClass InteractableSettings { get; set; }

        [field: Space]

        #region Variables

        private bool IsInteracting { get; set; }

        private float _interactingTime;

        protected int SelectedMaskLayer;
        protected int NoPlayerCollisionLayer;

        private bool _isSelected;

        protected bool UpdateSelectionEvenWhenCannotBeUsed = false;

        //For Selection//
        private Transform[] _allObjects;
        private int[] _allObjectsSavedLayerMasks;

        #endregion Variables

        #region Methodes

        public virtual string GetCurrentLocalizationText()
        {
            string currentLocalizationText;

            if (InteractableSettings.LocalizationParameters.UseCustomText)
            {
                currentLocalizationText = InteractableSettings.LocalizationParameters.CustomText.GetLocalizedString();
            }
            else
            {
                currentLocalizationText = InteractableSettings.LocalizationParameters.ActionName.GetLocalizedString() + " " + InteractableSettings.LocalizationParameters.ObjectName.GetLocalizedString();
            }

            return currentLocalizationText;
        }

        protected abstract void Interact();

        protected void EnableInteraction()
        {
            InteractableSettings.CanBeUsed = true;
        }

        protected void DisableInteraction()
        {
            InteractableSettings.CanBeUsed = false;
            SetObjectSelection(false);
        }

        #region OnHold

        public virtual void BeginInteraction()
        {
            if (!InteractableSettings.CanBeUsed)
            {
#if UNITY_EDITOR
                Debug.Log("Cannot be used!", this);
#endif
                return;
            }

            if (InteractableSettings.InteractionParameters == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Interactable parameters not set", this);
#endif
                return;
            }

            if (InteractableSettings.InteractionParameters.InteractionMode == InteractionModes.OnClick)
            {
                Interact();

                if (InteractableSettings.InteractionParameters.DisableAfterInteraction)
                {
                    DisableInteraction();
                }
            }
            else
            {
                IsInteracting = true;
            }
        }

        public void EndInteraction()
        {
            if (InteractableSettings.InteractionParameters?.InteractionMode != InteractionModes.OnClick)
            {
                IsInteracting = false;
            }
        }

        protected virtual void Update()
        {
            if (!InteractableSettings.CanBeUsed) return;
            if (InteractableSettings.InteractionParameters.InteractionMode != InteractionModes.OnHold && InteractableSettings.InteractionParameters.InteractionMode != InteractionModes.OnHoldProgressionOnly) return;

            //Is interacting
            if (IsInteracting)
            {
                InteractionHandling();
            }
            else if (_interactingTime != 0) //When not interacting, degrade progress until 0
            {
                if (InteractableSettings.InteractionParameters.OnHoldParameters.progressDegradation)
                {
                    _interactingTime -= Time.deltaTime * InteractableSettings.InteractionParameters.OnHoldParameters.progressDegradationAmountEverySecond;
                    _interactingTime = Mathf.Clamp(_interactingTime, 0, 100);
                }
                else if (InteractableSettings.InteractionParameters.InteractionMode == InteractionModes.OnHoldProgressionOnly)
                {
                    _interactingTime -= Time.deltaTime;
                    _interactingTime = Mathf.Clamp(_interactingTime, 0, 100);
                }
                else //When progress should not degrade over time, reset instantly
                {
                    _interactingTime = 0;
                }
            }
        }

        private void InteractionHandling()
        {
            _interactingTime += Time.deltaTime;

            //No interaction, only progression//
            if (InteractableSettings.InteractionParameters.InteractionMode == InteractionModes.OnHoldProgressionOnly)
            {
                _interactingTime = Mathf.Clamp(_interactingTime, 0, InteractableSettings.InteractionParameters.OnHoldParameters.timeBeforeUsingInSeconds);
            }
            else if (_interactingTime >= InteractableSettings.InteractionParameters.OnHoldParameters.timeBeforeUsingInSeconds) //Default Interaction
            {
                //Normal Interaction//
                Interact();

                if (InteractableSettings.InteractionParameters.DisableAfterInteraction)
                {
                    DisableInteraction();

                    //Make sure that after trigger progression is full// 
                    _interactingTime = InteractableSettings.InteractionParameters.OnHoldParameters.timeBeforeUsingInSeconds;
                }
                else
                {
                    ResetTimer();
                }

                ResetInteracting();
            }
        }

        private void ResetTimer()
        {
            _interactingTime = 0;
        }

        private void ResetInteracting()
        {
            IsInteracting = false;
        }

        #endregion OnHold

        #region Outline

        //For outline effect
        public void SetObjectSelection(bool isSelected)
        {
            if (!InteractableSettings.CanBeUsed && !UpdateSelectionEvenWhenCannotBeUsed)
            {
                isSelected = false;
            }

            //If state is the same, there is no need to update//
            if (_isSelected == isSelected) return;

            if (isSelected)
            {
                _isSelected = true;
                SetLayerMask(true);
            }
            else
            {
                _isSelected = false;
                SetLayerMask(false);
            }
        }

        private void SetLayerMask(bool isSelected)
        {
            int currentLayer = gameObject.layer;

            if (isSelected && currentLayer != SelectedMaskLayer)
            {
                SetLayerMasks(LayerMaskSetOptions.Select);
            }
            else
            {
                SetLayerMasks(LayerMaskSetOptions.Default);
            }
        }

        protected virtual void SetLayerMasks(LayerMaskSetOptions option)
        {
            switch (option)
            {
                default:
                case LayerMaskSetOptions.Default:
                    for (int i = 0; i < _allObjects.Length; i++)
                    {
                        if (_allObjects[i].CompareTag("SoundInstance"))
                            continue;

                        _allObjects[i].gameObject.layer = _allObjectsSavedLayerMasks[i];
                    }

                    break;
                case LayerMaskSetOptions.Select:
                    for (int i = 0; i < _allObjects.Length; i++)
                    {
                        if (_allObjects[i].CompareTag("SoundInstance"))
                            continue;

                        _allObjects[i].gameObject.layer = SelectedMaskLayer;
                    }

                    break;
                case LayerMaskSetOptions.NoPlayerCollision:
                    for (int i = 0; i < _allObjects.Length; i++)
                    {
                        if (_allObjects[i].CompareTag("SoundInstance"))
                            continue;

                        _allObjects[i].gameObject.layer = NoPlayerCollisionLayer;
                    }

                    break;
            }
        }

        protected enum LayerMaskSetOptions
        {
            Default,
            Select,
            NoPlayerCollision,
        }

        #endregion Outline

        #region OnEnable OnDisable

        private void OnEnable()
        {
            NoPlayerCollisionLayer = LayerMask.NameToLayer("NoPlayerCollision");

            _allObjects = GetComponentsInChildren<Transform>();

            _allObjectsSavedLayerMasks = new int[_allObjects.Length];

            for (int i = 0; i < _allObjects.Length; i++)
            {
                _allObjectsSavedLayerMasks[i] = _allObjects[i].gameObject.layer;
            }

            SelectedMaskLayer = InteractableSettings.SelectionLayers[InteractableSettings.SelectionLayer];
            SetLayerMask(false);
        }

        protected virtual void OnDisable()
        {
            SetLayerMask(false);
        }

        #endregion OnEnable OnDisable

        public float GetCurrentProgress()
        {
            return _interactingTime / InteractableSettings.InteractionParameters.OnHoldParameters.timeBeforeUsingInSeconds;
        }

        #endregion Methodes
    }

    [Serializable]
    public class InteractableParametersClass
    {
        //If it can be interacted with
        [field: SerializeField]
        public bool CanBeUsed { get; set; } = true;

        //Localization Settings//
        [field: Space, SerializeField]
        public InteractableLocalizationSettingsClass LocalizationParameters { get; set; } = new();

        //Interaction Settings//
        [field: Space, SerializeField]
        public InteractableInteractionScriptableObject InteractionParameters { get; set; }

        [field: Space, SerializeField]
        public InteractableSelectionLayers SelectionLayer { get; set; }

        public Dictionary<InteractableSelectionLayers, int> SelectionLayers { get; set; } = new()
        {
            { InteractableSelectionLayers.Interactable, 7 },
            { InteractableSelectionLayers.Item, 10 }
        };
    }

    public enum InteractableSelectionLayers
    {
        Interactable,
        Item
    }

    [Serializable]
    public class InteractableLocalizationSettingsClass
    {
        [field: Header("Simple")]
        [field: SerializeField]
        public LocalizedString ObjectName { get; set; } = new();

        [field: SerializeField]
        public LocalizedString ActionName { get; set; } = new();

        //Custom Text
        //Used instead of simple pattern
        [field: Header("Custom")]
        [field: SerializeField]
        public bool UseCustomText { get; set; } = false;

        [field: SerializeField]
        public LocalizedString CustomText { get; set; } = new();
    }

    [Serializable]
    public class InteractableOnHoldSettingsClass
    {
        [Header("Time of holding before interaction")]
        [Range(0f, 100f)]
        public float timeBeforeUsingInSeconds = 0.4f;

        //Degradation
        [Header("Progress degradation while not holding")]
        public bool progressDegradation = false;

        [Range(0f, 100f)]
        public float progressDegradationAmountEverySecond = 1;
    }

    public interface IInteractable
    {
        //If it can be interacted with
        public InteractableParametersClass InteractableSettings { get; set; }

        //Player Starts Interacting//
        public void BeginInteraction();

        //Player Stops Interacting//
        public void EndInteraction();
    }

    public enum InteractionModes
    {
        OnClick,
        OnHold,
        OnHoldProgressionOnly
    }
}