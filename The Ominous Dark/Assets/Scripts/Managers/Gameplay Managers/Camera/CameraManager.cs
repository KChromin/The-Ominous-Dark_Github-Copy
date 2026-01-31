using System;
using System.Collections.Generic;
using NOS.GameManagers.Settings;
using NOS.Patterns.Singleton;
using Unity.Cinemachine;
using UnityEngine;

namespace NOS.GameplayManagers
{
    [DefaultExecutionOrder(-49)]
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

        private SettingsContainers _settings;

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

        #region Default Camera

        private CinemachineCamera _defaultCamera;

        #region FOV

        private const float SmoothUpdateTimeDefaultCameraFOV = 0.12f;
        private float _defaultCameraBaseFOV;
        private bool _defaultCameraUpdateFOVUpdateIsNeeded;
        private float _defaultCameraUpdateFOVTarget;
        private float _defaultCameraUpdateFOVCalculations;

        #endregion FOV

        #endregion Default Camera

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            _settings = SettingsManager.Instance.CurrentSettings;
            SettingsManager.Instance.OnSettingsUpdate += OnSettingsUpdate;
        }

        private void Start()
        {
            _playerHeadPivot = GameplayReferenceManager.Instance.PlayerReferences.Objects.headPivot.transform;

            SetupCamerasDictionary();

            SetupCameras();

            //Set default camera//
            ChangeCurrentCamera(CameraNames.Default);

            _defaultCamera = _camerasDictionary[CameraNames.Default];
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
            UpdateBaseFieldOfViews();
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

        private void Update()
        {
            #region Default Camera

            //FOV
            UpdateDefaultCameraFOVInTime();

            #endregion Default Camera
        }

        #region Field of view

        private void UpdateBaseFieldOfViews()
        {
            foreach (CinemachineCamera cinemachineCamera in cinemachineCameras)
            {
                _defaultCameraBaseFOV = _settings.game.fieldOfView;
                cinemachineCamera.Lens.FieldOfView = _settings.game.fieldOfView;
            }
        }


        public void UpdateDefaultCameraFieldOfView(float additionalFOV)
        {
            _defaultCameraUpdateFOVTarget = _defaultCameraBaseFOV + additionalFOV;
            _defaultCameraUpdateFOVUpdateIsNeeded = true;
        }

        public void UpdateDefaultCameraFieldOfViewTryToReset()
        {
            if (!Mathf.Approximately(_defaultCamera.Lens.FieldOfView, _defaultCameraBaseFOV))
            {
                _defaultCameraUpdateFOVTarget = _defaultCameraBaseFOV;
                _defaultCameraUpdateFOVUpdateIsNeeded = true;
            }
        }

        private void UpdateDefaultCameraFOVInTime()
        {
            if (!_defaultCameraUpdateFOVUpdateIsNeeded) return;
            if (!Mathf.Approximately(_defaultCamera.Lens.FieldOfView, _defaultCameraUpdateFOVTarget))
            {
                _defaultCamera.Lens.FieldOfView = Mathf.SmoothDamp(_defaultCamera.Lens.FieldOfView, _defaultCameraUpdateFOVTarget, ref _defaultCameraUpdateFOVCalculations, SmoothUpdateTimeDefaultCameraFOV);

                if (Mathf.Approximately(_defaultCamera.Lens.FieldOfView, _defaultCameraUpdateFOVTarget))
                {
                    _defaultCameraUpdateFOVUpdateIsNeeded = false;
                    _defaultCamera.Lens.FieldOfView = _defaultCameraUpdateFOVTarget;
                }
            }
        }

        #endregion Field of view

        private void OnSettingsUpdate()
        {
            UpdateBaseFieldOfViews();
        }

        private void OnDestroy()
        {
            SettingsManager.Instance.OnSettingsUpdate -= OnSettingsUpdate;
        }
    }
}