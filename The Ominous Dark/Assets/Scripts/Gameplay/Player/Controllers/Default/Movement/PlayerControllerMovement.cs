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
            _parameters = references.ScriptableObjects.Default.movement;
            _controlSettings = SettingsManager.Instance.CurrentSettings.control;

            _rigidBody = references.Components.rigidBody;
            _floatingCapsule = references.Components.floatingCapsule;

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

        private float _currentMovementMaxSpeed;
        private float _currentMovementMaxSpeedCalculations;
        private float _currentSlopeSlidingSpeed;
        private float _currentSlopeSlidingSpeedCalculations;
        private float _timeFromStartingToSlide;

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

        private bool _onEnterSlopeSlidingResetWasDone;

        public void ExecuteMovement()
        {
            UpdateMoveThresholdCases();

            //Exception for control in air
            if (!_conditions.cases.isGrounded)
            {
                if (_conditions.cases.isJumping)
                {
                    _currentTargetVelocity = Vector3.zero;
                }

                ExecuteInAir();
                return;
            }

            //Exception for slope handling
            if (_conditions.cases.isOnTooSteepSlope)
            {
                if (!_onEnterSlopeSlidingResetWasDone)
                {
                    _onEnterSlopeSlidingResetWasDone = true;
                }

                ExecuteSlopeSliding();
                return;
            }

            _onEnterSlopeSlidingResetWasDone = false;
            
            SlidingMaxSpeedReduceInTime();
            _timeFromStartingToSlide = 0;

            //Input
            Vector3 input = GetProcessedInput();

            //Update max speed in time, for less jerky movement//
            _currentMovementMaxSpeed = Mathf.SmoothDamp(_currentMovementMaxSpeed, _currentMovementParameters.maxSpeed, ref _currentMovementMaxSpeedCalculations, _parameters.maxSpeedUpdateInTimeValue, Mathf.Infinity, Time.fixedDeltaTime);

            Vector3 targetVelocity = input * MaxSpeedAfterSlopeSpeedReduction(_currentMovementMaxSpeed, input);

            //Collide and slide//
            if (_conditions.cases.wantsToMoveOnTooSteepSlope)
            {
                Vector3 slopeNormal = _values.Default.slopeCheckDirectionNormal;
                slopeNormal.y = 0;
                targetVelocity = Vector3.ProjectOnPlane(targetVelocity.normalized, slopeNormal.normalized) * targetVelocity.magnitude;
            }


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

        private void SlidingMaxSpeedReduceInTime()
        {
            if (_currentSlopeSlidingSpeed != 0)
            {
                _currentSlopeSlidingSpeed = Mathf.SmoothDamp(_currentSlopeSlidingSpeed, 0, ref _currentSlopeSlidingSpeedCalculations, _parameters.slidingMaxSpeedReductionSpeed, Mathf.Infinity, Time.fixedDeltaTime);

                if (Mathf.Approximately(_currentSlopeSlidingSpeed, 0))
                {
                    _currentSlopeSlidingSpeed = 0;
                }
            }
        }

        private float MaxSpeedAfterSlopeSpeedReduction(float currentMaxSpeed, Vector3 input)
        {
            if (_values.Default.slopeCheckAngle > _parameters.steepSlopeMovementReductionThresholdStart)
            {
                Vector3 slopeNormal = _values.Default.slopeCheckNormal;
                float slopeDirection = Vector3.Dot(slopeNormal, input);

                if (slopeDirection < 0)
                {
                    float speedBasedOnSlope = Mathf.Lerp(currentMaxSpeed, currentMaxSpeed * _parameters.steepSlopeMovementReductionMaximalReductionMultiplier, _values.Default.slopeCheckAngle / _parameters.steepSlopeMovementReductionThresholdEnd);

                    return speedBasedOnSlope;
                }
            }

            return currentMaxSpeed;
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

        #region Checkers

        private void UpdateMoveThresholdCases()
        {
            float currentHorizontalSpeed = _values.General.rigidBodyCurrentVelocityMagnitudeXZ;

            if (_conditions.cases.isMoving)
            {
                if (currentHorizontalSpeed >= _parameters.minimalMovingThreshold)
                {
                    _conditions.cases.isMovingAboveMinimalThreshold = true;
                }
                else
                {
                    _conditions.cases.isMovingAboveMinimalThreshold = false;
                }
            }
            else
            {
                _conditions.cases.isMovingAboveMinimalThreshold = false;
            }

            if (_conditions.cases.isRunning)
            {
                if (currentHorizontalSpeed >= _parameters.minimalRunningThreshold)
                {
                    _conditions.cases.isRunningAboveMinimalThreshold = true;
                }
                else
                {
                    _conditions.cases.isRunningAboveMinimalThreshold = false;
                }
            }
            else
            {
                _conditions.cases.isRunningAboveMinimalThreshold = false;
            }
        }

        #endregion Checkers

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
            _conditions.cases.wantsToMove = _input.inputtingMove && _input.inputMove.magnitude >= InputDataContainer.MinimumInputMagnitudeBeforeTrigger;
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
            Vector3 slopeNormal = _values.Default.slopeCheckNormal;
            Vector3 slopeDirection = (Vector3.up - slopeNormal) * Vector3.Dot(Vector3.up, slopeNormal);
            // slopeDirection.y = 0;
            //   slopeDirection.x = 0;
            //  slopeDirection.z = 0;

            return slopeDirection;
        }

        private void ExecuteSlopeSliding()
        {
            if (_timeFromStartingToSlide >= _parameters.slidingTimeBeforeDisablingFloatingCapsule)
            {
                //No resistance while sliding//
                _floatingCapsule?.SuspendExecutionNextFixedUpdate();
            }

            _timeFromStartingToSlide += Time.fixedDeltaTime;

            //Input
            Vector3 input = GetProcessedInput();
            Vector3 slopeDirection = -GetSlopeSlideVector();

            _currentSlopeSlidingSpeed = Mathf.SmoothDamp(_currentSlopeSlidingSpeed, _parameters.slidingFromSteepSlopeValues.maxSpeed, ref _currentSlopeSlidingSpeedCalculations, _parameters.slidingMaxSpeedAccelerationSpeed, Mathf.Infinity, Time.fixedDeltaTime);

            Vector3 targetVelocity = (slopeDirection + input * _parameters.slidingControlMultiplier).normalized * _currentSlopeSlidingSpeed;

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