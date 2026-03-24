using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsAudioContainer : ICloneable
    {
        [Header("Audio Volume")]
        [SerializeField]
        [Range(0, 100)]
        private short volumeMaster = 100;

        public short VolumeMaster
        {
            get => volumeMaster;
            set => volumeMaster = SettingsValues.GetShortValue(value, new Vector2Int(0, 100));
        }

        [SerializeField]
        [Range(0, 100)]
        private short volumeSfx = 80;

        public short VolumeSfx
        {
            get => volumeSfx;
            set => volumeSfx = SettingsValues.GetShortValue(value, new Vector2Int(0, 100));
        }

        [SerializeField]
        [Range(0, 100)]
        private short volumeMusic = 80;

        public short VolumeMusic
        {
            get => volumeMusic;
            set => volumeMusic = SettingsValues.GetShortValue(value, new Vector2Int(0, 100));
        }

        [SerializeField]
        [Range(0, 100)]
        private short volumeVoice = 80;

        public short VolumeVoice
        {
            get => volumeVoice;
            set => volumeVoice = SettingsValues.GetShortValue(value, new Vector2Int(0, 100));
        }

        [field: Header("Subtitles")]
        [field: SerializeField]
        public bool Subtitles { get; set; } = false;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}