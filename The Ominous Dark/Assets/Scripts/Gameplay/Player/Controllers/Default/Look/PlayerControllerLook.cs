using NOS.GameManagers.Input;
using NOS.GameManagers.Settings;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    public class PlayerControllerLook : ControllerBase
    {
        public PlayerControllerLook(InputDataContainer input, PlayerConditions conditions, PlayerReferences references, SettingsManager settingsManager)
        {
            _input = input;
            _conditions = conditions.Default;
            _orientation = references.Objects.orientation.transform;
            _headPivot = references.Objects.headPivot.transform;
            _parameters = references.ScriptableObjects.Default.look;
            _settingsManager = settingsManager;
        }

        private readonly InputDataContainer _input;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly Transform _orientation;
        private readonly Transform _headPivot;
        private readonly PlayerControllerLookScriptableObject _parameters;
        private readonly SettingsManager _settingsManager;

        private float _verticalRotationValue;
        private Vector2 _finalInput;
        private Vector2 _smoothedInputCalculations;

        public override void Update()
        {
            //Input
            if (_settingsManager.CurrentSettings.control.LookSmoothing)
            {
                LookSmoothing();
            }
            else
            {
                _finalInput = CurrentInput();
            }

            #region Cases

            if (_finalInput == Vector2.zero || (!_conditions.possibilities.canLookY && !_conditions.possibilities.canLookX))
            {
                _conditions.cases.isLooking = false;
            }
            else
            {
                _conditions.cases.isLooking = true;
            }

            #endregion Cases

            RotationVertical();
            RotationHorizontal();
        }

        #region Input

        //Get current input with all settings applied//
        private Vector2 CurrentInput()
        {
            Vector2 rawInput = _input.inputLook;

            return InputWithControlSettings(rawInput);
        }

        //Apply control settings//
        private Vector2 InputWithControlSettings(Vector2 rawInput)
        {
            //Invert Y Axis//
            if (_settingsManager.CurrentSettings.control.LookInvertYAxis)
            {
                rawInput.y = -rawInput.y;
            }

            //Separate Sensitivities//
            if (_settingsManager.CurrentSettings.control.LookSeparateSensitivityAxes)
            {
                Vector2 sensitivity = new(_settingsManager.CurrentSettings.control.LookSensitivityXAxis, _settingsManager.CurrentSettings.control.LookSensitivityYAxis);
                return new Vector2(rawInput.x * sensitivity.x, rawInput.y * sensitivity.y);
            }

            return rawInput * _settingsManager.CurrentSettings.control.LookSensitivityGeneral;
        }

        //Smooth input in time//
        private void LookSmoothing()
        {
            _finalInput = Vector2.SmoothDamp(_finalInput, CurrentInput(), ref _smoothedInputCalculations, _settingsManager.CurrentSettings.control.LookSmoothingTime, Mathf.Infinity, Time.deltaTime);
        }

        #endregion Input

        #region Execution

        private void RotationVertical()
        {
            if (_finalInput.y == 0 || !_conditions.possibilities.canLookY) return;

            //Update by delta//
            _verticalRotationValue -= _finalInput.y;

            //Clamp angles//
            _verticalRotationValue = Mathf.Clamp(_verticalRotationValue, _parameters.maxAngleDown, _parameters.maxAngleUp);

            //Execute//
            _headPivot.localRotation = Quaternion.Euler(Vector3.right * _verticalRotationValue);
        }

        private void RotationHorizontal()
        {
            if (_finalInput.x == 0 || !_conditions.possibilities.canLookX) return;

            _orientation.Rotate(_finalInput.x * Vector3.up);
        }

        #endregion Execution
    }
}