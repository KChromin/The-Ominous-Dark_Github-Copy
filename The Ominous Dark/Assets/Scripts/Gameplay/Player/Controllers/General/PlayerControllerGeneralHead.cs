using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.General
{
    public class PlayerControllerGeneralHead : ControllerBase
    {
        public PlayerControllerGeneralHead(PlayerReferences references)
        {
            _headBase = references.Objects.head.transform;
            _headTruck = references.Objects.headTruck.transform;
            _headHeadBobbing = references.Objects.headBobbing.transform;
            _headPivot = references.Objects.headPivot.transform;
        }

        private readonly Transform _headBase;
        private readonly Transform _headTruck;
        private readonly Transform _headHeadBobbing;
        private readonly Transform _headPivot;


        public void UpdateLocalPositionHeadBobbing(Vector3 newLocalPosition)
        {
            _headHeadBobbing.localPosition = newLocalPosition;
        }
    }
}