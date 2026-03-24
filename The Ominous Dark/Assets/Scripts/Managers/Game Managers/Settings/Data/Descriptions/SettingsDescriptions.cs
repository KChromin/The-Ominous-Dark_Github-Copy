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

            "0 - 1280 x 720, 1 - 1600 x 900, 2 - 1920 x 1080, 3 - 2048 x 1152, 4 - 2560 x 1440, 5 - 3200 x 1800, 6 - 3840 x 2160",
            "0 - 1440 x 900, 1 - 1680 x 1050, 2 - 1920 x 1200, 3 - 2560 x 1600, 4 - 3840 x 2400",
            "0 - 2560 x 1080, 1 - 2880 x 1200, 2 - 3440 x 1440, 3 - 3840 x 1600, 4 - 4320 x 1800, 5 - 5120 x 2160",

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

            "60 - 90 (75)", //fov

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