using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;
using NOS.GameManagers.Settings;
using NOS.Player.Controller.General;

namespace NOS.Player.Controller.Default
{
    //Head bobbing

    public class PlayerControllerHeadBobbingDefault : ControllerBase
    {
        public PlayerControllerHeadBobbingDefault(PlayerReferences references, PlayerControllers.GeneralControllersClass controllers, PlayerConditions conditions, SettingsContainers settings, PlayerActions actions)
        {
            _parameters = references.ScriptableObjects.Default.headBobbing;
            _conditions = conditions.Default;
            _headBobbing = references.Objects.headBobbing.transform;
            _headController = controllers.Head;
            _gameSettings = settings.game;
            _actions = actions;
        }

        #region Variables

        private readonly PlayerControllerHeadBobbingDefaultScriptableObject _parameters;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly SettingsGameContainer _gameSettings;
        private readonly PlayerControllerGeneralHead _headController;
        private readonly PlayerActions _actions;

        private readonly Transform _headBobbing;
        private PlayerControllerHeadBobbingDefaultScriptableObject.HeadBobbingDefaultValuesClass _currentHeadBobbingParameters = new();

        private Vector3 _headBobbingSmoothTransitionCalculations;

        private float _cycleTimer;
        private bool _cycleReachedFirstPi;

        private bool _needReset;
        private bool _resetRequest;

        private HeadBobbingStates _currentState;

        private const float Tau = Mathf.PI * 2;

        public enum HeadBobbingStates
        {
            Disable,
            Idle,
            Walk,
            Run,
            CrouchIdle,
            CrouchWalk
        }

        #endregion Variables

        public void SetHeadBobbingState(HeadBobbingStates headBobbingState)
        {
            _currentState = headBobbingState;
            SetCurrentStateParameters();
        }
        
        private bool _isMovingAboveMinimalThresholdLastFrame;
        private bool _isRunningAboveMinimalThresholdLastFrame;

        public override void Update()
        {
            //When disabled in options, then reset, and don't update//
            if (_gameSettings.headBobbingIntensity == 0)
            {
                if (_needReset)
                {
                    ResetToDefault();
                }

                return;
            }

            if (_resetRequest)
            {
                ResetToDefault();
                return;
            }

            #region Update thresholds while moving

            if (_isMovingAboveMinimalThresholdLastFrame != _conditions.cases.isMovingAboveMinimalThreshold || _isRunningAboveMinimalThresholdLastFrame != _conditions.cases.isRunningAboveMinimalThreshold)
            {
                SetCurrentStateParameters();
            }

            _isMovingAboveMinimalThresholdLastFrame = _conditions.cases.isMovingAboveMinimalThreshold;
            _isRunningAboveMinimalThresholdLastFrame = _conditions.cases.isRunningAboveMinimalThreshold;

            #endregion Update thresholds while moving

            _needReset = true;

            if (_conditions.cases.isGrounded)
            {
                UpdateCycleTimer();
                ExecuteHeadBobbing();
            }
            else
            {
                ResetToDefault();
            }
        }

        private void SetCurrentStateParameters()
        {
            switch (_currentState)
            {
                default:
                case HeadBobbingStates.Idle:
                    _currentHeadBobbingParameters = _parameters.idleValues;
                    break;
                case HeadBobbingStates.Walk:
                    _currentHeadBobbingParameters = _conditions.cases.isMovingAboveMinimalThreshold ? _parameters.walkValues : _parameters.idleValues;
                    break;
                case HeadBobbingStates.Run:
                    if (_conditions.cases.isRunningAboveMinimalThreshold)
                    {
                        _currentHeadBobbingParameters = _parameters.runValues;
                    }
                    else if (_conditions.cases.isMovingAboveMinimalThreshold)
                    {
                        _currentHeadBobbingParameters = _parameters.walkValues;
                    }
                    else
                    {
                        _currentHeadBobbingParameters = _parameters.idleValues;
                    }

                    break;
                case HeadBobbingStates.CrouchIdle:
                    _currentHeadBobbingParameters = _parameters.crouchIdleValues;
                    break;
                case HeadBobbingStates.CrouchWalk:
                    _currentHeadBobbingParameters = _conditions.cases.isMovingAboveMinimalThreshold ? _parameters.crouchWalkValues : _parameters.crouchIdleValues;
                    break;
                case HeadBobbingStates.Disable:
                    _currentHeadBobbingParameters = _parameters.disabledValues;
                    break;
            }
        }

        private void UpdateCycleTimer()
        {
            _cycleTimer += Time.deltaTime * _currentHeadBobbingParameters.headBobbingSpeed;

            //Delete 2Pi from timer, for any case to avoid overflowing//
            //And to know when new cycle began//

            if (_cycleReachedFirstPi)
            {
                if (_cycleTimer > Tau)
                {
                    _cycleTimer -= Tau;
                    _cycleReachedFirstPi = false;
                    _actions.Default.OnHeadBobHalfCycle?.Invoke();
                }
            }
            else
            {
                if (_cycleTimer > Mathf.PI) //First time timer reached PI
                {
                    _cycleReachedFirstPi = true;
                    _actions.Default.OnHeadBobHalfCycle?.Invoke();
                }
            }
        }

        private void ExecuteHeadBobbing()
        {
            float currentCos = Mathf.Sin(_cycleTimer); // -1 - 1
            sbyte positiveNegativeMultiplier = 1;

            if (currentCos < 0)
            {
                positiveNegativeMultiplier = -1;
            }

            Vector3 headCurrentLocalPosition = _headBobbing.localPosition;

            if (_currentHeadBobbingParameters.headBobbingMaxWidth == 0)
            {
                float currentPercentageToMaxRange = Mathf.Abs(currentCos) / 1;
                headCurrentLocalPosition.y = currentPercentageToMaxRange * (_currentHeadBobbingParameters.headBobbingMaxHeight * _gameSettings.headBobbingIntensity);
            }
            else
            {
                //Width
                float currentPercentageToMaxRange = Mathf.Abs(currentCos) / 1;
                headCurrentLocalPosition.x = positiveNegativeMultiplier * currentPercentageToMaxRange * (_currentHeadBobbingParameters.headBobbingMaxWidth * _gameSettings.headBobbingIntensity);

                //Height
                float currentWidthToMaxPercent = Mathf.Abs(headCurrentLocalPosition.x) / (_currentHeadBobbingParameters.headBobbingMaxWidth * _gameSettings.headBobbingIntensity);
                float currentHeightMultiplierValueFromCurve = _currentHeadBobbingParameters.headBobbingHeightCurve.Evaluate(currentWidthToMaxPercent);
                headCurrentLocalPosition.y = currentHeightMultiplierValueFromCurve * (_currentHeadBobbingParameters.headBobbingMaxHeight * _gameSettings.headBobbingIntensity);
            }

            //Execute smooth transition in position//
            Vector3 newLocalPosition = Vector3.SmoothDamp(_headBobbing.localPosition, headCurrentLocalPosition, ref _headBobbingSmoothTransitionCalculations, _parameters.transitionSmoothingTime, Mathf.Infinity, Time.deltaTime);
            _headController.UpdateLocalPositionHeadBobbing(newLocalPosition);
        }

        public void DisableHeadBobbing()
        {
            SetHeadBobbingState(HeadBobbingStates.Disable);
        }

        private void ResetToDefault()
        {
            Vector3 newLocalPosition = Vector3.SmoothDamp(_headBobbing.localPosition, Vector3.zero, ref _headBobbingSmoothTransitionCalculations, _parameters.timeToFullResetInSeconds, Mathf.Infinity, Time.deltaTime);
            _headController.UpdateLocalPositionHeadBobbing(newLocalPosition);

            if (_headBobbing.localPosition == Vector3.zero)
            {
                _needReset = false;
                _resetRequest = false;
            }
        }
    }
}