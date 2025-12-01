using NOS.Patterns.StateMachine;
using NOS.Player.Controller.Default;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateCrouchingIdling : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateCrouchingIdling(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.crouchingState = PlayerDebug.StateMachineClass.DefaultStatesClass.CrouchingStates.Idling;
#endif

            #endregion Debug

            //Set Movement Speed//
            Ctx.Controllers.Default.Movement.SetMovementParameters(PlayerControllerMovement.MovementStates.CrouchIdle);

            //Set Head Bobbing//
            Ctx.Controllers.Default.HeadBobbing.SetHeadBobbingParameters(PlayerControllerHeadBobbingDefault.HeadBobbingStates.CrouchIdle);
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
        }

        protected override void UpdateState()
        {
        }

        protected override void FixedUpdateState()
        {
        }

        protected override void CheckSwitchState()
        {
            if (Ctx.Conditions.Default.cases.wantsToMove && Ctx.Conditions.Default.possibilities.canMove)
            {
                SwitchStates(Factory.DefaultCrouchingWalking());
            }
        }

        protected override void InitializeSubState()
        {
        }

        #endregion State Methodes
    }
}