using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.General
{
    public class PlayerControllerGeneralValuesUpdater : ControllerBase
    {
        public PlayerControllerGeneralValuesUpdater(PlayerValues values, PlayerReferences references)
        {
            _values = values.General;
            _references = references;

            //Cache Transforms//
            _playerBase = _references.objects.playerBase.transform;
            _orientation = references.objects.orientation.transform;

            _values.rigidBodyMass = (int)_references.components.rigidBody.mass;
        }

        private readonly PlayerValues.GeneralValuesClass _values;
        private readonly PlayerReferences _references;

        //Transforms
        private readonly Transform _playerBase;
        private readonly Transform _orientation;

        public override void Update()
        {
            //Player position//
            _values.playerPositionLastFrame = _playerBase.position;

            UpdateComponentsValues();
            UpdateObjectsValues();
        }

        private void UpdateComponentsValues()
        {
            //Rigidbody//
            _values.rigidBodyCurrentVelocity = _references.components.rigidBody.linearVelocity;
            _values.rigidBodyCurrentVelocityMagnitudeXZ = new Vector2(_values.rigidBodyCurrentVelocity.x, _values.rigidBodyCurrentVelocity.z).magnitude;
            _values.rigidBodyCurrentVelocityY = _values.rigidBodyCurrentVelocity.y;
        }

        private void UpdateObjectsValues()
        {
            //Orientation//
            _values.orientationCurrentPosition = _orientation.position;
            _values.orientationCurrentRotation = _orientation.rotation;
        }
    }
}