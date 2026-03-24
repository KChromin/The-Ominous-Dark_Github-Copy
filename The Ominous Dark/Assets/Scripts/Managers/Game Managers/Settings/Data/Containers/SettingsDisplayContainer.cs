using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsDisplayContainer : ICloneable
    {
        [Header("Fullscreen")]
        [SerializeField]
        [Range(0, 2)]
        private short fullscreenMode = 0;

        public short FullscreenMode
        {
            get => fullscreenMode;
            set => fullscreenMode = SettingsValues.GetShortValue(value, new Vector2Int(0, 2), 0);
        }

        [Header("Resolution")]
        [SerializeField]
        [Range(0, 2)]
        private short aspectRatio = 0;

        public short AspectRatio
        {
            get => aspectRatio;
            set => aspectRatio = SettingsValues.GetShortValue(value, new Vector2Int(0, 2), 0);
        }

        [SerializeField]
        [Range(0, 6)]
        private short resolutionPreset16X9 = 2;

        public short ResolutionPreset16X9
        {
            get => resolutionPreset16X9;
            set => resolutionPreset16X9 = SettingsValues.GetShortValue(value, new Vector2Int(0, 6), 2);
        }

        [SerializeField]
        [Range(0, 4)]
        private short resolutionPreset16X10 = 2;

        public short ResolutionPreset16X10
        {
            get => resolutionPreset16X10;
            set => resolutionPreset16X10 = SettingsValues.GetShortValue(value, new Vector2Int(0, 4), 2);
        }

        [SerializeField]
        [Range(0, 5)]
        private short resolutionPreset21X9 = 0;

        public short ResolutionPreset21X9
        {
            get => resolutionPreset21X9;
            set => resolutionPreset21X9 = SettingsValues.GetShortValue(value, new Vector2Int(0, 5), 0);
        }

        [field: Header("Custom Resolution")]
        [field: SerializeField]
        public bool ResolutionCustom { get; set; } = false;

        [SerializeField]
        private short resolutionCustomHeight = 1080;

        public short ResolutionCustomHeight
        {
            get => resolutionCustomHeight;
            set => resolutionCustomHeight = SettingsValues.GetShortValue(value, new Vector2Int(0, short.MaxValue));
        }

        [SerializeField]
        private short resolutionCustomWidth = 1920;

        public short ResolutionCustomWidth
        {
            get => resolutionCustomWidth;
            set => resolutionCustomWidth = SettingsValues.GetShortValue(value, new Vector2Int(0, short.MaxValue));
        }

        [Header("VSync")]
        [SerializeField]
        [Range(0, 4)]
        private short vSyncMode = 1;

        public short VSyncMode
        {
            get => vSyncMode;
            set => vSyncMode = SettingsValues.GetShortValue(value, new Vector2Int(0, 4), 0);
        }

        [field: Header("Custom Framerate")]
        [field: SerializeField]
        public bool FramerateCustomMax { get; set; } = false;

        [SerializeField]
        [Range(1, 1000)]
        private short framerateCustomMaxValue = 144;

        public short FramerateCustomMaxValue
        {
            get => framerateCustomMaxValue;
            set => framerateCustomMaxValue = SettingsValues.GetShortValue(value, new Vector2Int(1, short.MaxValue));
        }

        [field: Header("Run In Background")]
        [field: SerializeField]
        public bool RunInBackground { get; set; } = false;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}