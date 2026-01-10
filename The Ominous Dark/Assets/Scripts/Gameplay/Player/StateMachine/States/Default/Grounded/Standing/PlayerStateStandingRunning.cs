using NOS.Patterns.StateMachine;
using NOS.Player.Controller.Default;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateStandingRunning : PlayerStateBase
    {
        #region State Methodes

        public PlayerStateStandingRunning(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        protected override void EnterState()
        {
            #region Debug

#if UNITY_EDITOR

            Ctx.Debug.stateMachine.defaultStates.standingState = PlayerDebug.StateMachineClass.DefaultStatesClass.StandingStates.Running;
#endif

            #endregion Debug

            #region Event

            //Trigger Event//
            Ctx.Actions.Default.OnStandingRunningState?.Invoke();

            #endregion Event
            
            //Set Movement Speed//
            Ctx.Controllers.Default.Movement.SetMovementParameters(PlayerControllerMovement.MovementStates.Run);
            
            //Set Head Bobbing//
            Ctx.Controllers.Default.HeadBobbing.SetHeadBobbingState(PlayerControllerHeadBobbingDefault.HeadBobbingStates.Run);
            
            //Set Condition
            Ctx.Conditions.Default.cases.isMoving = true;
            Ctx.Conditions.Default.cases.isRunning = true;
            
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
            
            //Set Condition
            Ctx.Conditions.Default.cases.isMoving = false;
            Ctx.Conditions.Default.cases.isRunning = false;
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
                if (!Ctx.Conditions.Default.cases.wantsToRun || !Ctx.Conditions.Default.possibilities.canRun || Ctx.Conditions.Default.cases.staminaWasFullyDepleted)
                {
                    SwitchStates(Factory.DefaultStandingWalking());
                }
            }
            else
            {
                SwitchStates(Factory.DefaultStandingIdling());
            }
        }

        protected override void InitializeSubState()
        {
        }

        #endregion State Methodes
    }
}