using System.Collections.Generic;
using NOS.Patterns.StateMachine;

namespace NOS.Player.StateMachine
{
    /*
    How to Add State?
    0. Create new state script
    1. Add State name to Enum
    3. Assign it to dictionary
    4. Create new State return
    */

    public class PlayerStateFactory : StateFactory
    {
        //State dictionary//
        private readonly Dictionary<PlayerStates, HierarchicalStateBase> _states = new();

        //Declare all states to dictionary//
        public PlayerStateFactory(StateContext context)
        {
            _states[PlayerStates.Disabled] = new PlayerStateDisabled(context, this);

            #region Default

            _states[PlayerStates.Default] = new PlayerStateDefault(context, this);

            #region Grounded

            _states[PlayerStates.DefaultGrounded] = new PlayerStateGrounded(context, this);

            #region Standing

            _states[PlayerStates.DefaultStanding] = new PlayerStateStanding(context, this);
            _states[PlayerStates.DefaultStandingIdling] = new PlayerStateStandingIdling(context, this);
            _states[PlayerStates.DefaultStandingWalking] = new PlayerStateStandingWalking(context, this);
            _states[PlayerStates.DefaultStandingRunning] = new PlayerStateStandingRunning(context, this);

            #endregion Standing

            #region Crouching

            _states[PlayerStates.DefaultCrouching] = new PlayerStateCrouching(context, this);
            _states[PlayerStates.DefaultCrouchingIdling] = new PlayerStateCrouchingIdling(context, this);
            _states[PlayerStates.DefaultCrouchingWalking] = new PlayerStateCrouchingWalking(context, this);

            #endregion Crouching

            #endregion Grounded

            #region InAir

            _states[PlayerStates.DefaultInAir] = new PlayerStateInAir(context, this);
            _states[PlayerStates.DefaultInAirRising] = new PlayerStateInAirRising(context, this);
            _states[PlayerStates.DefaultInAirFalling] = new PlayerStateInAirFalling(context, this);

            #endregion InAir

            #endregion Default

            _states[PlayerStates.Dead] = new PlayerStateDead(context, this);
        }

        #region Get State Methodes

        public HierarchicalStateBase Disabled()
        {
            return _states[PlayerStates.Disabled];
        }

        #region Default

        public HierarchicalStateBase Default()
        {
            return _states[PlayerStates.Default];
        }

        #region Grounded

        public HierarchicalStateBase DefaultGrounded()
        {
            return _states[PlayerStates.DefaultGrounded];
        }

        #region Standing

        public HierarchicalStateBase DefaultStanding()
        {
            return _states[PlayerStates.DefaultStanding];
        }

        public HierarchicalStateBase DefaultStandingIdling()
        {
            return _states[PlayerStates.DefaultStandingIdling];
        }

        public HierarchicalStateBase DefaultStandingWalking()
        {
            return _states[PlayerStates.DefaultStandingWalking];
        }

        public HierarchicalStateBase DefaultStandingRunning()
        {
            return _states[PlayerStates.DefaultStandingRunning];
        }

        #endregion Standing

        #region Crouching

        public HierarchicalStateBase DefaultCrouching()
        {
            return _states[PlayerStates.DefaultCrouching];
        }

        public HierarchicalStateBase DefaultCrouchingIdling()
        {
            return _states[PlayerStates.DefaultCrouchingIdling];
        }

        public HierarchicalStateBase DefaultCrouchingWalking()
        {
            return _states[PlayerStates.DefaultCrouchingWalking];
        }

        #endregion Crouching

        #endregion Grounded

        #region InAir

        public HierarchicalStateBase DefaultInAir()
        {
            return _states[PlayerStates.DefaultInAir];
        }

        public HierarchicalStateBase DefaultInAirRising()
        {
            return _states[PlayerStates.DefaultInAirRising];
        }

        public HierarchicalStateBase DefaultInAirFalling()
        {
            return _states[PlayerStates.DefaultInAirFalling];
        }

        #endregion InAir

        #endregion Default

        public HierarchicalStateBase Dead()
        {
            return _states[PlayerStates.Dead];
        }

        #endregion Get State Methodes
    }

    //Player State Names//
    public enum PlayerStates
    {
        Disabled,

        #region Default

        Default,

        //Grounded//
        DefaultGrounded,

        //Standing//
        DefaultStanding,
        DefaultStandingIdling,
        DefaultStandingWalking,
        DefaultStandingRunning,

        //Crouching//
        DefaultCrouching,
        DefaultCrouchingIdling,
        DefaultCrouchingWalking,

        //InAir//
        DefaultInAir,
        DefaultInAirRising,
        DefaultInAirFalling,

        #endregion Default

        Dead
    }
}