using UnityEngine;

namespace NOS.Player.Controller.Default
{
    [CreateAssetMenu(fileName = "PlayerControllerLookParameters", menuName = "ScriptableObjects/Player/Default/Look")]
    public class PlayerControllerLookScriptableObject : ScriptableObject
    {
        [Header("Max Angles")]
        [Range(-90, 90)]
        public int maxAngleUp = 80;
        [Range(-90, 90)]
        public int maxAngleDown = -80;
        
    }
}