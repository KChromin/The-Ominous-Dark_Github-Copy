using NOS.Patterns.StateMachine;
using NOS.Player.Controller.Default;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.StateMachine
{
    public class PlayerStateStandingIdling : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateStandingIdling(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.standingState = PlayerDebug.StateMachineClass.DefaultStatesClass.StandingStates.Idling;
#endif

            #endregion Debug

            #region Event

            //Trigger Event//
            Ctx.Actions.Default.OnStandingIdlingState?.Invoke();

            #endregion Event

            //Set Movement Speed//
            Ctx.Controllers.Default.Movement.SetMovementParameters(PlayerControllerMovement.MovementStates.Idle);
            
            //Set Head Bobbing//
            Ctx.Controllers.Default.HeadBobbing.SetHeadBobbingState(PlayerControllerHeadBobbingDefault.HeadBobbingStates.Idle);
        }

        protected override void ExitState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.standingState = PlayerDebug.StateMachineClass.DefaultStatesClass.StandingStates.Disabled;
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
                if (Ctx.Conditions.Default.cases.wantsToRun && Ctx.Conditions.Default.possibilities.canRun && !Ctx.Conditions.Default.cases.staminaWasFullyDepleted)
                {
                    SwitchStates(Factory.DefaultStandingRunning());
                }
                else
                {
                    SwitchStates(Factory.DefaultStandingWalking());
                }
            }
        }

        protected override void InitializeSubState()
        {
        }

        #endregion State Methodes
    }
}