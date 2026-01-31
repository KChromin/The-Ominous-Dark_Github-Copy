using UnityEngine;

namespace NOS.GameManagers.Settings
{
    public static class SettingsDescriptions
    {
        #region Headers

        public static readonly string[] SettingsHeaders =
        {
            "[Audio]",
            "[Control]",
            "[Display]",
            "[Game]",
            "[Visual]",
            "[Accessibility]"
        };

        #endregion Headers

        #region Audio

        public static readonly string[] AudioSettingsDescriptions =
        {
            "0 - 100%",
            "0 - 100%",
            "0 - 100%",
            "0 - 100%",

            "true, false"
        };

        #endregion Audio

        #region Control

        [Space]
        public static readonly string[] ControlSettingsDescriptions =
        {
            "0 - 100",

            "true, false",
            "0 - 100",
            "0 - 100",

            "true, false",

            "true, false",
            "0 - 0.1",

            "true, false"
        };

        #endregion Control

        #region Display

        [Space]
        public static readonly string[] DisplaySettingsDescriptions =
        {
            "0 - Exclusive Fullscreen, 1 - Maximized Window, 2 - Windowed",

            "0 - 16:9, 1 - 16:10, 2 - 21:9",

            "0 - 720 x 1280, 1 - 900 x 1600, 2 - 1080 x 1920, 3 - 1152 x 2048, 4 - 1440 x 2560, 5 - 1800 x 3200, 6 - 2160 x 3840",
            "0 - 900 x 1440, 1 - 1050 x 1680, 2 - 1200 x 1920, 3 - 1600 x 2560, 4 - 2400 x 3840",
            "0 - 1080 x 2560, 1 - 1200 x 2880, 2 - 1440 x 3440, 3 - 1600 x 3840, 4 - 1800 x 4320, 5 - 2160 x 5120",

            "true, false",
            "",
            "",

            "0 - Off, 1 - On (Full), 2 - On (Half), 3 - On (Quarter), 4 - On (1/8)",

            "true, false",
            "",

            "true, false"
        };

        #endregion Display

        #region Game

        [Space]
        public static readonly string[] GameSettingsDescriptions =
        {
            "0 - English, 1 - Polish",
            
            "65 - 90 (75)", //fov
            
            "0 - 1",
            
            "True, False"
        };

        #endregion Game

        #region Visual

        [Space]
        public static readonly string[] VisualSettingsDescriptions =
        {
            "eee",
            "ooo"
        };

        #endregion Visual

        #region Accessibility

        [Space]
        public static readonly string[] AccessibilitySettingsDescriptions =
        {
            "true, false",
        };

        #endregion Accessibility
    }
}