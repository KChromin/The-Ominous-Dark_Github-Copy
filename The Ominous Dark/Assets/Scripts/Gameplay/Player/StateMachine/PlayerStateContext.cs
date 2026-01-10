using NOS.Patterns.StateMachine;
using NOS.GameManagers.Input;
using NOS.Player.Controller;
using NOS.Player.Data;

namespace NOS.Player.StateMachine
{
    public class PlayerStateContext : StateContext
    {
        public PlayerStateContext(InputDataContainer input, PlayerActions actions, PlayerConditions conditions, PlayerValues values, PlayerReferences references, PlayerControllers controllers)
        {
            Input = input;
            Actions = actions;
            Conditions = conditions;
            Values = values;
            References = references;
            Controllers = controllers;
        }

        //Data
        public readonly InputDataContainer Input;

        public readonly PlayerActions Actions;

        public readonly PlayerConditions Conditions;

        public readonly PlayerValues Values;

        public readonly PlayerReferences References;

        //Controllers
        public readonly PlayerControllers Controllers;

        #region Debug

#if UNITY_EDITOR


        public PlayerDebug Debug;

        public void SetDebug(PlayerDebug debug)
        {
            Debug = debug;
        }
#endif

        #endregion Debug
    }
}