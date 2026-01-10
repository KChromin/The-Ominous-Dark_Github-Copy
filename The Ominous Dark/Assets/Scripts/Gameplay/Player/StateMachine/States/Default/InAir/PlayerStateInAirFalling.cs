using NOS.Patterns.StateMachine;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateInAirFalling : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateInAirFalling(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.inAirState = PlayerDebug.StateMachineClass.DefaultStatesClass.InAirStates.Falling;
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
            if (Ctx.Values.General.rigidBodyCurrentVelocityY > 0)
            {
                SwitchStates(Factory.DefaultInAirRising());
            }
        }

        protected override void InitializeSubState()
        {
        }

        #endregion State Methodes
    }
}