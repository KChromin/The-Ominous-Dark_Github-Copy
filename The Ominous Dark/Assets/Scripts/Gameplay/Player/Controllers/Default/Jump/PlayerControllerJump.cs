using System.Threading;
using Cysharp.Threading.Tasks;
using NOS.Controllers;
using NOS.GameManagers.Input;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    public class PlayerControllerJump : ControllerBase
    {
        private readonly InputDataContainer _input;
        private readonly PlayerConditions.DefaultConditionsClass.CasesClass _cases;
        private readonly PlayerConditions.DefaultConditionsClass.PossibilitiesClass _possibilities;
        private readonly PlayerActions.DefaultActionsClass _actions;
        private readonly PlayerDynamicReferences _dynamicReferences;

        private readonly PlayerControllerJumpScriptableObject _parameters;

        private readonly Rigidbody _rigidBody;
        private readonly RigidbodyFloatingCapsule _floatingCapsule;

        private bool _shouldApplyAdditionalGravity;

        //Because crouch is executed earlier, and jump cannot be done when crouching, must somehow check this//
        private bool _wasCrouchingLastFrame;

        private readonly CancellationTokenSource _cts = new();
        private readonly CancellationTokenSource _failedJumpCheckCancellationToken = new();


        public PlayerControllerJump(InputDataContainer input, PlayerConditions conditions, PlayerReferences references, PlayerDynamicReferences dynamicReferences, PlayerActions playerActions)
        {
            _input = input;
            _cases = conditions.Default.cases;
            _possibilities = conditions.Default.possibilities;
            _actions = playerActions.Default;
            _dynamicReferences = dynamicReferences;
            _parameters = references.ScriptableObjects.Default.jump;
            _rigidBody = references.Components.rigidBody;
            _floatingCapsule = references.Components.floatingCapsule;
            SubscribeToEvents();
        }

        public override void Update()
        {
            CoyoteJumpUpdateTimer();

            if (!_shouldApplyAdditionalGravity && _cases.isJumping && !_cases.isGrounded)
            {
                ApplyAdditionalGravityForSomeTime().Forget();
            }
        }

        private async UniTaskVoid ApplyAdditionalGravityForSomeTime()
        {
            _shouldApplyAdditionalGravity = true;
            await UniTask.WaitForSeconds(_parameters.gravityBoostAfterJumpTime);
            _shouldApplyAdditionalGravity = false;
        }

        public override void FixedUpdate()
        {
            ApplyAdditionalGravity();
        }

        private bool _checkForCoyoteJump;
        private float _currentTimeFromLastTouchingTheGround;

        private void CoyoteJumpUpdateTimer()
        {
            if (_checkForCoyoteJump)
            {
                _currentTimeFromLastTouchingTheGround += Time.deltaTime;
            }
        }

        private bool ShouldCoyoteJump()
        {
            if (!_checkForCoyoteJump) return false;

            bool isAbleToCoyoteJump = _currentTimeFromLastTouchingTheGround <= _parameters.coyoteJumpTime;

            if (isAbleToCoyoteJump)
            {
                _checkForCoyoteJump = false;
            }

            return isAbleToCoyoteJump;
        }

        private void TryToExecuteJump()
        {
            //When cannot jump
            if (!_possibilities.canJump) return;

            bool shouldExecuteCoyoteJump = ShouldCoyoteJump();

            //Cannot jump when not grounded, but give time for coyote jump
            if (!_cases.isGrounded && !shouldExecuteCoyoteJump) return;

            //No stamina left to jump//
            if (_cases.staminaIsTooLowToJump) return;

            //Cannot jump on too steep slopes
            if (_cases.isOnTooSteepSlope) return;

            //Cannot jump while crouching
            if (_wasCrouchingLastFrame || _cases.isCrouching) return;

            //If already jumped, then cannot do that again
            if (_cases.isJumping) return;

            //When on cooldown
            if (_cases.jumpIsOnCooldown) return;

            _cases.wantsToJump = true;
            FailedJumpStateReset().Forget();
            ExecuteJump();

            //When coyote jumping, change isJumping state immediately because it is already in air//
            if (shouldExecuteCoyoteJump)
            {
                _cases.isJumping = true;
            }
        }

        private void ExecuteJump()
        {
            //No ground snapping for some time//
            _floatingCapsule?.SuspendSpringExecution(_parameters.floatingCapsuleSuspensionTime);

            //Reset Velocity
            Vector3 currentVelocity = _rigidBody.linearVelocity;
            currentVelocity.y = 0;
            _rigidBody.linearVelocity = currentVelocity;

            _rigidBody.AddRelativeForce(Vector3.up * _parameters.jumpPower, ForceMode.VelocityChange);
        }

        private async UniTaskVoid ExecuteCooldown()
        {
            _cases.jumpIsOnCooldown = true;
            await UniTask.WaitForSeconds(_parameters.jumpCooldownTime, cancellationToken: _cts.Token, cancelImmediately: true);
            _cases.jumpIsOnCooldown = false;
        }

        private async UniTaskVoid FailedJumpStateReset()
        {
            await UniTask.WaitForSeconds(_parameters.failedJumpCaseResetTimeInSeconds, cancellationToken: _failedJumpCheckCancellationToken.Token, cancelImmediately: true);
            if (_cases.wantsToJump && !_cases.isJumping)
            {
                _cases.wantsToJump = false;
            }
        }

        private void OnAirborne()
        {
            if (_cases.wantsToJump)
            {
                _cases.isJumping = true;
            }
            else
            {
                _checkForCoyoteJump = true;
            }
        }

        private void OnLanding()
        {
            //Cancel FailedJumpState 
            _failedJumpCheckCancellationToken.Cancel();

            //Set jump cooldown
            if (_cases.isJumping)
            {
                ExecuteCooldown().Forget();
            }

            //Reset everything
            _cases.wantsToJump = false;
            _cases.isJumping = false;
            _currentTimeFromLastTouchingTheGround = 0;
            _checkForCoyoteJump = false;
            _shouldApplyAdditionalGravity = false;
        }

        private void OnStanding()
        {
            KeepInformationIfWasCrouchingLastFrame().Forget();
        }

        private void OnCrouch()
        {
            _wasCrouchingLastFrame = true;
        }

        private async UniTaskVoid KeepInformationIfWasCrouchingLastFrame()
        {
            await UniTask.NextFrame(cancellationToken: _cts.Token, cancelImmediately: true);
            _wasCrouchingLastFrame = false;
        }

        private void ApplyAdditionalGravity()
        {
            if (!_shouldApplyAdditionalGravity) return;

            _rigidBody.AddRelativeForce(Vector3.down * _parameters.gravityBoostAfterJump, ForceMode.Acceleration);
        }

        #region Events

        private void SubscribeToEvents()
        {
            _input.OnPerformedJump += TryToExecuteJump;
            _actions.OnInAirState += OnAirborne;
            _actions.OnGroundedState += OnLanding;
            _actions.OnStandingState += OnStanding;
            _actions.OnCrouchingState += OnCrouch;
        }

        public override void OnDestroy()
        {
            _cts.Cancel();
            _actions.OnStandingState -= OnStanding;
            _input.OnPerformedJump -= TryToExecuteJump;
            _actions.OnInAirState -= OnAirborne;
            _actions.OnGroundedState -= OnLanding;
            _actions.OnCrouchingState -= OnCrouch;
        }

        #endregion Events
    }
}