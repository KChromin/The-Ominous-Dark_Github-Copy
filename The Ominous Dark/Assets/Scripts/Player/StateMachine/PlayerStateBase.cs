using NOS.Patterns.StateMachine;

namespace NOS.Player.StateMachine
{
    public abstract class PlayerStateBase : HierarchicalStateBase
    {
        protected PlayerStateBase(StateContext currentContext, StateFactory stateFactory) : base(currentContext, stateFactory)
        {
            Ctx = (PlayerStateContext)currentContext;
            Factory = (PlayerStateFactory)stateFactory;
        }

        protected PlayerStateContext Ctx { get; private set; }
        protected PlayerStateFactory Factory { get; private set; }

        #region Debug Gizmos

#if UNITY_EDITOR
        public virtual void OnDrawGizmos()
        {
        }
#endif

        #endregion Debug Gizmos
    }
}