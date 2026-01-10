using NOS.GameManagers.Input;
using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.Default
{
    public class PlayerControllerCheckersDefault : ControllerBase
    {
        public PlayerControllerCheckersDefault(InputDataContainer input, PlayerConditions conditions, PlayerValues values, PlayerReferences references)
        {
            _input = input;
            _conditions = conditions.Default;
            _valuesGeneral = values.General;
            _valuesDefault = values.Default;
            _parameters = references.ScriptableObjects.Default.checkers;
        }

        private readonly InputDataContainer _input;
        private readonly PlayerConditions.DefaultConditionsClass _conditions;
        private readonly PlayerValues.GeneralValuesClass _valuesGeneral;
        private readonly PlayerValues.DefaultValuesClass _valuesDefault;
        private readonly PlayerControllerCheckersDefaultScriptableObject _parameters;

        public override void Update()
        {
            GroundCheck();
            SlopeCheck();
            CellingCheck();
        }

        #region Ground Check

        private void GroundCheck()
        {
            _conditions.cases.isGrounded = Physics.SphereCast(_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForGroundCheck, _parameters.groundCheckRadius, Vector3.down, out _valuesDefault.GroundCheckHit, _parameters.groundCheckDistance, _parameters.groundLayer);
        }

        #endregion Ground Check

        #region Slope Check

        //todo cast sphere in input direction to check if there is a slope. If there is, then do not go

        private void SlopeCheck()
        {
            if (!_conditions.cases.isGrounded)
            {
                _conditions.cases.isOnTooSteepSlope = false;
                _conditions.cases.wantsToMoveOnTooSteepSlope = false;

                #region Debug

#if UNITY_EDITOR
                _debugSlopeCheckHit = false;
#endif

                #endregion Debug

                return;
            }

            Vector3 groundHitOrigin = _valuesDefault.GroundCheckHit.point;

            if (Physics.Raycast(groundHitOrigin + _parameters.slopeCheckRaycastOffset, Vector3.down, out RaycastHit hit, _parameters.slopeCheckRaycastOffset.y + 0.01f, _parameters.groundLayer, QueryTriggerInteraction.Ignore))
            {
                //When player is on slope don't check//
                Vector3 slopeCheckOrigin = _valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForGroundCheck;

                if (Physics.SphereCast(slopeCheckOrigin, _parameters.slopeCheckRadius, Vector3.down, out RaycastHit slopeCheckHit, _parameters.slopeCheckDistance, _parameters.groundLayer))
                {
                    _valuesDefault.slopeCheckNormal = hit.normal;
                    _valuesDefault.slopeCheckAngle = Vector3.Angle(slopeCheckHit.normal, Vector3.up);
                    //  _conditions.cases.isOnTooSteepSlope = Vector3.Angle(hit.normal, Vector3.up) > _parameters.slopeCheckMaxSlope && Vector3.Angle(slopeCheckHit.normal, Vector3.up) > _parameters.slopeCheckMaxSlope;
                    _conditions.cases.isOnTooSteepSlope = Vector3.Angle(slopeCheckHit.normal, Vector3.up) > _parameters.slopeCheckMaxSlope;

                    #region Debug

#if UNITY_EDITOR
                    _debugSlopeCheckHit = true;
                    _debugSlopeCheckHitRaycast = slopeCheckHit;
#endif

                    #endregion Debug
                }
                else //When somehow sphere cannot hit, check groundCheck's normal
                {
                    _valuesDefault.slopeCheckAngle = Vector3.Angle(_valuesDefault.GroundCheckHit.normal, Vector3.up);
                    _conditions.cases.isOnTooSteepSlope = _valuesDefault.slopeCheckAngle > _parameters.slopeCheckMaxSlope;

                    #region Debug

#if UNITY_EDITOR
                    _debugSlopeCheckHit = false;
#endif

                    #endregion Debug
                }
            }
            else //If raycast cannot hit, check groundCheck's normal
            {
                _valuesDefault.slopeCheckAngle = Vector3.Angle(_valuesDefault.GroundCheckHit.normal, Vector3.up);
                _conditions.cases.isOnTooSteepSlope = _valuesDefault.slopeCheckAngle > _parameters.slopeCheckMaxSlope;

                #region Debug

#if UNITY_EDITOR
                _debugSlopeCheckHit = false;
#endif

                #endregion Debug
            }

            if (_conditions.cases.isOnTooSteepSlope)
            {
                _conditions.cases.wantsToMoveOnTooSteepSlope = false;
            }
            else
            {
                Vector3 slopeCheckOrigin = (_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForGroundCheck) + Vector3.down * _valuesDefault.GroundCheckHit.distance + Vector3.down * _parameters.slopeCheckDirectionVerticalOffset;
                Vector3 inputDirection = (_valuesGeneral.orientationCurrentRotation * _input.inputMove3D).normalized;

                if (Physics.SphereCast(slopeCheckOrigin, _parameters.slopeCheckDirectionRadius, inputDirection, out RaycastHit slopeCheckDirectionHit, _parameters.slopeCheckDirectionDistance, _parameters.groundLayer))
                {
                    if (Physics.Raycast(slopeCheckDirectionHit.point + _parameters.slopeCheckRaycastOffset, Vector3.down, out RaycastHit slopeCheckDirectionRaycastHit, _parameters.slopeCheckRaycastOffset.y + 0.01f, _parameters.groundLayer, QueryTriggerInteraction.Ignore))
                    {
                        _valuesDefault.slopeCheckDirectionNormal = slopeCheckDirectionRaycastHit.normal;
                        _conditions.cases.wantsToMoveOnTooSteepSlope = Vector3.Angle(slopeCheckDirectionRaycastHit.normal, Vector3.up) > _parameters.slopeCheckMaxSlope;
                    }
                    else
                    {
                        _conditions.cases.wantsToMoveOnTooSteepSlope = false;
                    }
                }
                else
                {
                    _conditions.cases.wantsToMoveOnTooSteepSlope = false;
                }
            }
        }

        #endregion Slope Check

        #region Celling Check

        private void CellingCheck()
        {
            if (!_conditions.cases.isCrouching) return; //When not crouching, there is no need to check that
            _conditions.cases.isAbleToStandUp = !Physics.SphereCast(_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForCellingCheck, _parameters.cellingCheckRadius, Vector3.up, out _valuesDefault.CellingCheckHit, _parameters.cellingCheckDistance, _parameters.groundLayer,
                QueryTriggerInteraction.Ignore);
        }

        #endregion Celling Check

        #region Debug

#if UNITY_EDITOR

        private bool _debugSlopeCheckHit;
        private RaycastHit _debugSlopeCheckHitRaycast;

        public override void OnDrawGizmos()
        {
            DebugDrawGroundCheck();
            DebugDrawSlopeCheck();
            DebugDrawCellingCheck();
        }

        private void DebugDrawGroundCheck()
        {
            if (!_parameters.debugGroundCheckDraw) return;

            float checkDistance = _parameters.groundCheckDistance;
            Gizmos.color = _parameters.debugGroundCheckColor;

            if (_conditions.cases.isGrounded)
            {
                checkDistance = _valuesDefault.GroundCheckHit.distance;
                Gizmos.color = _parameters.debugGroundCheckColorHit;
                Gizmos.DrawLine((_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForGroundCheck), _valuesDefault.GroundCheckHit.point);
            }

            Gizmos.DrawWireSphere((_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForGroundCheck) + Vector3.down * checkDistance, _parameters.groundCheckRadius);
        }

        private void DebugDrawSlopeCheck()
        {
            if (!_parameters.debugSlopeCheckDraw) return;

            float checkDistance = _parameters.slopeCheckDistance;
            Gizmos.color = _parameters.debugSlopeCheckColor;

            if (_debugSlopeCheckHit)
            {
                checkDistance = _debugSlopeCheckHitRaycast.distance;
                Gizmos.color = _parameters.debugSlopeCheckColorHit;
                Gizmos.DrawLine((_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForGroundCheck), _debugSlopeCheckHitRaycast.point);
            }

            Gizmos.DrawWireSphere((_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForGroundCheck) + Vector3.down * checkDistance, _parameters.slopeCheckRadius);

            Gizmos.color = Color.deepPink;
            Vector3 inputDirection = (_valuesGeneral.orientationCurrentRotation * _input.inputMove3D).normalized;
            Gizmos.DrawWireSphere(((_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForGroundCheck) + Vector3.down * _valuesDefault.GroundCheckHit.distance) + inputDirection * _parameters.slopeCheckDirectionDistance + Vector3.down * _parameters.slopeCheckDirectionVerticalOffset,
                _parameters.slopeCheckDirectionRadius);
        }

        private void DebugDrawCellingCheck()
        {
            if (!_parameters.debugCellingCheckDraw) return;

            float checkDistance = _parameters.cellingCheckDistance;
            Gizmos.color = _parameters.debugCellingCheckColor;

            if (!_conditions.cases.isAbleToStandUp)
            {
                checkDistance = _valuesDefault.CellingCheckHit.distance;
                Gizmos.color = _parameters.debugCellingCheckColorHit;
                Gizmos.DrawLine((_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForCellingCheck), _valuesDefault.CellingCheckHit.point);
            }

            Gizmos.DrawWireSphere((_valuesGeneral.orientationCurrentPosition + _parameters.checkPositionOffsetFromOriginForCellingCheck) + Vector3.up * checkDistance, _parameters.cellingCheckRadius);
        }
#endif

        #endregion Debug
    }
}