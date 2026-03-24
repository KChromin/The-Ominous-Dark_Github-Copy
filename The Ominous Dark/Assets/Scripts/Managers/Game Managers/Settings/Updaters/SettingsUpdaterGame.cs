using UnityEngine;
using UnityEngine.Localization.Settings;

namespace NOS.GameManagers.Settings
{
    public class SettingsUpdaterGame : ISettingsUpdater
    {
        public SettingsUpdaterGame(SettingsManager settingsManager)
        {
            Settings = settingsManager;
        }
        
        public SettingsManager Settings { get; set; }

        public void UpdateSettings()
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[Settings.CurrentSettings.game.Language];
        }
    }
}