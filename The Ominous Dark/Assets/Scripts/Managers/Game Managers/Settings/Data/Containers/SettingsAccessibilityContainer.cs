using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsAccessibilityContainer : ICloneable
    {
        [Header("Cursor")]
        public bool cursorLockToWindow = false;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}