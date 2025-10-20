using NOS.Patterns.StateMachine;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateCrouching : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateCrouching(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.groundedState = PlayerDebug.StateMachineClass.DefaultStatesClass.GroundedStates.Crouching;
#endif

            #endregion Debug

            #region Event

            //Trigger Event//
            Ctx.Actions.Default.OnCrouchingState?.Invoke();

            #endregion Event
        }

        protected override void ExitState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.groundedState = PlayerDebug.StateMachineClass.DefaultStatesClass.GroundedStates.Disabled;
#endif

            #endregion Debug
        }

        protected override void UpdateState()
        {
        }

        protected override void FixedUpdateState()
        {
        }

        protected override void CheckSwitchState()
        {
            if (!Ctx.Conditions.Default.cases.isCrouching)
            {
                SwitchStates(Factory.DefaultStanding());
            }
        }

        protected override void InitializeSubState()
        {
            if (Ctx.Conditions.Default.cases.wantsToMove && Ctx.Conditions.Default.possibilities.canMove)
            {
                SetSubState(Factory.DefaultCrouchingWalking());
            }
            else
            {
                SetSubState(Factory.DefaultCrouchingIdling());
            }
        }

        #endregion State Methodes
    }
}