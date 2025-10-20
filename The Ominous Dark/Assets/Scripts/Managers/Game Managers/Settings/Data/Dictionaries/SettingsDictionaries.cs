using System.Collections.Generic;

namespace NOS.GameManagers.Settings
{
    public static class SettingsDictionaries
    {
        #region Display

        public static readonly Dictionary<int, ResolutionPreset> ResolutionPresets16X9 = new()
        {
            { 0, new ResolutionPreset(720, 1280) },
            { 1, new ResolutionPreset(900, 1600) },
            { 2, new ResolutionPreset(1080, 1920) },
            { 3, new ResolutionPreset(1152, 2048) },
            { 4, new ResolutionPreset(1440, 2560) },
            { 5, new ResolutionPreset(1800, 3200) },
            { 6, new ResolutionPreset(2160, 3840) }
        };

        public static readonly Dictionary<int, ResolutionPreset> ResolutionPresets16X10 = new()
        {
            { 0, new ResolutionPreset(900, 1440) },
            { 1, new ResolutionPreset(1050, 1680) },
            { 2, new ResolutionPreset(1200, 1920) },
            { 3, new ResolutionPreset(1600, 2560) },
            { 4, new ResolutionPreset(2400, 3840) },
        };

        public static readonly Dictionary<int, ResolutionPreset> ResolutionPresets21X9 = new()
        {
            { 0, new ResolutionPreset(1080, 2560) },
            { 1, new ResolutionPreset(1200, 2880) },
            { 2, new ResolutionPreset(1440, 3440) },
            { 3, new ResolutionPreset(1600, 3840) },
            { 4, new ResolutionPreset(1800, 4320) },
            { 5, new ResolutionPreset(2160, 5120) },
        };

        public struct ResolutionPreset
        {
            public ResolutionPreset(short height, short width)
            {
                Height = height;
                Width = width;
            }

            public readonly short Height;
            public readonly short Width;
        }

        #endregion Display
    }
}