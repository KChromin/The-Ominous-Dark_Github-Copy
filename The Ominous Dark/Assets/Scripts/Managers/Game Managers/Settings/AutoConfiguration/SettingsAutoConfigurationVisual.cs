using UnityEngine;

namespace NOS.GameManagers.Settings
{
    public class SettingsAutoConfigurationVisual : ISettingsAutoConfiguration
    {
        public SettingsAutoConfigurationVisual(SettingsContainers currentSettings)
        {
            _current = currentSettings;
        }

        private readonly SettingsContainers _current;

        public void SetAutoConfigurationSettings()
        {
        }
    }
}