using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsVisualContainer :ICloneable
    {
        [Header("Test")]
        public bool test;
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}