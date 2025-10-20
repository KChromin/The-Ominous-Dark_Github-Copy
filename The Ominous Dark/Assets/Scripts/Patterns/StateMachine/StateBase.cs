namespace NOS.Patterns.StateMachine
{
    public abstract class StateBase
    {
        protected StateBase(StateContext currentContext, StateFactory stateFactory)
        {
            Context = currentContext;
            StatesFactory = stateFactory;
        }

        //Context and factory//
        protected StateContext Context { get; }
        protected StateFactory StatesFactory { get; }

        //When Entering State//
        protected abstract void EnterState();

        //When exiting state//
        protected abstract void ExitState();

        //Update Function//
        protected abstract void UpdateState();

        //Fixed Update Function//
        protected abstract void FixedUpdateState();

        //Check for state switching//
        protected abstract void CheckSwitchState();

        //Enter State, and Initialize Substate//
        public virtual void EnterStates()
        {
            EnterState();
        }

        //Updates state//
        public virtual void UpdateStates()
        {
            //Check for switch//
            CheckSwitchStates();

            //Update Functions//
            UpdateState();
        }

        //Like Update but in fixed update//
        public virtual void FixedUpdateStates()
        {
            //Update Functions//
            FixedUpdateState();
        }

        //Switches main and substates//
        protected virtual void CheckSwitchStates()
        {
            //Check for switch//
            CheckSwitchState();
        }

        //Exits main and sub states//
        protected virtual void ExitStates()
        {
            //Call exit state//
            ExitState();
        }

        //Switch state//
        protected void SwitchStates(StateBase newState)
        {
            //Exit actual state//
            ExitStates();

            //Set new root//
            Context.CurrentState = newState;
            //Enter new state//
            Context.CurrentState.EnterStates();
        }
    }
}