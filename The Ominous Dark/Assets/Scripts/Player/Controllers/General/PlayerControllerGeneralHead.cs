using NOS.Patterns.Controller;
using NOS.Player.Data;
using UnityEngine;

namespace NOS.Player.Controller.General
{
    public class PlayerControllerGeneralHead : ControllerBase
    {
        public PlayerControllerGeneralHead(PlayerReferences references)
        {
            _headBase = references.objects.head.transform;
            _headTruck = references.objects.headTruck.transform;
            _headHeadBobbing = references.objects.headBobbing.transform;
            _headPivot = references.objects.headPivot.transform;
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