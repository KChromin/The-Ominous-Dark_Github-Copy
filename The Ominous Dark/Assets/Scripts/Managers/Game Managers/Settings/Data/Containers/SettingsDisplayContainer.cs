using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsDisplayContainer :ICloneable
    {
        [Header("Fullscreen")]
        [Range(0, 2)]
        public byte fullscreenMode = 0;

        [Header("Resolution")]
        [Range(0, 2)]
        public byte aspectRatio = 0;

        [Range(0, 6)]
        public byte resolutionPreset16X9 = 2;

        [Range(0, 4)]
        public byte resolutionPreset16X10 = 2;

        [Range(0, 5)]
        public byte resolutionPreset21X9 = 0;

        [Header("Custom Resolution")]
        public bool resolutionCustom = false;

        public short resolutionCustomHeight = 1080;
        public short resolutionCustomWidth = 1920;

        [Header("VSync")]
        [Range(0, 4)]
        public byte vSyncMode = 1;

        [Header("Custom Framerate")]
        public bool framerateCustomMax = false;
        [Range(1, 1000)]
        public short framerateCustomMaxValue = 144;

        [Header("Run In Background")]
        public bool runInBackground = false;
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}