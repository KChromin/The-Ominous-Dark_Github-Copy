using NOS.Patterns.StateMachine;
using NOS.Player.Controller.Default;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.StateMachine
{
    public class PlayerStateStanding : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateStanding(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.groundedState = PlayerDebug.StateMachineClass.DefaultStatesClass.GroundedStates.Standing;
#endif

            #endregion Debug

            #region Event

            //Trigger Event//
            Ctx.Actions.Default.OnStandingState?.Invoke();

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
            if (Ctx.Conditions.Default.cases.isCrouching)
            {
                SwitchStates(Factory.DefaultCrouching());
            }
        }

        protected override void InitializeSubState()
        {
            if (Ctx.Conditions.Default.cases.wantsToMove && Ctx.Conditions.Default.possibilities.canMove)
            {
                if (Ctx.Conditions.Default.cases.wantsToRun && Ctx.Conditions.Default.possibilities.canRun)
                {
                    SetSubState(Factory.DefaultStandingRunning());
                }
                else
                {
                    SetSubState(Factory.DefaultStandingWalking());
                }
            }
            else
            {
                SetSubState(Factory.DefaultStandingIdling());
            }
        }

        #endregion State Methodes
    }
}