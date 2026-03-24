using System;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using SteamAudio;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace NOS.GameManagers.Audio
{
    [RequireComponent(typeof(StudioEventEmitter))]
    [RequireComponent(typeof(SteamAudioSource))]
    [RequireComponent(typeof(SteamAudioReflectionsDistanceLimiter))]
    [RequireComponent(typeof(SphereCollider))]
    public class SoundInstanceController : MonoBehaviour
    {
        private StudioEventEmitter _emitter;
        private SteamAudioSource _steamAudioSource;
        private SteamAudioReflectionsDistanceLimiter _reflectionLimiter;
        private STOP_MODE _currentStopMode;
        public SoundManager.SoundType currentSoundType;

        private void Awake()
        {
            _emitter = GetComponent<StudioEventEmitter>();
            _steamAudioSource = GetComponent<SteamAudioSource>();
            _reflectionLimiter = GetComponent<SteamAudioReflectionsDistanceLimiter>();
        }

        public void SetupInstance(SoundInstanceReference instanceReference, SoundInstanceParameters instanceParameters)
        {
            currentSoundType = instanceReference.soundType;

            _emitter.EventReference = instanceReference.eventReference;
            _emitter.RefreshEvent();

            switch (instanceReference.eventStopMode)
            {
                default:
                case STOP_MODE.ALLOWFADEOUT:
                    _emitter.AllowFadeout = true;
                    break;
                case STOP_MODE.IMMEDIATE:
                    _emitter.AllowFadeout = false;
                    break;
            }

            //Steam audio
            _steamAudioSource.occlusion = instanceParameters.enableOcclusion;
            _steamAudioSource.occlusionType = OcclusionType.Volumetric;
            _steamAudioSource.occlusionSamples = instanceParameters.occlusionSamples;
            _steamAudioSource.occlusionRadius = instanceParameters.occlusionRadius;
            _steamAudioSource.transmission = instanceParameters.enableTransmissions;
            _steamAudioSource.maxTransmissionSurfaces = instanceParameters.transmissionsMaxSurfaces;
            _steamAudioSource.reflections = instanceParameters.enableReflections;

            if (_steamAudioSource.reflections && instanceParameters.useReflectionLimiter)
            {
                _reflectionLimiter.UpdateEnabledness(true);
                _reflectionLimiter.UpdateParameters(instanceParameters.reflectionRange, instanceParameters.falloffCurve);
            }
            else
            {
                _reflectionLimiter.UpdateEnabledness(false);
            }
        }

        public void Play()
        {
            _emitter.Play();
        }

        public void Stop()
        {
            _emitter.Stop();
            _reflectionLimiter.Disable();
        }

        public void SetParameter(string parameterName, float parameterValue)
        {
            _emitter.SetParameter(parameterName, parameterValue);
        }

        private void OnDestroy()
        {
            _emitter.AllowFadeout = false;
            _emitter.Stop();
            SoundManager.Instance.UnregisterInstanceFromList(this);
        }

        public void OnParentDestroy()
        {
            SoundManager.Instance.DestroySoundInstance(this);
        }
    }

    [Serializable]
    public class SoundInstanceReference
    {
        [Header("Sound Type")]
        public SoundManager.SoundType soundType = SoundManager.SoundType.Sfx;

        [Header("FMOD Emitter")]
        public EventReference eventReference;
        public STOP_MODE eventStopMode = STOP_MODE.ALLOWFADEOUT;
    }


    [Serializable]
    public class SoundInstanceParameters
    {
        [Header("Steam Audio")]
        public bool enableOcclusion = true;
        [Range(1, 16)]
        public int occlusionSamples = 8;
        [Range(0.01f, 10f)]
        public float occlusionRadius = 1f;
        [Space]
        public bool enableTransmissions = true;
        [Range(1, 8)]
        public int transmissionsMaxSurfaces = 8;
        [Space]
        public bool enableReflections;

        [Header("Reflection Limiter")]
        public bool useReflectionLimiter = true;
        [Range(0, 150)]
        public int reflectionRange = 20;
        public AnimationCurve falloffCurve = new()
        {
            keys = new Keyframe[]
            {
                new(0, 1, 0, -1.75f),
                new(1, 0, 0, 0)
            },
            preWrapMode = WrapMode.Clamp,
            postWrapMode = WrapMode.Clamp
        };
    }
}