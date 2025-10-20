using System;
using UnityEngine;
using NOS.Player.Data;
using NOS.Player.StateMachine;
using NOS.GameManagers.Input;
using NOS.GameManagers.Settings;
using NOS.Player.Controller;

namespace NOS.Player
{
    #region Required Components

    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]

    #endregion Required Components

    public class PlayerMain : MonoBehaviour
    {
        #region Variables

        #region Debug

#if UNITY_EDITOR
        [SerializeField]
        public PlayerDebug debug = new();
#endif

        #endregion Debug

        #region Data Containers

        [Space, SerializeField]
        private InputDataContainer input;

        [SerializeField]
        private PlayerActions actions = new();

        [Space, SerializeField]
        private PlayerConditions conditions = new();

        [Space, SerializeField]
        private PlayerValues values = new();

        [Space, SerializeField]
        private PlayerReferences references;

        #endregion Data Containers

        #region Controllers

        private PlayerControllers _controllers;

        #endregion Controllers

        #region State Machine

        private PlayerStateContext _stateContext;
        private PlayerStateFactory _stateFactory;

        #endregion State Machine

        #endregion Variables

        #region Private Methodes

        #region Awake

        private void Awake()
        {
            input = InputManager.Instance.CurrentInput;
            _controllers = new PlayerControllers(input, actions, conditions, values, references, SettingsManager.Instance.CurrentSettings);
            SetupStateMachine();
        }

        #region Setups

        private void SetupStateMachine()
        {
            _stateContext = new PlayerStateContext(input, actions, conditions, values, references, _controllers);
            _stateFactory = new PlayerStateFactory(_stateContext);

            #region Setup debug

#if UNITY_EDITOR
            _stateContext.SetDebug(debug);
#endif

            #endregion Setup debug

            //Set first state//
            _stateContext.CurrentState = _stateFactory.Default();
            _stateContext.CurrentState.EnterStates();
        }

        #endregion Setups

        #endregion Awake

        #region Update

        private void Update()
        {
            //General Values Updater//
            _controllers.General.ValuesUpdater.Update();

            _stateContext.CurrentState.UpdateStates();
        }

        #endregion Update

        #region FixedUpdate

        private void FixedUpdate()
        {
            _stateContext.CurrentState.FixedUpdateStates();
        }

        #endregion FixedUpdate

        #region OnDestroy

        private void OnDestroy()
        {
            _controllers.OnDestroy();
        }

        #endregion OnDestroy

        #region Debug

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            _controllers.OnDrawGizmos();
        }
#endif

        #endregion Debug

        #endregion Private Methodes

        #region Public Methodes

        #endregion Public Methodes
    }
}