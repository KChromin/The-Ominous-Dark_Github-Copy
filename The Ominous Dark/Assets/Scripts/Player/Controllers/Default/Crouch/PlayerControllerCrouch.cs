using NOS.GameManagers.Input;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    public class PlayerControllerCrouch : ControllerBase
    {
        private readonly InputDataContainer _input;
        private readonly PlayerConditions.DefaultConditionsClass.CasesClass _cases;
        private readonly PlayerConditions.DefaultConditionsClass.PossibilitiesClass _possibilities;

        private readonly PlayerControllerCrouchScriptableObject _parameters;
        private readonly CapsuleCollider _collider;
        private readonly Transform _head;

        private PlayerControllerCrouchScriptableObject.CrouchParametersClass _targetValues;
        private bool _colliderUpdateIsNeeded;

        private float _headHeightUpdateCalculations;
        private float _colliderHeightUpdateCalculations;
        private float _colliderCenterUpdateCalculations;

        public enum CrouchStates
        {
            Standing,
            Crouching
        }

        public PlayerControllerCrouch(InputDataContainer input, PlayerConditions conditions, PlayerReferences references)
        {
            _input = input;
            _cases = conditions.Default.cases;
            _possibilities = conditions.Default.possibilities;
            _collider = references.components.collider;
            _parameters = references.scriptableObjects.Default.crouch;
            _head = references.objects.head.transform;
            SubscribeToEvents();
        }

        #region Events

        private void SubscribeToEvents()
        {
            _input.OnPerformedCrouch += OnCrouchPerformed;
            _input.OnCancelCrouch += OnCrouchCanceled;
        }

        public override void OnDestroy()
        {
            _input.OnPerformedCrouch -= OnCrouchPerformed;
            _input.OnCancelCrouch -= OnCrouchCanceled;
        }

        #endregion Events

        private void OnCrouchPerformed()
        {
            _cases.wantsToDoCrouchAction = true;
            ExecuteCrouchAction();
        }

        private void OnCrouchCanceled()
        {
            _cases.wantsToDoCrouchAction = false;
        }

        private void ExecuteCrouchAction()
        {
            if (_cases.isCrouching) //Stand
            {
                if (_cases.isAbleToStandUp) //Can stand up
                {
                    _cases.isCrouching = false;

                    _colliderUpdateIsNeeded = true;
                    SetCrouchState(CrouchStates.Standing);
                }
                else
                {
                    //Here cannot stand up info
                }
            }
            else //Crouch
            {
                if (_possibilities.canCrouch) //Can crouch
                {
                    _cases.isCrouching = true;

                    _colliderUpdateIsNeeded = true;
                    SetCrouchState(CrouchStates.Crouching);
                }
                else
                {
                    //Here cannot crouch info
                }
            }
        }

        private void CheckCrouchCancelingActions()
        {
            if (!_cases.isCrouching) return;

            //When player stops being able to crouch
            if (!_possibilities.canCrouch)
            {
                ExecuteCrouchAction();
            }

            //When player wants to start running//
            if (_cases.wantsToMove && _possibilities.canMove && _cases.wantsToRun && _possibilities.canRun)
            {
                ExecuteCrouchAction();
            }

            //When player try to jump//
            if (_cases.wantsToJump && _possibilities.canJump)
            {
                ExecuteCrouchAction();
            }

            //todo When player is in air for set amount of time//
        }

        public override void Update()
        {
            CheckCrouchCancelingActions();
            UpdateAllValues();
        }

        public void SetCrouchState(CrouchStates newCrouchState)
        {
            switch (newCrouchState)
            {
                default:
                case CrouchStates.Standing:
                    _targetValues = _parameters.standValues;
                    break;
                case CrouchStates.Crouching:
                    _targetValues = _parameters.crouchValues;
                    break;
            }

            _colliderUpdateIsNeeded = true;
        }

        private void UpdateAllValues()
        {
            if (!_colliderUpdateIsNeeded) return;

            bool headHeightCompleted = UpdateHeadHeight();
            bool colliderHeightUpdateCompleted = UpdateColliderHeight();
            bool colliderCenterUpdateCompleted = UpdateCenter();

            if (colliderCenterUpdateCompleted && colliderHeightUpdateCompleted && headHeightCompleted)
            {
                _colliderUpdateIsNeeded = false;
            }
        }

        private bool UpdateHeadHeight()
        {
            Vector3 localPosition = _head.localPosition;
            float currentHeight = localPosition.y;

            //Change Height
            if (!Mathf.Approximately(currentHeight, _targetValues.headHeight))
            {
                currentHeight = Mathf.SmoothDamp(currentHeight, _targetValues.headHeight, ref _headHeightUpdateCalculations, _targetValues.updateTime, Mathf.Infinity, Time.deltaTime);

                //Rounding
                if (Mathf.Abs(currentHeight - _targetValues.headHeight) <= _parameters.roundingValue)
                {
                    currentHeight = _targetValues.headHeight;
                }

                //Update Collider
                localPosition.y = currentHeight;
                _head.localPosition = localPosition;

                return false;
            }

            //Height is good//
            return true;
        }

        private bool UpdateColliderHeight()
        {
            float currentHeight = _collider.height;

            //Change Height
            if (!Mathf.Approximately(currentHeight, _targetValues.colliderHeight))
            {
                currentHeight = Mathf.SmoothDamp(currentHeight, _targetValues.colliderHeight, ref _colliderHeightUpdateCalculations, _targetValues.updateTime, Mathf.Infinity, Time.deltaTime);

                //Rounding
                if (Mathf.Abs(currentHeight - _targetValues.colliderHeight) <= _parameters.roundingValue)
                {
                    currentHeight = _targetValues.colliderHeight;
                }

                //Update Collider
                _collider.height = currentHeight;

                return false;
            }

            //Height is good//
            return true;
        }

        private bool UpdateCenter()
        {
            Vector3 currentCenter = _collider.center;
            float currentCenterY = currentCenter.y;

            //Change Center
            if (!Mathf.Approximately(currentCenterY, _targetValues.colliderCenterY))
            {
                currentCenterY = Mathf.SmoothDamp(currentCenterY, _targetValues.colliderCenterY, ref _colliderCenterUpdateCalculations, _targetValues.updateTime, Mathf.Infinity, Time.deltaTime);

                if (Mathf.Abs(currentCenterY - _targetValues.colliderCenterY) <= _parameters.roundingValue)
                {
                    currentCenterY = _targetValues.colliderCenterY;
                }

                //Update Collider
                currentCenter.y = currentCenterY;
                _collider.center = currentCenter;

                return false;
            }

            //Center is good//
            return true;
        }
    }
}