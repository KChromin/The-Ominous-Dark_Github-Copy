using NOS.GameManagers.Audio;
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
    [DefaultExecutionOrder(-45)]

    #endregion Required Components

    public class PlayerMain : MonoBehaviour
    {
        #region Variables

        #region Debug

#if UNITY_EDITOR
        [field: SerializeField]
        public PlayerDebug Debug { get; set; } = new();
#endif

        #endregion Debug

        #region Data Containers

        [field: Space, SerializeField]
        private InputDataContainer Input { get; set; } = new();

        [field: SerializeField]
        private PlayerActions Actions { get; set; } = new();

        [field: Space, SerializeField]
        private PlayerConditions Conditions { get; set; } = new();

        [field: Space, SerializeField]
        private PlayerValues Values { get; set; } = new();

        [field: Space, SerializeField]
        private PlayerReferences References { get; set; } = new();

        [field: Space, SerializeField]
        private PlayerDynamicReferences DynamicReferences { get; set; } = new();

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
            Input = InputManager.Instance.CurrentInput;
            _controllers = new PlayerControllers(Input, Actions, Conditions, Values, References, DynamicReferences, SettingsManager.Instance, SoundManager.Instance);
            SetupStateMachine();
            SetupRigidbodyIterations();
        }

        #region Setups

        private void SetupStateMachine()
        {
            _stateContext = new PlayerStateContext(Input, Actions, Conditions, Values, References, _controllers);
            _stateFactory = new PlayerStateFactory(_stateContext);

            #region Setup debug

#if UNITY_EDITOR
            _stateContext.SetDebug(Debug);
#endif

            #endregion Setup debug

            //Set first state//
            _stateContext.CurrentState = _stateFactory.Default();
            _stateContext.CurrentState.EnterStates();
        }

        private void SetupRigidbodyIterations()
        {
            Rigidbody rigidBody = References.Components.rigidBody;
            rigidBody.solverIterations = 128;
            rigidBody.solverVelocityIterations = 32;
        }

        #endregion Setups

        #endregion Awake

        #region Update

        private void Update()
        {
            if (Time.timeScale == 0) return;

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

        public PlayerConditions GetPlayerConditions()
        {
            return Conditions;
        }

        public PlayerReferences GetPlayerReferences()
        {
            return References;
        }

        public PlayerDynamicReferences GetPlayerDynamicReferences()
        {
            return DynamicReferences;
        }

        #endregion Public Methodes
    }
}