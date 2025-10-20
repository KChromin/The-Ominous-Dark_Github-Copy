using NOS.Patterns.StateMachine;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateInAirRising : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateInAirRising(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.inAirState = PlayerDebug.StateMachineClass.DefaultStatesClass.InAirStates.Rising;
#endif

            #endregion Debug
        }

        protected override void ExitState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.inAirState = PlayerDebug.StateMachineClass.DefaultStatesClass.InAirStates.Disabled;
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
            if (Ctx.Values.General.rigidBodyCurrentVelocityY <= 0)
            {
                SwitchStates(Factory.DefaultInAirFalling());
            }
        }

        protected override void InitializeSubState()
        {
        }

        #endregion State Methodes
    }
}