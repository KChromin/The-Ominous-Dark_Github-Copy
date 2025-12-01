using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.Controllers.Interactions
{
    [DefaultExecutionOrder(20)]
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        #region Parameters

        [field: SerializeField]
        public bool CanBeUsed { get; set; } = true;

        [field: SerializeField]
        public string ObjectName { get; set; } = "Object";

        [field: SerializeField]
        public InteractionActionName InteractionName { get; set; }

        [field: SerializeField]
        public InteractionModes InteractionMode { get; set; }

        #region On Hold Options

        [HideInInspector, SerializeField]
        [Range(0f, 100f)]
        public float onHoldTimeToUseInSeconds = 1;

        [HideInInspector, SerializeField]
        public bool onHoldProgressDegradesWhileNotHolding = true;

        [HideInInspector, SerializeField]
        [Range(0f, 100f)]
        public float onHoldProgressDegradationEverySecond = 1;

        #endregion On Hold Options

        #endregion Parameters

        private bool _wasAlreadyActivated;

        public bool IsInteracting { get; private set; }
        public float InteractingTime { get; private set; }

        //Layer Mask for Outline//
        private int _interactableLayer;
        private int _interactableSelectedLayer;

        private bool _isSelected;


        #region Methodes

        public void BeginInteraction()
        {
            if (!CanBeUsed)
            {
                Debug.Log("Cannot be used!");
                return;
            }

            if (_wasAlreadyActivated) return;

            if (InteractionMode == InteractionModes.OnClick)
            {
                Interact();
                _wasAlreadyActivated = true;
            }
            else if (InteractionMode == InteractionModes.OnHold)
            {
                IsInteracting = true;
            }
        }

        public void EndInteraction()
        {
            if (InteractionMode == InteractionModes.OnHold)
            {
                IsInteracting = false;
            }
        }

        protected virtual void Interact()
        {
            Debug.Log("Interaction");
        }

        protected void Update()
        {
            if (!CanBeUsed) return;
            if (InteractionMode != InteractionModes.OnHold) return;
            if(_wasAlreadyActivated) return;

            //Is interacting
            if (IsInteracting)
            {
                InteractingTime += Time.deltaTime;

                if (InteractingTime >= onHoldTimeToUseInSeconds)
                {
                    Interact();
                    _wasAlreadyActivated = true;
                }
            }
            else if (onHoldProgressDegradesWhileNotHolding)
            {
                if (InteractingTime == 0) return;

                InteractingTime -= Time.deltaTime * onHoldProgressDegradationEverySecond;
                InteractingTime = Mathf.Clamp(InteractingTime, 0, 100);
            }
        }

        private void OnEnable()
        {
            _interactableLayer = LayerMask.NameToLayer("Interactable");
            _interactableSelectedLayer = LayerMask.NameToLayer("InteractableSelected");
            SetLayerMask(false);
        }

        private void OnDisable()
        {
            SetLayerMask(false);
        }

        //For outline effect
        public void SetObjectSelection(bool isSelected)
        {
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

            if (isSelected && currentLayer != _interactableSelectedLayer)
            {
                gameObject.layer = _interactableSelectedLayer;
            }
            else
            {
                gameObject.layer = _interactableLayer;
            }
        }

        #endregion Methodes
    }
}