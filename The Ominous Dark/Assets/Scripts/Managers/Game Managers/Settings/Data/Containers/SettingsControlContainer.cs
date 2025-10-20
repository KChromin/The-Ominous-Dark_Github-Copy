using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsControlContainer : ICloneable
    {
        [Header("Look")]
        [Range(0, 100f)]
        public float lookSensitivityGeneral = 20f;

        [Space]
        public bool lookSeparateSensitivityAxes = false;

        [Range(0, 100f)]
        public float lookSensitivityXAxis = 20f;

        [Range(0, 100f)]
        public float lookSensitivityYAxis = 20f;

        [Space]
        public bool lookInvertYAxis = false;

        [Space]
        public bool lookSmoothing = true;
        [Range(0, 0.1f)]
        public float lookSmoothingTime = 0.002f;

        [Header("Input")]
        public bool inputCrouchToggle = true;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}