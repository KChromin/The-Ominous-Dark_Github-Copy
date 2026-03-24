using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsControlContainer : ICloneable
    {
        [Header("Look")]
        [SerializeField]
        [Range(0, 100f)]
        private float lookSensitivityGeneral = 20f;

        public float LookSensitivityGeneral
        {
            get => lookSensitivityGeneral;
            set => lookSensitivityGeneral = SettingsValues.GetFloatValue(value, new Vector2(0, 100));
        }

        [field: Space]
        [field: SerializeField]
        public bool LookSeparateSensitivityAxes { get; set; } = false;

        [SerializeField]
        [Range(0, 100f)]
        private float lookSensitivityXAxis = 20f;

        public float LookSensitivityXAxis
        {
            get => lookSensitivityXAxis;
            set => lookSensitivityXAxis = SettingsValues.GetFloatValue(value, new Vector2(0, 100));
        }

        [SerializeField]
        [Range(0, 100f)]
        private float lookSensitivityYAxis = 20f;

        public float LookSensitivityYAxis
        {
            get => lookSensitivityYAxis;
            set => lookSensitivityYAxis = SettingsValues.GetFloatValue(value, new Vector2(0, 100));
        }

        [field: Space]
        [field: SerializeField]
        public bool LookInvertYAxis { get; set; } = false;

        [field: Space]
        [field: SerializeField]
        public bool LookSmoothing { get; set; } = true;

        [SerializeField]
        [Range(0, 0.1f)]
        private float lookSmoothingTime = 0.002f;

        public float LookSmoothingTime
        {
            get => lookSmoothingTime;
            set => lookSmoothingTime = SettingsValues.GetFloatValue(value, new Vector2(0, 0.1f));
        }

        [field: Header("Input")]
        [field: SerializeField]
        public bool InputCrouchToggle { get; set; } = true;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}