using System.Collections.Generic;
using NOS.GameManagers.Audio;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    public class PlayerControllerFootsteps : ControllerBase
    {
        public PlayerControllerFootsteps(PlayerReferences references, PlayerConditions conditions, PlayerValues values, PlayerActions actions, SoundManager soundManager)
        {
            Transform footStepsOrigin = references.Objects.footSteps.transform;
            _parameters = references.ScriptableObjects.Default.footsteps;

            _conditions = conditions;
            _values = values;
            _actions = actions;

            _soundInstance = soundManager.CreateSoundInstance(_parameters.soundReference, references.SoundInstanceParameters.closeToPlayerSoundsWithReflections, footStepsOrigin);

            SubscribeToEvents();
        }

        private readonly PlayerControllerFootstepsScriptableObject _parameters;
        private readonly PlayerConditions _conditions;
        private readonly PlayerValues _values;
        private readonly PlayerActions _actions;
        private readonly SoundInstanceController _soundInstance;


        private bool _leftStep;

        private FootstepsTypes _currentFootstepsType;
        private const string PanSidingParameterName = "PanSiding";
        private const string FootstepsTypeParameterName = "FootstepsType";

        private void OnLanding()
        {
            if (_values.General.rigidBodyCurrentVelocityY <= _parameters.landingSoundMinimumForce)
            {
                _soundInstance.Play();
                _soundInstance.SetParameter(PanSidingParameterName, 2);
                _soundInstance.SetParameter(FootstepsTypeParameterName, (int)FootstepsTypes.LandBasic);
            }
        }

        private void OnHeadBob()
        {
            if (!_conditions.Default.cases.isMoving || !_conditions.Default.cases.isMovingAboveMinimalThreshold) return;

            _leftStep = !_leftStep;
            int side = _leftStep ? 0 : 2;
            _soundInstance.Play();
            _soundInstance.SetParameter(PanSidingParameterName, side);
            _soundInstance.SetParameter(FootstepsTypeParameterName, GetCurrentFootStepType());
        }

        private int GetCurrentFootStepType()
        {
            if (_conditions.Default.cases.isCrouching && _conditions.Default.cases.isMovingAboveMinimalThreshold)
            {
                return (int)FootstepsTypes.CrouchWalkBasic;
            }

            if (_conditions.Default.cases.isRunning && _conditions.Default.cases.isRunningAboveMinimalThreshold)
            {
                return (int)FootstepsTypes.RunBasic;
            }

            return (int)FootstepsTypes.WalkBasic;
        }

        #region Subscriptions and OnDestroy

        private void SubscribeToEvents()
        {
            _actions.Default.OnHeadBobHalfCycle += OnHeadBob;
            _actions.Default.OnGroundedState += OnLanding;
        }

        private void UnsubscribeFromEvents()
        {
            _actions.Default.OnHeadBobHalfCycle -= OnHeadBob;
            _actions.Default.OnGroundedState -= OnLanding;
        }

        public override void OnDestroy()
        {
            _soundInstance.OnParentDestroy();
            UnsubscribeFromEvents();
        }

        #endregion Subscriptions and OnDestroy
    }
}