using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using SteamAudio;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Vector3 = UnityEngine.Vector3;

namespace NOS.GameManagers.Audio
{
    [SelectionBase]
    [RequireComponent(typeof(SteamAudioSource))]
    public class SteamAudioReflectionsDistanceLimiter : MonoBehaviour
    {
        #region Parameters

        [Header("Max Distance")]
        [SerializeField] //Base event distance + reflections distance = max reflections range
        private float reflectionDistanceAfterEventMaxDistance = 10f;

        [Header("Fall Off Curve")]
        [SerializeField]
        private AnimationCurve reflectionsFallOffCurve = new()
        {
            keys = new Keyframe[]
            {
                new(0, 1, 0, -1.75f),
                new(1, 0, 0, 0)
            },
            preWrapMode = WrapMode.Clamp,
            postWrapMode = WrapMode.Clamp
        };

        [Header("Time Between Updates")]
        [SerializeField]
        private float reflectionsMixUpdateInterval = 0.1f;

        #endregion Parameters

        private Transform _playerObject;
        private Transform _myTransform;

        private StudioEventEmitter _emitter;

        private SphereCollider _distanceTrigger;

        private float _baseReflectionsMixLevel;
        private float _reflectionsMaxDistance;
        private float _eventMaxDistance;

        private bool _playerIsInRange; //If player is in trigger//
        private bool _playerOutsideRangeResetWasDone;

        private bool _wasSetToBaseLevel;
        private bool _wasSetToZero;

        //Value under which reflection mix is rounded to zero//
        private const float RoundToZeroValue = 0.02f;
        private const string BaseReflectionMixLevelUserPropertyName = "BaseReflectionMixLevel";
        private const string BaseReflectionMixLevelParameterName = "SteamReflectionsMixLevel";

        private readonly CancellationTokenSource _cancellation = new();

        private void Awake()
        {
            SetupStatics();
        }

        public void UpdateParameters(float newReflectionMaxAdditionalDistance = -10, AnimationCurve newFallOffCurve = null, float newReflectionUpdateInterval = -10)
        {
            if (newReflectionMaxAdditionalDistance > 0)
            {
                reflectionDistanceAfterEventMaxDistance = newReflectionMaxAdditionalDistance;
            }

            if (newFallOffCurve != null)
            {
                reflectionsFallOffCurve.CopyFrom(newFallOffCurve);
            }

            if (newReflectionUpdateInterval > 0)
            {
                reflectionsMixUpdateInterval = newReflectionUpdateInterval;
            }

            SetupParameters();
        }

        public void Disable()
        {
            _cancellation.Cancel();
        }

        #region Setups

        private void SetupStatics()
        {
            _emitter = GetComponent<StudioEventEmitter>();
            //Todo, optimize this from manager!!!!
            _playerObject = GameObject.FindGameObjectWithTag("Player").transform;
            _myTransform = transform;

            _distanceTrigger = GetComponent<SphereCollider>();
        }

        private void SetupParameters()
        {
            #region Base Reflection Mix

            _emitter.EventInstance.getDescription(out EventDescription eventDescription);
            if (eventDescription.getUserProperty(BaseReflectionMixLevelUserPropertyName, out USER_PROPERTY userProperty) == RESULT.OK)
            {
                _baseReflectionsMixLevel = userProperty.floatValue();
            }
            else //If isn't specifically set, give default value
            {
                _emitter.EventInstance.getParameterByName(BaseReflectionMixLevelParameterName, out _baseReflectionsMixLevel);
            }

            _emitter.EventInstance.setParameterByName(BaseReflectionMixLevelParameterName, _baseReflectionsMixLevel);

            #endregion Base Reflection Mix

            eventDescription.getMinMaxDistance(out _, out _eventMaxDistance);

            _reflectionsMaxDistance = _eventMaxDistance + reflectionDistanceAfterEventMaxDistance;

            _distanceTrigger.radius = _reflectionsMaxDistance;

            _cancellation.Cancel();
            UpdateReflectionMixEveryInterval(_cancellation.Token).Forget();
        }

        #endregion Setup

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerIsInRange = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerIsInRange = false;
            }
        }

        private async UniTaskVoid UpdateReflectionMixEveryInterval(CancellationToken cancellationToken)
        {
            UpdateReflectionMixValue();
            await UniTask.WaitForSeconds(reflectionsMixUpdateInterval, cancellationToken: cancellationToken);
            UpdateReflectionMixEveryInterval(cancellationToken).Forget();
        }

        // ReSharper disable once CognitiveComplexity
        private void UpdateReflectionMixValue()
        {
            if (_playerIsInRange)
            {
                _playerOutsideRangeResetWasDone = false;

                float playerDistanceToAudioSource = Vector3.Distance(_playerObject.position, _myTransform.position);

                if (playerDistanceToAudioSource > _reflectionsMaxDistance)
                {
                    _wasSetToBaseLevel = false;

                    if (_wasSetToZero) return;

                    _emitter.EventInstance.setParameterByName(BaseReflectionMixLevelParameterName, 0);
                    _wasSetToZero = true;
                }
                else if (playerDistanceToAudioSource < _eventMaxDistance)
                {
                    _wasSetToZero = false;

                    if (_wasSetToBaseLevel) return;
                    _emitter.EventInstance.setParameterByName(BaseReflectionMixLevelParameterName, _baseReflectionsMixLevel);
                    _wasSetToBaseLevel = true;
                }
                else
                {
                    _wasSetToZero = false;
                    _wasSetToBaseLevel = false;

                    float distanceRatio = Mathf.InverseLerp(_eventMaxDistance, _reflectionsMaxDistance, playerDistanceToAudioSource);
                    float mixValue = reflectionsFallOffCurve.Evaluate(distanceRatio) * _baseReflectionsMixLevel;

                    //Round to zero
                    if (mixValue <= RoundToZeroValue)
                    {
                        mixValue = 0;
                    }

                    _emitter.EventInstance.setParameterByName(BaseReflectionMixLevelParameterName, mixValue);
                }
            }
            else if (!_playerOutsideRangeResetWasDone)
            {
                _playerOutsideRangeResetWasDone = true;

                _wasSetToZero = true;
                _emitter.EventInstance.setParameterByName(BaseReflectionMixLevelParameterName, 0);
            }
        }

        private void OnDestroy()
        {
            _cancellation.Cancel();
        }

        private void OnDisable()
        {
            _wasDisabled = true;
            _cancellation.Cancel();
        }

        private bool _wasDisabled;

        public void UpdateEnabledness(bool newState)
        {
            enabled = newState;
            _distanceTrigger.enabled = newState;

            if (newState && _wasDisabled)
            {
                _wasDisabled = false;
                UpdateReflectionMixEveryInterval(_cancellation.Token).Forget();
            }
        }
    }
}