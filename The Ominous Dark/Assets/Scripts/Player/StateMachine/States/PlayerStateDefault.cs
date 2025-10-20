using NOS.GameManagers;
using NOS.Patterns.StateMachine;
using NOS.Player.Controller.Default;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateDefault : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateDefault(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
            IsRootState = true;
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.mainState = PlayerDebug.StateMachineClass.MainStates.Default;

#endif

            #endregion Debug

            CursorManager.Instance.SetCursorForGameplay();
        }

        protected override void ExitState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.mainState = PlayerDebug.StateMachineClass.MainStates.Disabled;

#endif

            #endregion Debug

            CursorManager.Instance.SetCursorForUI();
        }

        protected override void UpdateState()
        {
            //Default
            Ctx.Controllers.Default.Look.Update();
            Ctx.Controllers.Default.Checkers.Update();
            Ctx.Controllers.Default.Movement.Update();
            Ctx.Controllers.Default.Crouch.Update();
        }

        protected override void FixedUpdateState()
        {
            Ctx.Controllers.Default.Movement.ExecuteMovement();
            Ctx.Controllers.Default.Movement.ApplyGravitation();
        }

        protected override void CheckSwitchState()
        {
        }

        protected override void InitializeSubState()
        {
            SetSubState(Ctx.Conditions.Default.cases.isGrounded ? Factory.DefaultGrounded() : Factory.DefaultInAir());
        }

        #endregion State Methodes
    }
}