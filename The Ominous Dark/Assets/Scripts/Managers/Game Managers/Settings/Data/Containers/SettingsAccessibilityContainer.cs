using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsAccessibilityContainer : ICloneable
    {
        [field: Header("Cursor")]
        [field: SerializeField]
        public bool CursorLockToWindow { get; set; } = true;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}