using UnityEngine;

namespace NOS.GameManagers.Settings
{
    public class SettingsAutoConfigurationDisplay : ISettingsAutoConfiguration
    {
        public SettingsAutoConfigurationDisplay(SettingsContainers currentSettings)
        {
            _current = currentSettings;
        }

        private readonly SettingsContainers _current;

        public void SetAutoConfigurationSettings()
        {
            AutoConfigResolution();
        }

        // ReSharper disable once CognitiveComplexity
        private void AutoConfigResolution()
        {
            Resolution current = Screen.currentResolution;

            float height = current.height;
            float width = current.width;
            float aspectRatio = width / height;

            if (Mathf.Approximately(aspectRatio, 1.77777777778f)) //16x9
            {
                _current.display.aspectRatio = 0;

                //Find resolution in presets
                for (int i = 0; i < SettingsDictionaries.ResolutionPresets16X9.Count; i++)
                {
                    if (Mathf.Approximately(height, SettingsDictionaries.ResolutionPresets16X9[i].Height))
                    {
                        _current.display.resolutionPreset16X9 = (byte)i;
                        return;
                    }
                }

                //When not in dictionary, set custom
                SetCustomResolution((short)height, (short)width);
            }
            else if (Mathf.Approximately(aspectRatio, 1.6f)) //16x10
            {
                _current.display.aspectRatio = 1;

                //Find resolution in presets
                for (int i = 0; i < SettingsDictionaries.ResolutionPresets16X10.Count; i++)
                {
                    if (Mathf.Approximately(height, SettingsDictionaries.ResolutionPresets16X10[i].Height))
                    {
                        _current.display.resolutionPreset16X10 = (byte)i;
                        return;
                    }
                }

                //When not in dictionary, set custom
                SetCustomResolution((short)height, (short)width);
            }
            else if (Mathf.Approximately(aspectRatio, 2.33333333333f)) //21x10
            {
                _current.display.aspectRatio = 2;

                //Find resolution in presets
                for (int i = 0; i < SettingsDictionaries.ResolutionPresets21X9.Count; i++)
                {
                    if (Mathf.Approximately(height, SettingsDictionaries.ResolutionPresets21X9[i].Height))
                    {
                        _current.display.resolutionPreset21X9 = (byte)i;
                        return;
                    }
                }

                //When not in dictionary, set custom
                SetCustomResolution((short)height, (short)width);
            }
            else //When not in preset aspect ratios, then custom
            {
                SetCustomResolution((short)height, (short)width);
            }
        }

        private void SetCustomResolution(short height, short width)
        {
            _current.display.resolutionCustom = true;
            _current.display.resolutionCustomHeight = height;
            _current.display.resolutionCustomWidth = width;
        }
    }
}