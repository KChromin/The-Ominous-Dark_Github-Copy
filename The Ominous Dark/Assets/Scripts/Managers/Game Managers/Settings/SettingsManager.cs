using NOS.Patterns.Singleton;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [DefaultExecutionOrder(-90)]
    public class SettingsManager : SingletonPersistent<SettingsManager>
    {
        #region Settings Containers

        [field: SerializeField]
        public SettingsContainers CurrentSettings { get; private set; } //Current settings//

        [Space, SerializeField]
        private SettingsContainers temporarySettings; //Temporary settings for settings UI//
        [Space, SerializeField]
        private SettingsScriptableObject defaultSettings; //For reverting to default settings//

        #endregion Settings Containers

        private SettingsManagerFileHandler _fileHandler;
        private SettingsUpdaters _updaters;
        private SettingsAutoConfigurations _autoConfigs;

        public enum SettingsType
        {
            Audio,
            Control,
            Display,
            Game,
            Visual,
            Accessibility
        }

        #region Methodes

        protected override void Awake()
        {
            base.Awake(); //Set manager//

            //Setup classes
            CurrentSettings = new SettingsContainers();
            temporarySettings = new SettingsContainers();
            _autoConfigs = new SettingsAutoConfigurations(CurrentSettings);
            _fileHandler = new SettingsManagerFileHandler(CurrentSettings);
            _updaters = new SettingsUpdaters(CurrentSettings);

            //Events
            _fileHandler.OnCorruptedSettingsCheck += ResetSettings;

            //Load current settings//
            _fileHandler.LoadFile();
            MoveSettingsCurrentToTemporary();

            //Update all async and ignore return value//
            _updaters.UpdateAllSettingsNextFrame().Forget();
        }

        #region Move Settings

        private void MoveSettingsTemporaryToCurrent()
        {
            CurrentSettings = (SettingsContainers)temporarySettings.Clone();
        }

        private void MoveSettingsCurrentToTemporary()
        {
            temporarySettings = (SettingsContainers)CurrentSettings.Clone();
        }

        private void MoveSettingsDefaultToCurrent()
        {
            CurrentSettings = (SettingsContainers)defaultSettings.settings.Clone();
        }

        #endregion Move Settings

        //Reset settings, first try to auto config, when not available then set to default//
        private void ResetSettings(bool[] settingsToSet) //todo - here autoconfig//
        {
            //Audio//
            if (!settingsToSet[0])
            {
                SetSettingsToDefault(SettingsType.Audio);
            }

            //Control//
            if (!settingsToSet[1])
            {
                SetSettingsToDefault(SettingsType.Control);
            }

            //Display//
            if (!settingsToSet[2])
            {
                SetSettingsToDefault(SettingsType.Display); //First set to defaults
                _autoConfigs.AutoConfigureSettings(SettingsAutoConfigurations.SettingsType.Display); //Then try to auto config
            }

            //Game//
            if (!settingsToSet[3])
            {
                SetSettingsToDefault(SettingsType.Game);
            }

            //Visual//
            if (!settingsToSet[4])
            {
                SetSettingsToDefault(SettingsType.Visual);
                _autoConfigs.AutoConfigureSettings(SettingsAutoConfigurations.SettingsType.Visual);
            }

            //Accessibility
            if (!settingsToSet[5])
            {
                SetSettingsToDefault(SettingsType.Accessibility);
            }

            _fileHandler.SaveFile();
        }

        private void SetToDefaultSettings(bool[] settingsToSet)
        {
            //Audio//
            if (!settingsToSet[0])
            {
                SetSettingsToDefault(SettingsType.Audio);
            }

            //Control//
            if (!settingsToSet[1])
            {
                SetSettingsToDefault(SettingsType.Control);
            }

            //Display//
            if (!settingsToSet[2])
            {
                SetSettingsToDefault(SettingsType.Display);
            }

            //Game//
            if (!settingsToSet[3])
            {
                SetSettingsToDefault(SettingsType.Game);
            }

            //Visual//
            if (!settingsToSet[4])
            {
                SetSettingsToDefault(SettingsType.Visual);
            }

            //Accessibility
            if (!settingsToSet[5])
            {
                SetSettingsToDefault(SettingsType.Accessibility);
            }

            _fileHandler.SaveFile();
        }

        private void SetSettingsToDefault(SettingsType settingsType)
        {
            switch (settingsType)
            {
                default:
                case SettingsType.Audio:
                    CurrentSettings.audio = (SettingsAudioContainer)defaultSettings.settings.audio.Clone();
                    break;
                case SettingsType.Control:
                    CurrentSettings.control = (SettingsControlContainer)defaultSettings.settings.control.Clone();
                    break;
                case SettingsType.Display:
                    CurrentSettings.display = (SettingsDisplayContainer)defaultSettings.settings.display.Clone();
                    break;
                case SettingsType.Game:
                    CurrentSettings.game = (SettingsGameContainer)defaultSettings.settings.game.Clone();
                    break;
                case SettingsType.Visual:
                    CurrentSettings.visual = (SettingsVisualContainer)defaultSettings.settings.visual.Clone();
                    break;
                case SettingsType.Accessibility:
                    CurrentSettings.accessibility = (SettingsAccessibilityContainer)defaultSettings.settings.accessibility.Clone();
                    break;
            }
        }

        #region Load Save Settings

        private void OnDisable()
        {
            _fileHandler.OnCorruptedSettingsCheck -= ResetSettings;
        }

        #endregion Load Save Settings

        #endregion Methodes
    }
}