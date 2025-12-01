using NOS.GameManagers.Input;
using NOS.GameManagers.Settings;
using NOS.Player.Controller.General;
using NOS.Player.Controller.Default;
using NOS.Player.Data;

namespace NOS.Player.Controller
{
    //Creates, and keeps all player controllers in one place//
    //Additionally it enables possibility of using OnDrawGizmos for controllers/

    public class PlayerControllers
    {
        public PlayerControllers(InputDataContainer input, PlayerActions actions, PlayerConditions conditions, PlayerValues values, PlayerReferences references, SettingsContainers settings)
        {
            General = new GeneralControllersClass(actions, values, references);
            Default = new DefaultControllersClass(input, actions, conditions, values, references, General, settings);
        }

        #region General

        public GeneralControllersClass General { get; private set; }

        public class GeneralControllersClass
        {
            public GeneralControllersClass(PlayerActions actions, PlayerValues values, PlayerReferences references)
            {
                ValuesUpdater = new PlayerControllerGeneralValuesUpdater(values, references);
                Head = new PlayerControllerGeneralHead(references);
            }

            //Values Updater//
            public readonly PlayerControllerGeneralValuesUpdater ValuesUpdater;

            public readonly PlayerControllerGeneralHead Head;
        }

        #endregion General

        #region Default

        public DefaultControllersClass Default { get; private set; }

        public class DefaultControllersClass
        {
            public DefaultControllersClass(InputDataContainer input, PlayerActions actions, PlayerConditions conditions, PlayerValues values, PlayerReferences references, GeneralControllersClass generalControllers, SettingsContainers settings)
            {
                Look = new PlayerControllerLook(input, conditions, references, settings);
                Checkers = new PlayerControllerCheckersDefault(conditions, values, references);
                Movement = new PlayerControllerMovement(input, actions, conditions, values, references);
                Crouch = new PlayerControllerCrouch(input, conditions, references, actions);
                Jump = new PlayerControllerJump(input, conditions, references, actions);
                HeadBobbing = new PlayerControllerHeadBobbingDefault(references, generalControllers, settings, actions);
                Interaction = new PlayerControllerDefaultInteraction(input, conditions, references);
            }

            //Look//
            public readonly PlayerControllerLook Look;

            //Checkers//
            public readonly PlayerControllerCheckersDefault Checkers;

            //Movement//
            public readonly PlayerControllerMovement Movement;

            //Crouch//
            public readonly PlayerControllerCrouch Crouch;

            //Jump//
            public readonly PlayerControllerJump Jump;

            //Head Bobbing//
            public readonly PlayerControllerHeadBobbingDefault HeadBobbing;

            //Interaction//
            public readonly PlayerControllerDefaultInteraction Interaction;
        }

        #endregion Default

        //Mainly for unsubscribing from events
        public void OnDestroy()
        {
            Default.Movement.OnDestroy();
            Default.Crouch.OnDestroy();
        }

        #region OnGizmoDraw

#if UNITY_EDITOR

        public void OnDrawGizmos()
        {
            Default.Checkers.OnDrawGizmos();
            Default.Movement.OnDrawGizmos();
        }

#endif

        #endregion OnGizmoDraw
    }
}