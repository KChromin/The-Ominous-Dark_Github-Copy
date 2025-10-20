using NOS.GameManagers.Settings;
using NOS.GameManagers.Input;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;


namespace NOS.Player.Controller.Default
{
    public class PlayerControllerMovement : ControllerBase
    {
        public PlayerControllerMovement(InputDataContainer input, PlayerActions actions, PlayerConditions conditions, PlayerValues values, PlayerReferences references)
        {
            _input = input;
            _actions = actions.Default;
            _conditions = conditions.Default;
            _values = values;
            _parameters = references.scriptableObjects.Default.movement;
            _controlSettings = SettingsManager.Instance.CurrentSettings.control;

            _rigidBody = references.components.rigidBody;
        }

        #region Variables

        private readonly InputDataContainer _input;
        private readonly PlayerActions.DefaultActionsClass _actions;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly PlayerValues _values;
        private readonly PlayerControllerMovementScriptableObject _parameters;
        private readonly SettingsControlContainer _controlSettings;

        private readonly Rigidbody _rigidBody;

        private float _currentForce;
        private bool _applyGravitation;

        private Vector3 _currentTargetVelocity;

        private PlayerControllerMovementScriptableObject.MovementValuesClass _currentMovementParameters = new();

        public enum MovementStates
        {
            Idle,
            Walk,
            Run,
            CrouchIdle,
            CrouchWalk
        }

        #endregion Variables

        #region Public Methodes

        #region Set Parameters

        public void SetMovementParameters(MovementStates movementState)
        {
            switch (movementState)
            {
                default:
                case MovementStates.Idle:
                    _currentMovementParameters = _parameters.idleValues;
                    break;
                case MovementStates.Walk:
                    _currentMovementParameters = _parameters.walkValues;
                    break;
                case MovementStates.Run:
                    _currentMovementParameters = _parameters.runValues;
                    break;
                case MovementStates.CrouchIdle:
                    _currentMovementParameters = _parameters.crouchIdleValues;
                    break;
                case MovementStates.CrouchWalk:
                    _currentMovementParameters = _parameters.crouchWalkValues;
                    break;
            }
        }

        #endregion Set Parameters

        public void ExecuteMovement()
        {
            if (!_conditions.cases.isGrounded) return;

            #region Speed Modifiers

            float speedModifiersValue = 1;

            //Steep slope modifier//
            if (_conditions.cases.isOnTooSteepSlope)
            {
                speedModifiersValue *= _parameters.slopeSlidingMovementMultiplier;
            }

            #endregion Speed Modifiers

            //Input
            Vector3 input = GetProcessedInput();
            Vector3 targetVelocity = input * (_currentMovementParameters.maxSpeed * speedModifiersValue);

            //Velocity not accounting Y//
            Vector3 currentVelocityWithoutY = _values.General.rigidBodyCurrentVelocity;
            currentVelocityWithoutY.y = 0;

            //For quicker turning back
            float inputToVelocityDot = Vector3.Dot(input, currentVelocityWithoutY);
            float accelerationValueAfterGraph = _currentMovementParameters.accelerationSpeed * _currentMovementParameters.maxAccelerationForceFactorFromDot.Evaluate(inputToVelocityDot);

            _currentTargetVelocity = Vector3.MoveTowards(_currentTargetVelocity, targetVelocity, accelerationValueAfterGraph * Time.fixedDeltaTime);

            Vector3 neededAcceleration = (_currentTargetVelocity - currentVelocityWithoutY) / Time.fixedDeltaTime;

            neededAcceleration.y = 0;
            neededAcceleration = Vector3.ClampMagnitude(neededAcceleration, _currentMovementParameters.maxAccelerationForce * _currentMovementParameters.maxAccelerationForceFactorFromDot.Evaluate(inputToVelocityDot));

            _rigidBody.AddForce(neededAcceleration * _values.General.rigidBodyMass, ForceMode.Force);
        }

        public void ApplyGravitation()
        {
            _rigidBody.AddRelativeForce(Physics.gravity, ForceMode.Acceleration);
        }

        public override void Update()
        {
            CheckAllPlayerWants();
        }

        public void ExecuteSlideFromSteepSlope()
        {
            if (!_conditions.cases.isOnTooSteepSlope) return;

            _rigidBody.AddForce(GetSlopeSlideVector() * _values.General.rigidBodyMass, ForceMode.Force);
        }

        #endregion Public Methodes

        #region Private Methodes

        private Vector3 GetProcessedInput()
        {
            Vector3 rawInput = _input.inputMove3D;

            #region Apply Processing

            rawInput.x *= _parameters.factorMoveSideways;

            if (rawInput.z < 0)
            {
                rawInput.z *= _parameters.factorMoveBackwards;
            }

            #endregion Apply Processing

            return _values.General.orientationCurrentRotation * rawInput;
        }

        private void CheckAllPlayerWants()
        {
            _conditions.cases.wantsToMove = _input.inputtingMove;
            _conditions.cases.wantsToRun = _input.inputtingRun;
            _conditions.cases.wantsToJump = _input.inputtingJump;
        }

        private Vector3 GetSlopeSlideVector()
        {
            Vector3 slopeNormal = _values.Default.SlopeCheckNormal;
            Vector3 slopeDirection = Vector3.up - slopeNormal * Vector3.Dot(Vector3.up, slopeNormal);
            slopeDirection.y = 0;

            return slopeDirection * (-_parameters.slopeSlidingSpeed * _values.General.rigidBodyMass);
        }

        #endregion Private Methodes
    }
}