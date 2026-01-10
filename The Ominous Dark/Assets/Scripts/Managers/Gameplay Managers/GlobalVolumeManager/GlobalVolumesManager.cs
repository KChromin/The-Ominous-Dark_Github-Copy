using System;
using System.Collections.Generic;
using NOS.Patterns.Singleton;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace NOS.GameplayManagers
{
    [DefaultExecutionOrder(-55)]
    [RequireComponent(typeof(Volume))]
    [RequireComponent(typeof(CustomPassVolume))]
    public class GlobalVolumesManager : SingletonPersistent<GlobalVolumesManager>
    {
        public GlobalVolumesManagerScriptableObject globalVolumes;

        private Volume _volume;
        private CustomPassVolume[] _customPassVolumes;

        private readonly Dictionary<GlobalVolumeProfileNames, VolumeProfile> _globalVolumeDictionary = new();
        private readonly Dictionary<CustomPassInjectionPoint, CustomPassVolume> _customPassesDictionary = new();

        #region Custom Pass Effects

        #region Stamina

        private Material _staminaOverlay;
        private static readonly int Intensity = Shader.PropertyToID("_Intensity");

        #endregion Stamina

        #endregion Custom Pass Effects

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            _volume = GetComponent<Volume>();
            _customPassVolumes = GetComponents<CustomPassVolume>();

            SetupDictionary();
            SetupCustomPassEffects();

            //todo
            UpdateCurrentVolume(GlobalVolumeProfileNames.TestScene1);
        }

        private void SetupDictionary()
        {
            for (int i = 0; i < globalVolumes.volumesList.Count; i++)
            {
                _globalVolumeDictionary.Add(globalVolumes.volumesList[i].volumeProfileName, globalVolumes.volumesList[i].volumeProfile);
            }

            for (int i = 0; i < _customPassVolumes.Length; i++)
            {
                switch (_customPassVolumes[i].injectionPoint)
                {
                    case CustomPassInjectionPoint.BeforePostProcess:
                        _customPassesDictionary.Add(CustomPassInjectionPoint.BeforePostProcess, _customPassVolumes[i]);
                        break;
                    case CustomPassInjectionPoint.AfterPostProcess:
                        _customPassesDictionary.Add(CustomPassInjectionPoint.AfterPostProcess, _customPassVolumes[i]);
                        break;
                }
            }
        }

        private void SetupCustomPassEffects()
        {
            for (int i = 0; i < _customPassesDictionary.Count; i++)
            {
                if (_customPassesDictionary[CustomPassInjectionPoint.BeforePostProcess])
                {
                }

                if (_customPassesDictionary[CustomPassInjectionPoint.AfterPostProcess])
                {
                    SetupCustomPassEffectsAfterPostProcess();
                }
            }
        }

        private void SetupCustomPassEffectsAfterPostProcess()
        {
            for (int i = 0; i < _customPassesDictionary[CustomPassInjectionPoint.AfterPostProcess].customPasses.Count; i++)
            {
                if (_customPassesDictionary[CustomPassInjectionPoint.AfterPostProcess].customPasses[i].name == "StaminaOverlay")
                {
                    FullScreenCustomPass customPass = (FullScreenCustomPass)_customPassesDictionary[CustomPassInjectionPoint.AfterPostProcess].customPasses[i];
                    _staminaOverlay = customPass.fullscreenPassMaterial;
                }
            }
        }

        #endregion Setup

        #region Public methodes

        public void UpdateStaminaOverlay(float newValue01)
        {
            _staminaOverlay.SetFloat(Intensity, newValue01);
        }

        #endregion Public methodes

        private void UpdateCurrentVolume(GlobalVolumeProfileNames newVolumeProfile)
        {
            _volume.profile = _globalVolumeDictionary[newVolumeProfile];
        }


        private void OnDestroy()
        {
            UpdateStaminaOverlay(0);
        }
    }

    [Serializable]
    public class GlobalVolumeProfilesClass
    {
        public VolumeProfile volumeProfile;
        public GlobalVolumeProfileNames volumeProfileName;
    }

    public enum GlobalVolumeProfileNames
    {
        TestScene1,
        Off
    }
}