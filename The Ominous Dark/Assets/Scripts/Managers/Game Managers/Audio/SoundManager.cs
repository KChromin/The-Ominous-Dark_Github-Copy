using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using NOS.Patterns.Singleton;
using UnityEngine.Pool;

namespace NOS.GameManagers.Audio
{
    [DefaultExecutionOrder(-89)]
    public class SoundManager : SingletonPersistent<SoundManager>
    {
        [SerializeField]
        private List<GameObject> sfxInstances = new();
        [SerializeField]
        private List<GameObject> voiceInstances = new();
        [SerializeField]
        private List<GameObject> musicInstances = new();

        private ObjectPool<GameObject> _soundInstancesPool;
        
        [Space, SerializeField]
        private ObjectsClass objects;

        [Serializable]
        private class ObjectsClass
        {
            public Transform poolObjectsParent;
            public GameObject soundInstancePrefab;
        }

        private const int MaxInstances = 128;
        private const int DefaultPoolCapacity = 16;

        public enum SoundType
        {
            Sfx,
            Voice,
            Music
        }

        protected override void Awake()
        {
            base.Awake();

            //Create Pool//
            _soundInstancesPool = new ObjectPool<GameObject>(CreateSoundInstanceObject, OnGetSoundInstanceObject, OnReleaseSoundInstanceObject, OnDestroySoundInstanceObject, true, DefaultPoolCapacity, MaxInstances);
        }

        #region Pool Methodes

        private GameObject CreateSoundInstanceObject()
        {
            GameObject soundInstance = Instantiate(objects.soundInstancePrefab);

            return soundInstance;
        }
        
        private static void OnGetSoundInstanceObject(GameObject soundInstance)
        {
            soundInstance.SetActive(true);
        }

        private void OnReleaseSoundInstanceObject(GameObject soundInstance)
        {
            soundInstance.GetComponent<SoundInstanceController>().Stop();
            soundInstance.transform.SetParent(objects.poolObjectsParent);
            soundInstance.SetActive(false);
        }

        private static void OnDestroySoundInstanceObject(GameObject soundInstance)
        {
            Destroy(soundInstance);
        }

        public void UnregisterInstanceFromList(SoundInstanceController instance)
        {
            switch (instance.currentSoundType)
            {
                default:
                case SoundType.Sfx:
                    sfxInstances.Remove(instance?.gameObject);
                    break;
                case SoundType.Voice:
                    voiceInstances.Remove(instance?.gameObject);
                    break;
                case SoundType.Music:
                    musicInstances.Remove(instance?.gameObject);
                    break;
            }
        }

        #endregion Pool Methodes

        #region Create Instance

        public SoundInstanceController CreateSoundInstance(SoundInstanceReference instanceReference, SoundInstanceParameters instanceParameters, Transform parent)
        {
            GameObject soundInstance = _soundInstancesPool.Get();
            SoundInstanceController soundInstanceController = soundInstance.GetComponent<SoundInstanceController>();

            switch (instanceReference.soundType)
            {
                default:
                case SoundType.Sfx:
                    sfxInstances.Add(soundInstance);
                    break;
                case SoundType.Voice:
                    voiceInstances.Add(soundInstance);
                    break;
                case SoundType.Music:
                    musicInstances.Add(soundInstance);
                    break;
            }

            soundInstanceController.SetupInstance(instanceReference, instanceParameters);

            soundInstance.transform.SetParent(parent);
            soundInstance.transform.localPosition = Vector3.zero;
            soundInstance.transform.localRotation = Quaternion.identity;

            return soundInstanceController;
        }
        
        public SoundInstanceController CreateSoundInstance(SoundInstanceReference instanceReference, SoundInstanceParameters instanceParameters, Transform parent, Vector3 localOffset)
        {
            GameObject soundInstance = _soundInstancesPool.Get();
            SoundInstanceController soundInstanceController = soundInstance.GetComponent<SoundInstanceController>();

            switch (instanceReference.soundType)
            {
                default:
                case SoundType.Sfx:
                    sfxInstances.Add(soundInstance);
                    break;
                case SoundType.Voice:
                    voiceInstances.Add(soundInstance);
                    break;
                case SoundType.Music:
                    musicInstances.Add(soundInstance);
                    break;
            }

            soundInstanceController.SetupInstance(instanceReference, instanceParameters);

            soundInstance.transform.SetParent(parent);
            soundInstance.transform.localPosition = localOffset;
            soundInstance.transform.localRotation = Quaternion.identity;

            return soundInstanceController;
        }

        public SoundInstanceController CreateSoundInstance(SoundInstanceReference instanceReference, SoundInstanceParameters instanceParameters, Vector3 position, Quaternion rotation)
        {
            GameObject soundInstance = _soundInstancesPool.Get();
            SoundInstanceController soundInstanceController = soundInstance.GetComponent<SoundInstanceController>();

            switch (instanceReference.soundType)
            {
                default:
                case SoundType.Sfx:
                    sfxInstances.Add(soundInstance);
                    break;
                case SoundType.Voice:
                    voiceInstances.Add(soundInstance);
                    break;
                case SoundType.Music:
                    musicInstances.Add(soundInstance);
                    break;
            }

            soundInstanceController.SetupInstance(instanceReference, instanceParameters);

            soundInstance.transform.position = position;
            soundInstance.transform.rotation = rotation;

            return soundInstanceController;
        }

        #endregion Create Instance

        public void DestroySoundInstance(SoundInstanceController soundInstance)
        {
            switch (soundInstance.currentSoundType)
            {
                default:
                case SoundType.Sfx:
                    sfxInstances.Remove(soundInstance?.gameObject);
                    break;
                case SoundType.Voice:
                    voiceInstances.Remove(soundInstance?.gameObject);
                    break;
                case SoundType.Music:
                    musicInstances.Remove(soundInstance?.gameObject);
                    break;
            }

            _soundInstancesPool.Release(soundInstance.gameObject);
        }
    }
}