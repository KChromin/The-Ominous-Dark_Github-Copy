using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsGameContainer : ICloneable
    {
        [Header("Language")]
        [SerializeField]
        [Range(0, 1)]
        private short language;

        public short Language
        {
            get => language;
            set => language = SettingsValues.GetShortValue(value, new Vector2Int(0, 1), 0);
        }


        [Header("Field of view")]
        [SerializeField]
        [Range(60f, 90f)]
        private float fieldOfView = 75;

        public float FieldOfView
        {
            get => fieldOfView;
            set => fieldOfView = SettingsValues.GetFloatValue(value, new Vector2(60, 90));
        }


        [Header("Head Bobbing")]
        [SerializeField]
        [Range(0f, 1f)]
        private float headBobbingIntensity = 1;

        public float HeadBobbingIntensity
        {
            get => headBobbingIntensity;
            set => headBobbingIntensity = SettingsValues.GetFloatValue(value, new Vector2(0, 1));
        }


        [field:Header("Item Scrolling loops on ends")]
        [field: SerializeField]
        public bool InventoryItemScrollLoopsOnEnds { get; set; } = true;


        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}