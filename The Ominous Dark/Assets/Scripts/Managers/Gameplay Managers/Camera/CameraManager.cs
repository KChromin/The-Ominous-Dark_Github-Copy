using System.Collections.Generic;
using NOS.Patterns.Singleton;
using Unity.Cinemachine;
using UnityEngine;

namespace NOS.GameplayManagers
{
    [DefaultExecutionOrder(-50)]
    public class CameraManager : Singleton<CameraManager>
    {
        [Header("Currently Selected Camera")]
        public CameraNames currentCamera;

        [Header("Main Camera")]
        [SerializeField]
        private Camera mainCamera;

        [Header("Cinemachine Cameras")]
        [SerializeField]
        private List<CinemachineCamera> cinemachineCameras;

        public enum CameraNames
        {
            Default //Default Camera
        }

        #region Dictionary

        private Dictionary<CameraNames, CinemachineCamera> _camerasDictionary;

        private void SetupCamerasDictionary()
        {
            _camerasDictionary = new Dictionary<CameraNames, CinemachineCamera>()
            {
                { CameraNames.Default, cinemachineCameras[0] } //Default Camera
            };
        }

        #endregion Dictionary

        #region Setup

        protected override void Awake()
        {
            base.Awake();
            SetupCamerasDictionary();
        }

        //Must be called by player//
        public void Setup(Transform playerHeadPivot)
        {
            SetupCameras(playerHeadPivot);

            //Set default camera//
            ChangeCurrentCamera(CameraNames.Default);
        }

        private void SetupCameras(Transform playerHeadPivot)
        {
            //Setup all camera like Default//
            foreach (CinemachineCamera cinemachineCamera in cinemachineCameras)
            {
                cinemachineCamera.Target = new CameraTarget
                {
                    TrackingTarget = playerHeadPivot,
                    LookAtTarget = null,
                    CustomLookAtTarget = false
                };
            }

            //Setups for Specific cameras//
        }

        #endregion Setup

        public void ChangeCurrentCamera(CameraNames newCamera)
        {
            currentCamera = newCamera;

            //Disable all cameras//
            for (int i = 0; i < cinemachineCameras.Count; i++)
            {
                cinemachineCameras[i].gameObject.SetActive(false);
            }

            //Enable correct camera//
            _camerasDictionary[newCamera].gameObject.SetActive(true);
        }
    }
}