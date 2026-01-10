using NOS.Patterns.StateMachine;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateDisabled : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateDisabled(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
            IsRootState = true;
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.mainState = PlayerDebug.StateMachineClass.MainStates.Disabled;

#endif

            #endregion Debug
        }

        protected override void ExitState()
        {
        }

        protected override void UpdateState()
        {
        }

        protected override void FixedUpdateState()
        {
        }

        protected override void CheckSwitchState()
        {
        }

        protected override void InitializeSubState()
        {
        }

        #endregion State Methodes
    }
}