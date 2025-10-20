namespace NOS.Patterns.StateMachine
{
    public abstract class HierarchicalStateBase : StateBase
    {
        protected HierarchicalStateBase(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
        }

        //Is a state a root//
        protected bool IsRootState { get; set; }

        //Sub and super states//
        private HierarchicalStateBase CurrentSubState { get; set; }
        private HierarchicalStateBase CurrentSuperState { get; set; }

        //Initialize Sub-States//
        protected abstract void InitializeSubState();

        //Enter State, and Initialize SubState//
        public override void EnterStates()
        {
            base.EnterStates();

            InitializeSubState();
        }

        //Updates main and substates//
        public override void UpdateStates()
        {
            base.UpdateStates();

            //Update All Sub States//
            CurrentSubState?.UpdateStates();
        }

        //Like Update but in fixed update//
        public override void FixedUpdateStates()
        {
            base.FixedUpdateStates();

            //Update All Sub States//
            CurrentSubState?.FixedUpdateStates();
        }

        //Switches main and substates//
        protected override void CheckSwitchStates()
        {
            base.CheckSwitchStates();

            CurrentSubState?.CheckSwitchStates();
        }

        //Exits main and substates//
        protected override void ExitStates()
        {
            base.ExitStates();

            //Exit State in substates//
            CurrentSubState?.ExitStates();
        }

        //Switch state//
        protected void SwitchStates(HierarchicalStateBase newState)
        {
            //Exit current state//
            ExitStates();

            if (IsRootState)
            {
                //Set new root//
                Context.CurrentState = newState;
                //Enter new state//
                Context.CurrentState.EnterStates();
            }
            else
            {
                CurrentSuperState?.SetSubState(newState);
            }
        }

        //Set superstate//
        protected void SetSuperState(HierarchicalStateBase newSuperState)
        {
            CurrentSuperState = newSuperState;
        }

        //Set substate//
        protected void SetSubState(HierarchicalStateBase newSubState)
        {
            CurrentSubState = newSubState;
            CurrentSubState.SetSuperState(this);
            CurrentSubState.EnterStates();
        }
    }
}