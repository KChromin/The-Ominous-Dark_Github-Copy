using System;
using NOS.Patterns.Singleton;
using UnityEngine;
using UnityEngine.UIElements;

namespace NOS.GameManagers.Settings
{
    [DefaultExecutionOrder(-90)]
    public class SettingsManager : SingletonPersistent<SettingsManager>
    {
        #region Settings Containers

        [field: SerializeField]
        public SettingsContainers CurrentSettings { get; private set; } //Current settings//

        [field: Space, SerializeField]
        public SettingsContainers temporarySettings; //Temporary settings for settings UI//

        [Space, SerializeField]
        private SettingsScriptableObject defaultSettings; //For reverting to default settings//

        #endregion Settings Containers

        private SettingsManagerFileHandler _fileHandler;
        private SettingsUpdaters _updaters;
        private SettingsAutoConfigurations _autoConfigs;
        private SettingsGUI _gui;

        public enum SettingsType
        {
            Audio,
            Controls,
            Display,
            Game,
            Visuals,
            Accessibility
        }

        private bool _initializedActions;
        public Action OnSettingsUpdate; //For controllers to update cached values//

        #region Methodes

        protected override void Awake()
        {
            base.Awake(); //Set manager//

            //Setup classes
            CurrentSettings = new SettingsContainers();
            temporarySettings = new SettingsContainers();
            _autoConfigs = new SettingsAutoConfigurations(CurrentSettings);
            _fileHandler = new SettingsManagerFileHandler(this);
            _updaters = new SettingsUpdaters(this);

            //Events
            _fileHandler.OnCorruptedSettingsCheck += ResetSettings;
            _initializedActions = true;

            //Load current settings//
            _fileHandler.LoadFile();
            MoveSettingsCurrentToTemporary();

            //Update all async and ignore return value//
            UpdateAllSettingsNextFrame();
        }

        private void Start()
        {
            _gui = new SettingsGUI(GetComponent<UIDocument>(), this);
        }

        private void Update()
        {
            _gui.Update();
        }

        #region Settings Methodes

        #region Move Settings

        public void SwapTemporaryAndCurrentSettings()
        {
            (CurrentSettings, temporarySettings) = ((SettingsContainers)temporarySettings.Clone(), (SettingsContainers)CurrentSettings.Clone());
        }

        public void MoveSettingsCurrentToTemporary()
        {
            temporarySettings = (SettingsContainers)CurrentSettings.Clone();
        }

        public void MoveSettingsDefaultToCurrent()
        {
            CurrentSettings = (SettingsContainers)defaultSettings.settings.Clone();
        }

        public void ResetSettingsToDefaultOneCategory(SettingsType type)
        {
            bool[] settingToReset = new[]{true,true,true,true,true,true};

            switch (type)
            {
                case SettingsType.Audio:
                    settingToReset[0] = false;
                    break;
                default:
                case SettingsType.Controls:
                    settingToReset[1] = false;
                    break;
                case SettingsType.Display:
                    settingToReset[2] = false;
                    break;
                case SettingsType.Game:
                    settingToReset[3] = false;
                    break;
                case SettingsType.Visuals:
                    settingToReset[4] = false;
                    break;
                case SettingsType.Accessibility:
                    settingToReset[5] = false;
                    break;
            }

            ResetSettings(settingToReset);
        }

        #endregion Move Settings

        #region Reset Settings

        //Reset settings, first try to auto config, when not available then set to default//
        private void ResetSettings(bool[] settingsToSet)
        {
            //Audio//
            if (!settingsToSet[0])
            {
                SetSettingsToDefault(SettingsType.Audio);
            }

            //Control//
            if (!settingsToSet[1])
            {
                SetSettingsToDefault(SettingsType.Controls);
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
                SetSettingsToDefault(SettingsType.Visuals);
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
                SetSettingsToDefault(SettingsType.Controls);
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
                SetSettingsToDefault(SettingsType.Visuals);
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
                case SettingsType.Controls:
                    CurrentSettings.control = (SettingsControlContainer)defaultSettings.settings.control.Clone();
                    break;
                case SettingsType.Display:
                    CurrentSettings.display = (SettingsDisplayContainer)defaultSettings.settings.display.Clone();
                    break;
                case SettingsType.Game:
                    CurrentSettings.game = (SettingsGameContainer)defaultSettings.settings.game.Clone();
                    break;
                case SettingsType.Visuals:
                    CurrentSettings.visual = (SettingsVisualContainer)defaultSettings.settings.visual.Clone();
                    break;
                case SettingsType.Accessibility:
                    CurrentSettings.accessibility = (SettingsAccessibilityContainer)defaultSettings.settings.accessibility.Clone();
                    break;
            }
        }

        #endregion Reset Settings

        #region Apply Settings

        public void UpdateAllSettings()
        {
            _updaters.UpdateAllSettings();
        }

        public void UpdateAllSettingsNextFrame()
        {
            _updaters.UpdateAllSettingsNextFrame().Forget();
        }

        public void UpdateOneSetting(SettingsType settingsType)
        {
            _updaters.UpdateOneSetting(settingsType);
        }

        public void SaveCurrentSettings()
        {
            _fileHandler.SaveFile();
        }

        #endregion Apply Settings

        #endregion Settings Methodes

        private void OnDisable()
        {
            if (_initializedActions)
            {
                _fileHandler.OnCorruptedSettingsCheck -= ResetSettings;
            }
        }

        public void GUIOpenSettingsMenu()
        {
            _gui.OpenSettingsMenu();
        }
        
        public void GUICloseSettingsMenu()
        {
            _gui.CloseSettingsMenu(null);
        }

        public bool IsSettingsMenuOpen()
        {
            return _gui.IsSettingsMenuOpen();
        }

        #endregion Methodes
    }

    public static class SettingsValues
    {
        public static short GetShortValue(short value, Vector2Int limits, short fallbackValue = -1)
        {
            //If value is over the limits
            if (value < limits.x || value > limits.y)
            {
                //Clamp to limit
                if (fallbackValue == -1)
                {
                    value = (short)Mathf.RoundToInt(Mathf.Clamp(value, limits.x, limits.y));
                }
                else //Set Default
                {
                    value = fallbackValue;
                }
            }

            return value;
        }

        public static float GetFloatValue(float value, Vector2 limits, short fallbackValue = -1)
        {
            //If value is over the limits
            if (value < limits.x || value > limits.y)
            {
                //Clamp to limit
                if (fallbackValue == -1)
                {
                    value = Mathf.Clamp(value, limits.x, limits.y);
                }
                else //Set Default
                {
                    value = fallbackValue;
                }
            }

            return value;
        }
    }
}