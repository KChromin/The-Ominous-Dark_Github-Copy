using System;
using FMOD.Studio;
using FMODUnity;
using NOS.GameManagers.Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.Controllers.Interactions
{
    public class InteractableRadioPlayer : InteractableBase
    {
        [Space]
        [SerializeField]
        private SoundInstanceReference soundReference;
        [Space]
        [SerializeField]
        private SoundInstanceParameters soundParameters;

        [SerializeField]
        private Transform audioSourceObject;

        private SoundManager _soundManager;
        private SoundInstanceController _soundInstance;

        private bool _isPlaying;

        private void Awake()
        {
            _soundManager = SoundManager.Instance;
            Interact();
        }

        protected override void Interact()
        {
            if (_isPlaying)
            {
                _soundManager.DestroySoundInstance(_soundInstance);
                _soundInstance = null;
                _isPlaying = false;
            }
            else
            {
                _soundInstance = _soundManager.CreateSoundInstance(soundReference, soundParameters, audioSourceObject);
                _soundInstance.Play();
                _isPlaying = true;
            }
        }

        private void OnDestroy()
        {
            if (_soundInstance)
            {
                _soundInstance.OnParentDestroy();
            }
        }
    }
}