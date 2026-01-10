using System;
using System.Collections.Generic;
using NOS.Patterns.Singleton;
using Unity.Cinemachine;
using UnityEngine;

namespace NOS.GameplayManagers
{
    [DefaultExecutionOrder(-35)]
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

        private Transform _playerHeadPivot;

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

        private void Start()
        {
            _playerHeadPivot = GameplayReferenceManager.Instance.PlayerReferences.Objects.headPivot.transform;

            SetupCamerasDictionary();

            SetupCameras();

            //Set default camera//
            ChangeCurrentCamera(CameraNames.Default);
        }

        private void SetupCameras()
        {
            //Setup all camera like Default//
            foreach (CinemachineCamera cinemachineCamera in cinemachineCameras)
            {
                cinemachineCamera.Target = new CameraTarget
                {
                    TrackingTarget = _playerHeadPivot,
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