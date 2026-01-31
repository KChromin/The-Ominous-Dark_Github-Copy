using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsGameContainer : ICloneable
    {
        [Header("Language")]
        [Range(0, 1)]
        public short language = 0;

        [Header("Field of view")]
        [Range(65, 90)]
        public float fieldOfView = 75;
        
        [Header("Head Bobbing")]
        [Range(0, 1f)]
        public float headBobbingIntensity = 1f;

        [Header("Item Scrolling loops on ends")]
        public bool inventoryItemScrollLoopsOnEnds = true;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}