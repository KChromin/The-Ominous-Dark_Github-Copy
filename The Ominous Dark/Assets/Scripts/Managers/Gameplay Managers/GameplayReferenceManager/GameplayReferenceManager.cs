using System;
using NOS.Patterns.Singleton;
using NOS.Player;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.GameplayManagers
{
    //Gathers references for gameplay objects//
    //Gives methods to access info from other assembly definitions//
    [DefaultExecutionOrder(-40)]
    public class GameplayReferenceManager : Singleton<GameplayReferenceManager>
    {
        #region Player

        public Transform PlayerTransform { get; private set; }
        public PlayerConditions PlayerConditions { get; private set; }
        public PlayerReferences PlayerReferences { get; private set; }
        public PlayerDynamicReferences PlayerDynamicReferences { get; private set; }

        #endregion Player

        protected override void Awake()
        {
            base.Awake();
            SetupPlayer();
        }

        #region Player

        private void SetupPlayer()
        {
            PlayerTransform = GameObject.FindWithTag("Player").transform;
            PlayerMain playerMain = PlayerTransform.GetComponent<PlayerMain>();

            PlayerConditions = playerMain.GetPlayerConditions();
            PlayerReferences = playerMain.GetPlayerReferences();
            PlayerDynamicReferences = playerMain.GetPlayerDynamicReferences();
        }

        #endregion Player
    }
}