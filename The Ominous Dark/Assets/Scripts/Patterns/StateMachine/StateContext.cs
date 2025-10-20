namespace NOS.Patterns.StateMachine
{
    public abstract class StateContext
    {
        public StateBase CurrentState { get; set; }
    }
}
