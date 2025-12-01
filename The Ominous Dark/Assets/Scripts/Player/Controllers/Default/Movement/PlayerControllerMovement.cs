using NOS.Controllers;
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
            _floatingCapsule = references.components.floatingCapsule;

            SubscribeToEvents();
        }

        #region Variables

        private readonly InputDataContainer _input;
        private readonly PlayerActions.DefaultActionsClass _actions;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly PlayerValues _values;
        private readonly PlayerControllerMovementScriptableObject _parameters;
        private readonly SettingsControlContainer _controlSettings;

        private readonly Rigidbody _rigidBody;
        private readonly RigidbodyFloatingCapsule _floatingCapsule;

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
            //Exception for control in air
            if (!_conditions.cases.isGrounded)
            {
                _currentTargetVelocity = Vector3.zero;
                ExecuteInAir();
                return;
            }

            //Exception for slope handling
            if (_conditions.cases.isOnTooSteepSlope)
            {
                ExecuteSlopeSliding();
                return;
            }

            //Input
            Vector3 input = GetProcessedInput();

            Vector3 targetVelocity = input * (_currentMovementParameters.maxSpeed);

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

        #endregion Public Methodes

        #region Private Methodes

        #region Updates

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
        }

        #endregion Updates

        #region In Air

        private Vector3 _inAirMomentum;
        private float _inAirMomentumMaximalMagnitude;

        private void OnAirSaveMomentum()
        {
            _inAirMomentum = _values.General.rigidBodyCurrentVelocity;
            float keptY = _inAirMomentum.y;
            _inAirMomentum *= _parameters.inAirMomentumKeepMultiplier;
            _inAirMomentum.y = keptY;
            _rigidBody.linearVelocity = _inAirMomentum; //Reduce momentum
            _inAirMomentum.y = 0;
            _inAirMomentumMaximalMagnitude = _inAirMomentum.magnitude + _parameters.inAirMaximalMomentumAdditionBaseValue;
        }

        private void OnGroundingResetMomentum()
        {
            _inAirMomentum = Vector3.zero;
            _inAirMomentumMaximalMagnitude = 0;
        }

        private void ExecuteInAir()
        {
            Vector3 inputDirection = GetProcessedInput();
            Vector3 velocityWithoutY = _values.General.rigidBodyCurrentVelocity;
            velocityWithoutY.y = 0;
            float forceDirection = Vector3.Dot(inputDirection, velocityWithoutY.normalized);
            Vector3 finalForce = Vector3.zero;

            Vector3 lateralDir = inputDirection - velocityWithoutY.normalized * Vector3.Dot(inputDirection, velocityWithoutY.normalized);
            lateralDir.Normalize();

            //Jump in place//
            if (_values.General.rigidBodyCurrentVelocityMagnitudeXZ < _parameters.inAirMaximalMomentumAdditionJumpingInPlace && _inAirMomentum.magnitude < 0.2f)
            {
                finalForce = inputDirection * _parameters.inAirControlAccelerationSpeed;
            }
            else if (_values.General.rigidBodyCurrentVelocityMagnitudeXZ < _inAirMomentumMaximalMagnitude)
            {
                finalForce = inputDirection * _parameters.inAirControlAccelerationSpeed;
            }
            else if (_values.General.rigidBodyCurrentVelocityMagnitudeXZ >= _inAirMomentumMaximalMagnitude)
            {
                if (forceDirection < -0.01f)
                {
                    finalForce = inputDirection * _parameters.inAirControlAccelerationSpeed;
                }
                else if (lateralDir.sqrMagnitude > 0.01f)
                {
                    finalForce = lateralDir * _parameters.inAirControlAccelerationSpeed;
                }
            }

            _rigidBody.AddForce(finalForce, ForceMode.Acceleration);
        }

        #endregion In Air

        #region Slopes

        private Vector3 GetSlopeSlideVector()
        {
            Vector3 slopeNormal = _values.Default.SlopeCheckNormal;
            Vector3 slopeDirection = (Vector3.up - slopeNormal) * Vector3.Dot(Vector3.up, slopeNormal);
            // slopeDirection.y = 0;
            //   slopeDirection.x = 0;
            //  slopeDirection.z = 0;

            return slopeDirection;
        }

        private void ExecuteSlopeSliding()
        {
            //No resistance while sliding//
            _floatingCapsule.SuspendExecutionNextFixedUpdate();

            //Input
            Vector3 input = GetProcessedInput();
            Vector3 slopeDirection = -GetSlopeSlideVector();

            Vector3 targetVelocity = (slopeDirection + input * _parameters.slidingControlMultiplier).normalized * _parameters.slidingFromSteepSlopeValues.maxSpeed;

            //Velocity not accounting Y//
            Vector3 currentVelocityWithoutY = _values.General.rigidBodyCurrentVelocity;
            currentVelocityWithoutY.y = 0;

            //For quicker turning back
            float inputToVelocityDot = Vector3.Dot(slopeDirection, currentVelocityWithoutY);
            float accelerationValueAfterGraph = _parameters.slidingFromSteepSlopeValues.accelerationSpeed * _parameters.slidingFromSteepSlopeValues.maxAccelerationForceFactorFromDot.Evaluate(inputToVelocityDot);

            _currentTargetVelocity = Vector3.MoveTowards(_currentTargetVelocity, targetVelocity, accelerationValueAfterGraph * Time.fixedDeltaTime);

            Vector3 neededAcceleration = (_currentTargetVelocity - currentVelocityWithoutY) / Time.fixedDeltaTime;

            // neededAcceleration.y = 0;
            neededAcceleration = Vector3.ClampMagnitude(neededAcceleration, _parameters.slidingFromSteepSlopeValues.maxAccelerationForce * _parameters.slidingFromSteepSlopeValues.maxAccelerationForceFactorFromDot.Evaluate(inputToVelocityDot));

            _rigidBody.AddForce(neededAcceleration, ForceMode.Acceleration);
        }

        #endregion Slopes

        #endregion Private Methodes

        #region Events

        private void SubscribeToEvents()
        {
            _actions.OnInAirState += OnAirSaveMomentum;
            _actions.OnGroundedState += OnGroundingResetMomentum;
        }

        public override void OnDestroy()
        {
            _actions.OnInAirState -= OnAirSaveMomentum;
            _actions.OnGroundedState -= OnGroundingResetMomentum;
        }

        #endregion Events
    }
}