using NOS.Controllers.Interactions;
using NOS.GameManagers.Input;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    public class PlayerControllerDefaultInteraction : ControllerBase
    {
        public PlayerControllerDefaultInteraction(InputDataContainer input, PlayerConditions conditions, PlayerReferences references)
        {
            _input = input;
            _conditions = conditions.Default;
            _parameters = references.scriptableObjects.Default.interaction;
            _headPivot = references.objects.headPivot.transform;
            SubscribeToEvents();
        }

        private readonly InputDataContainer _input;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly PlayerControllerDefaultInteractionScriptableObject _parameters;
        private readonly Transform _headPivot;

        private RaycastHit _hit;
        private InteractableBase _currentSelectedInteractableObject;

        public override void Update()
        {
            if (!_conditions.possibilities.canInteract)
            {
                _currentSelectedInteractableObject = null;
                return;
            }

            InteractionRaycast();
        }

        private void InteractionRaycast()
        {
            if (Physics.Raycast(_headPivot.position, _headPivot.forward, out _hit, _parameters.interactionDistance, _parameters.interactableLayers, QueryTriggerInteraction.Collide))
            {
                InteractableBase thisFrameObject = _hit.collider?.GetComponent<InteractableBase>();

                //Disable selection when moving from one object to next
                if (thisFrameObject && _currentSelectedInteractableObject && thisFrameObject != _currentSelectedInteractableObject)
                {
                    _currentSelectedInteractableObject?.SetObjectSelection(false);
                }

                _currentSelectedInteractableObject = thisFrameObject;
                _currentSelectedInteractableObject?.SetObjectSelection(true);

                #region Debug

#if UNITY_EDITOR

                _parameters.currentlySelectedObjectName = _currentSelectedInteractableObject?.name;

#endif

                #endregion Debug
            }
            else
            {
                if (_currentSelectedInteractableObject)
                {
                    _currentSelectedInteractableObject.EndInteraction();
                    _currentSelectedInteractableObject?.SetObjectSelection(false);
                }

                _currentSelectedInteractableObject = null;
                _conditions.cases.isInteracting = false;

                #region Debug

#if UNITY_EDITOR

                _parameters.currentlySelectedObjectName = "";

#endif

                #endregion Debug
            }
        }

        private void TryToInteract()
        {
            if (!_conditions.possibilities.canInteract)
            {
                Debug.Log("Cannot interact");
                return;
            }

            if (_currentSelectedInteractableObject)
            {
                _currentSelectedInteractableObject.BeginInteraction();
                _conditions.cases.isInteracting = true;
            }
        }

        private void CancelInteractionAction()
        {
            if (_currentSelectedInteractableObject)
            {
                _currentSelectedInteractableObject.EndInteraction();
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