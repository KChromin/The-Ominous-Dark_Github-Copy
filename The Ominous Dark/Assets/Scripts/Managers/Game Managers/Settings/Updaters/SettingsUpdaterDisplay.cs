using System.Collections.Generic;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    public class SettingsUpdaterDisplay : ISettingsUpdater
    {
        public SettingsUpdaterDisplay(SettingsContainers currentSettings)
        {
            _current = currentSettings;
        }

        private readonly SettingsContainers _current;

        public void UpdateSettings()
        {
            SetAspectAndResolution();
            SetFullscreen();
            SetFrameRateAndVSync();
            Application.runInBackground = _current.display.runInBackground;
        }

        //Fullscreen mode//
        private void SetFullscreen()
        {
            switch (_current.display.fullscreenMode)
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
            if (_current.display.resolutionCustom)
            {
                Screen.SetResolution(_current.display.resolutionCustomWidth, _current.display.resolutionCustomHeight, true);
            }
            else
            {
                switch (_current.display.aspectRatio)
                {
                    //16x9
                    case 0:
                        Screen.SetResolution(SettingsDictionaries.ResolutionPresets16X9[_current.display.resolutionPreset16X9].Width, SettingsDictionaries.ResolutionPresets16X9[_current.display.resolutionPreset16X9].Height, true);
                        return;
                    //16x10
                    case 1:
                        Screen.SetResolution(SettingsDictionaries.ResolutionPresets16X10[_current.display.resolutionPreset16X10].Width, SettingsDictionaries.ResolutionPresets16X10[_current.display.resolutionPreset16X10].Height, true);
                        return;
                    //21x9
                    case 2:
                        Screen.SetResolution(SettingsDictionaries.ResolutionPresets21X9[_current.display.resolutionPreset21X9].Width, SettingsDictionaries.ResolutionPresets21X9[_current.display.resolutionPreset21X9].Height, true);
                        return;
                }
            }
        }

        private void SetFrameRateAndVSync()
        {
            if (_current.display.framerateCustomMax)
            {
                QualitySettings.vSyncCount = 0; //Disable VSync
                Application.targetFrameRate = _current.display.framerateCustomMaxValue;
            }
            else
            {
                QualitySettings.vSyncCount = _current.display.vSyncMode;
                Application.targetFrameRate = -1;
            }
        }
    }
}