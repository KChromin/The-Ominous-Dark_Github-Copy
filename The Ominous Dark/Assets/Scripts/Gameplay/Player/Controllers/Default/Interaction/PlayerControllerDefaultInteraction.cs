using System;
using NOS.Controllers.Interactions;
using NOS.GameManagers.Input;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    public class PlayerControllerDefaultInteraction : ControllerBase
    {
        public PlayerControllerDefaultInteraction(InputDataContainer input, PlayerConditions conditions, PlayerReferences references, PlayerDynamicReferences dynamicReferences)
        {
            _input = input;
            _conditions = conditions.Default;
            _parameters = references.ScriptableObjects.Default.interaction;
            _headPivot = references.Objects.headPivot.transform;
            _dynamicReferences = dynamicReferences;
            SubscribeToEvents();
        }

        private readonly InputDataContainer _input;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly PlayerControllerDefaultInteractionScriptableObject _parameters;
        private readonly Transform _headPivot;
        private readonly PlayerDynamicReferences _dynamicReferences;

        public override void Update()
        {
            if (!_conditions.possibilities.canInteract)
            {
                _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject = null;
                return;
            }

            InteractionRaycast();
        }

        private string _interactionLocalization;

        private void InteractionRaycast()
        {
            if (Physics.Raycast(_headPivot.position, _headPivot.forward, out _dynamicReferences.Default.InteractionDefault.Hit, _parameters.interactionDistance, _parameters.interactableLayers, QueryTriggerInteraction.Collide))
            {
                InteractableBase objectSelectedThisFrame = _dynamicReferences.Default.InteractionDefault.Hit.collider?.GetComponent<InteractableBase>();
                
                if (_dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject != objectSelectedThisFrame)
                {
                    if (_dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject)
                    {
                        _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.SetObjectSelection(false);
                        _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.EndInteraction();
                    }
                }
                
                
                _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject = objectSelectedThisFrame;
                _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject?.SetObjectSelection(true);
                
                //Invoke Event for UI refresh//
                _dynamicReferences.Default.InteractionDefault.OnSelectObjectChange?.Invoke();
            }
            else
            {
                if (_dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject)
                {
                    _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.SetObjectSelection(false);
                    _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.EndInteraction();
                    _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject = null;

                    //Invoke Event for UI refresh//
                    _dynamicReferences.Default.InteractionDefault.OnSelectObjectChange?.Invoke();
                }

                _conditions.cases.isInteracting = false;
            }
        }

        private void TryToInteract()
        {
            if (!_conditions.possibilities.canInteract)
            {
#if UNITY_EDITOR
                Debug.Log("Cannot interact");
#endif
                return;
            }

            if (_dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject)
            {
                _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.BeginInteraction();
                _conditions.cases.isInteracting = true;
            }
        }

        private void CancelInteractionAction()
        {
            if (_dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject)
            {
                _dynamicReferences.Default.InteractionDefault.currentSelectedInteractableObject.EndInteraction();
                _conditions.cases.isInteracting = false;
            }
        }

        #region Events

        private void SubscribeToEvents()
        {
            _input.OnPerformedInteract += TryToInteract;
            _input.OnCancelInteract += CancelInteractionAction;
        }

        public override void OnDestroy()
        {
            _input.OnPerformedInteract -= TryToInteract;
            _input.OnCancelInteract -= CancelInteractionAction;
        }

        #endregion Events
    }
}