using System.Collections.Generic;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    public class SettingsUpdaterDisplay : ISettingsUpdater
    {
        public SettingsUpdaterDisplay(SettingsManager settingsManager)
        {
            Settings = settingsManager;
        }

        public SettingsManager Settings { get; set; }

        public void UpdateSettings()
        {
            SetAspectAndResolution();
            SetFullscreen();
            SetFrameRateAndVSync();
            Application.runInBackground = Settings.CurrentSettings.display.RunInBackground;
        }

        //Fullscreen mode//
        private void SetFullscreen()
        {
            switch (Settings.CurrentSettings.display.FullscreenMode)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    Screen.fullScreen = true;
                    return;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                    Screen.fullScreen = true;
                    return;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Screen.fullScreen = false;
                    return;
            }
        }

        private void SetAspectAndResolution()
        {
            if (Settings.CurrentSettings.display.ResolutionCustom)
            {
                Screen.SetResolution(Settings.CurrentSettings.display.ResolutionCustomWidth, Settings.CurrentSettings.display.ResolutionCustomHeight, true);
            }
            else
            {
                switch (Settings.CurrentSettings.display.AspectRatio)
                {
                    //16x9
                    case 0:
                        Screen.SetResolution(SettingsDictionaries.ResolutionPresets16X9[Settings.CurrentSettings.display.ResolutionPreset16X9].Width, SettingsDictionaries.ResolutionPresets16X9[Settings.CurrentSettings.display.ResolutionPreset16X9].Height, true);
                        return;
                    //16x10
                    case 1:
                        Screen.SetResolution(SettingsDictionaries.ResolutionPresets16X10[Settings.CurrentSettings.display.ResolutionPreset16X10].Width, SettingsDictionaries.ResolutionPresets16X10[Settings.CurrentSettings.display.ResolutionPreset16X10].Height, true);
                        return;
                    //21x9
                    case 2:
                        Screen.SetResolution(SettingsDictionaries.ResolutionPresets21X9[Settings.CurrentSettings.display.ResolutionPreset21X9].Width, SettingsDictionaries.ResolutionPresets21X9[Settings.CurrentSettings.display.ResolutionPreset21X9].Height, true);
                        return;
                }
            }
        }

        private void SetFrameRateAndVSync()
        {
            if (Settings.CurrentSettings.display.FramerateCustomMax)
            {
                QualitySettings.vSyncCount = 0; //Disable VSync
                Application.targetFrameRate = Settings.CurrentSettings.display.FramerateCustomMaxValue;
            }
            else
            {
                QualitySettings.vSyncCount = Settings.CurrentSettings.display.VSyncMode;
                Application.targetFrameRate = -1;
            }
        }
    }
}