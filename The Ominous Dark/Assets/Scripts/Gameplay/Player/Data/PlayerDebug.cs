using System;

namespace NOS.Player.Data
{
    [Serializable]
    public class PlayerDebug
    {
        public StateMachineClass stateMachine = new();

        [Serializable]
        public class StateMachineClass
        {
            #region Main

            public MainStates mainState;

            public enum MainStates
            {
                Disabled,
                Default,
                Dead
            }

            #endregion Main

            #region Default

            public DefaultStatesClass defaultStates = new();

            [Serializable]
            public class DefaultStatesClass
            {
                public GroundingStates groundingState;

                public enum GroundingStates
                {
                    Disabled,
                    Grounded,
                    InAir
                }

                #region Grounded

                public GroundedStates groundedState;

                public enum GroundedStates
                {
                    Disabled,
                    Standing,
                    Crouching
                }

                #region Standing

                public StandingStates standingState;

                public enum StandingStates
                {
                    Disabled,
                    Idling,
                    Walking,
                    Running
                }

                #endregion Standing

                #region Crouching

                public CrouchingStates crouchingState;

                public enum CrouchingStates
                {
                    Disabled,
                    Idling,
                    Walking
                }

                #endregion Crouching

                #endregion Grounded

                #region In Air

                public InAirStates inAirState;

                public enum InAirStates
                {
                    Disabled,
                    Rising,
                    Falling
                }

                #endregion In Air
            }

            #endregion Default
        }
    }
}