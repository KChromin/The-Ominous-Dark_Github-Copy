using System.Collections.Generic;
using NOS.Patterns.Singleton;
using Unity.Cinemachine;
using UnityEngine;

namespace NOS.GameplayManagers
{
    public class CameraManager : Singleton<CameraManager>
    {
        [Header("Main Camera")]
        [SerializeField]
        private Camera mainCamera;

        [Header("Cinemachine Cameras")]
        [SerializeField]
        private List<CinemachineCamera> cinemachineCameras;

        public void SetupPlayerHead(Transform playerHead)
        {
            foreach (CinemachineCamera cinemachineCamera in cinemachineCameras)
            {
                cinemachineCamera.Target = new CameraTarget
                {
                    TrackingTarget = playerHead,
                    LookAtTarget = null,
                    CustomLookAtTarget = false
                };
            }
        }
        
        #region Temporary setup

        public Transform tempPlayerHead;

        protected override void Awake()
        {
            base.Awake();

            SetupPlayerHead(tempPlayerHead);
        }

        #endregion Temporary setup
    }
}