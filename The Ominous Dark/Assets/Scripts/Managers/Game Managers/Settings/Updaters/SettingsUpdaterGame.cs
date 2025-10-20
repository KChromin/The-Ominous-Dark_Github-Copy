using UnityEngine;
using UnityEngine.Localization.Settings;

namespace NOS.GameManagers.Settings
{
    public class SettingsUpdaterGame : ISettingsUpdater
    {
        public SettingsUpdaterGame(SettingsContainers currentSettings)
        {
            _current = currentSettings;
        }

        private readonly SettingsContainers _current;

        public void UpdateSettings()
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_current.game.language];
        }
    }
}