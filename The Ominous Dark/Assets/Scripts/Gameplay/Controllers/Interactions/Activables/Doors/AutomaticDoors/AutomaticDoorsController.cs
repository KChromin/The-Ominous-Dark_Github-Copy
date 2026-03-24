using UnityEngine;

namespace NOS.Controllers.Interactions
{
    public class AutomaticDoorsController : ActivableProximityBase
    {
        [Header("Open Door Offset")]
        [SerializeField]
        private Vector3 openDoorOffsetFromCenter = new(1.2f, 0, 0);

        [Header("Door Objects")]
        [SerializeField]
        private Transform leftDoor;
        [SerializeField]
        private Transform rightDoor;

        public override void OnProgressUpdate()
        {
            Vector3 newOffset = Vector3.Lerp(Vector3.zero, openDoorOffsetFromCenter, CurrentProgress01);
            leftDoor.localPosition = newOffset;
            rightDoor.localPosition = -newOffset;
        }
    }
}