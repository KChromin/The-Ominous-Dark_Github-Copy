using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsAudioContainer :ICloneable
    {
        [Header("Audio Volume")]
        [Range(0, 100)]
        public byte volumeMaster = 100;
        [Range(0, 100)]
        public byte volumeSfx = 80;
        [Range(0, 100)]
        public byte volumeMusic = 80;
        [Range(0, 100)]
        public byte volumeVoice = 80;

        [Header("Subtitles")]
        public bool subtitles = false;
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}