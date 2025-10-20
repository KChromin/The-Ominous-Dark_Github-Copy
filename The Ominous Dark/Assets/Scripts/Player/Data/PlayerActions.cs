using System;

namespace NOS.Player.Data
{
    [Serializable]
    public class PlayerActions
    {
        public Action OnDeadState;

        #region Default

        public DefaultActionsClass Default { get; private set; } = new();

        [Serializable]
        public class DefaultActionsClass
        {
            #region State Machine

            public Action OnGroundedState;

            public Action OnStandingState;
            public Action OnStandingIdlingState;
            public Action OnStandingWalkingState;
            public Action OnStandingRunningState;

            public Action OnCrouchingState;
            public Action OnCrouchingIdlingState;
            public Action OnCrouchingWalkingState;

            public Action OnInAirState;
            public Action OnInAirRisingState;
            public Action OnInAirFallingState;

            #endregion State Machine
        }

        #endregion Default
    }
}