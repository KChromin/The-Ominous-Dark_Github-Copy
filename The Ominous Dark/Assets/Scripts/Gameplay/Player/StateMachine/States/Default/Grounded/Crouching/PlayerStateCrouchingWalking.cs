using NOS.Patterns.StateMachine;
using NOS.Player.Controller.Default;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateCrouchingWalking : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateCrouchingWalking(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.crouchingState = PlayerDebug.StateMachineClass.DefaultStatesClass.CrouchingStates.Walking;
#endif

            #endregion Debug

            //Set Condition
            Ctx.Conditions.Default.cases.isMoving = true;

            //Set Movement Speed//
            Ctx.Controllers.Default.Movement.SetMovementParameters(PlayerControllerMovement.MovementStates.CrouchWalk);

            //Set Head Bobbing//
            Ctx.Controllers.Default.HeadBobbing.SetHeadBobbingState(PlayerControllerHeadBobbingDefault.HeadBobbingStates.CrouchWalk);
        }

        protected override void ExitState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.crouchingState = PlayerDebug.StateMachineClass.DefaultStatesClass.CrouchingStates.Disabled;
#endif

            #endregion Debug

            //Set Head Bobbing//
            Ctx.Controllers.Default.HeadBobbing.DisableHeadBobbing();

            //Set Condition
            Ctx.Conditions.Default.cases.isMoving = false;
        }

        protected override void UpdateState()
        {
        }

        protected override void FixedUpdateState()
        {
        }

        protected override void CheckSwitchState()
        {
            if (!Ctx.Conditions.Default.cases.wantsToMove || !Ctx.Conditions.Default.possibilities.canMove)
            {
                SwitchStates(Factory.DefaultCrouchingIdling());
            }
        }

        protected override void InitializeSubState()
        {
        }

        #endregion State Methodes
    }
}