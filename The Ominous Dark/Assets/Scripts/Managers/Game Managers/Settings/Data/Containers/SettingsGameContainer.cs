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

        [Header("Head Bobbing")]
        public bool headBobbingDisable = false;
        [Range(0, 1f)]
        public float headBobbingIntensity = 1f;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}