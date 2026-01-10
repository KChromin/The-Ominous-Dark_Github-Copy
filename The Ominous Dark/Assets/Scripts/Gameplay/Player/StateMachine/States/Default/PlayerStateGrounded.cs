using NOS.Patterns.StateMachine;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateGrounded : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateGrounded(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.groundingState = PlayerDebug.StateMachineClass.DefaultStatesClass.GroundingStates.Grounded;

#endif

            #endregion Debug

            #region Event

            //Trigger Event//
            Ctx.Actions.Default.OnGroundedState?.Invoke();

            #endregion Event
        }

        protected override void ExitState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.groundingState = PlayerDebug.StateMachineClass.DefaultStatesClass.GroundingStates.Disabled;
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
            if (!Ctx.Conditions.Default.cases.isGrounded)
            {
                SwitchStates(Factory.DefaultInAir());
            }
        }

        protected override void InitializeSubState()
        {
            if (Ctx.Conditions.Default.cases.isCrouching)
            {
                SetSubState(Factory.DefaultCrouching());
            }
            else
            {
                SetSubState(Factory.DefaultStanding());
            }
        }

        #endregion State Methodes
    }
}