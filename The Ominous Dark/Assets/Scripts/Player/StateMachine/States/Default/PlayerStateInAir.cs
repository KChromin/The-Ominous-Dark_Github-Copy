using NOS.Patterns.StateMachine;
using NOS.Player.Controller.Default;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateInAir : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateInAir(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.groundingState = PlayerDebug.StateMachineClass.DefaultStatesClass.GroundingStates.InAir;
#endif

            #endregion Debug

            #region Event

            //Trigger Event//
            Ctx.Actions.Default.OnInAirState?.Invoke();

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
            if (Ctx.Conditions.Default.cases.isGrounded)
            {
                SwitchStates(Factory.DefaultGrounded());
            }
        }

        protected override void InitializeSubState()
        {
            SetSubState(Ctx.Values.General.rigidBodyCurrentVelocityY <= 0 ? Factory.DefaultInAirFalling() : Factory.DefaultInAirRising());
        }

        #endregion State Methodes
    }
}