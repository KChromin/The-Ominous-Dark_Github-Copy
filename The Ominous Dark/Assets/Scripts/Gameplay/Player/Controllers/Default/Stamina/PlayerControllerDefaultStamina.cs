using NOS.GameplayManagers;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    public class PlayerControllerDefaultStamina : ControllerBase
    {
        public PlayerControllerDefaultStamina(PlayerActions actions, PlayerConditions conditions, PlayerReferences references, PlayerDynamicReferences dynamicReferences, PlayerValues values)
        {
            _actions = actions;
            _conditions = conditions.Default;
            _parameters = references.ScriptableObjects.Default.stamina;
            _dynamicReferences = dynamicReferences.Default.StaminaDefault;
            _values = values.Default;

            _values.currentStamina = _parameters.maximalStamina;

            _volumesManager = GlobalVolumesManager.Instance;

            SubscribeToEvents();
        }

        private readonly PlayerActions _actions;
        private readonly PlayerControllerDefaultStaminaScriptableObject _parameters;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly PlayerDynamicReferences.DefaultDynamicReferencesClass.StaminaDefaultClass _dynamicReferences;
        private readonly PlayerValues.DefaultValuesClass _values;
        private readonly GlobalVolumesManager _volumesManager;

        private void SubscribeToEvents()
        {
            _actions.Default.OnInAirState += CheckForJump;
        }

        public override void OnDestroy()
        {
            _actions.Default.OnInAirState -= CheckForJump;
        }


        void CheckForJump()
        {
            if (_conditions.cases.isJumping)
            {
                DecreaseStaminaJump();
            }
        }

        public override void Update()
        {
            if (_conditions.cases.isRunning && _conditions.cases.isRunningAboveMinimalThreshold)
            {
                DecreaseStaminaRunning();
            }
            else if (_conditions.cases.isGrounded)
            {
                IncreaseStamina();
            }

            if (_values.currentStamina < _parameters.staminaCostPerJump || _conditions.cases.staminaWasFullyDepleted)
            {
                _conditions.cases.staminaIsTooLowToJump = true;
            }
            else
            {
                _conditions.cases.staminaIsTooLowToJump = false;
            }

            UpdateOverlay();
        }


        private float _currentOverlayValue;
        private float _currentOverlayValueCalculations;

        private void UpdateOverlay()
        {
            float targetOverlayValue = 0;

            if (_conditions.cases.staminaWasFullyDepleted)
            {
                targetOverlayValue = 1;
            }
            else if (_values.currentStamina < _parameters.staminaOverlayFadingInStartValue)
            {
                float currentStamina01 = 1 - (_values.currentStamina / _parameters.staminaOverlayFadingInStartValue);
                targetOverlayValue = currentStamina01;
            }

            if (!Mathf.Approximately(_currentOverlayValue, targetOverlayValue))
            {
                _currentOverlayValue = Mathf.SmoothDamp(_currentOverlayValue, targetOverlayValue, ref _currentOverlayValueCalculations, _parameters.staminaOverlaySmoothTransitionTime);
                SetOverlay(_currentOverlayValue);
            }
        }

        private void DecreaseStaminaRunning()
        {
            if (_values.currentStamina > 0)
            {
                _values.currentStamina -= _parameters.staminaCostPerSecondRunning * Time.deltaTime;
            }
            else if (_values.currentStamina <= 0)
            {
                _values.currentStamina = 0;
                _conditions.cases.staminaWasFullyDepleted = true;
            }

            StaminaClampAndUpdateUI();
        }

        private void DecreaseStaminaJump()
        {
            _values.currentStamina -= _parameters.staminaCostPerJump;
            StaminaClampAndUpdateUI();
        }

        private void StaminaClampAndUpdateUI()
        {
            _values.currentStamina = Mathf.Clamp(_values.currentStamina, 0, _parameters.maximalStamina);
            _dynamicReferences.OnStaminaValueChange?.Invoke(_values.currentStamina / _parameters.maximalStamina);
        }

        private void IncreaseStamina()
        {
            if (Mathf.Approximately(_values.currentStamina, 100)) return;

            if (_conditions.cases.staminaWasFullyDepleted && _values.currentStamina <= _parameters.staminaRecoveryValueAfterDepletion)
            {
                _values.currentStamina += _parameters.staminaRegenerationAfterFullyDepleting * Time.deltaTime;
            }
            else
            {
                _conditions.cases.staminaWasFullyDepleted = false;
                _values.currentStamina += _parameters.staminaRegenerationBase * Time.deltaTime;
            }

            StaminaClampAndUpdateUI();
        }

        private void SetOverlay(float intensity01)
        {
            _volumesManager.UpdateStaminaOverlay(intensity01);
        }
    }
}