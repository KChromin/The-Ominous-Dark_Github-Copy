using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace NOS.GameManagers.Settings
{
    public class SettingsGUI
    {
        public SettingsGUI(UIDocument uiDocument, SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            VisualElement root = uiDocument.rootVisualElement;

            #region Settings Main Panel

            _mainSettingsWindow = root.Q<VisualElement>("SettingsContainer");

            #region Visual Elements

            //Categories
            _controlSettingsWindow = root.Q<VisualElement>("SettingsPanelControls");
            _displaySettingsWindow = root.Q<VisualElement>("SettingsPanelDisplay");
            _gameSettingsWindow = root.Q<VisualElement>("SettingsPanelGame");

            //Confirmation
            _confirmationPanel = root.Q<VisualElement>("ConfirmationPanel");

            #endregion Visual Elements

            #region Buttons

            //Categories
            Button controlsPanelButton = root.Q<Button>("SettingsCategoryControls");
            controlsPanelButton.RegisterCallback<ClickEvent, SettingsManager.SettingsType>(ChangeSettingsCategory, SettingsManager.SettingsType.Controls);

            Button audioPanelButton = root.Q<Button>("SettingsCategoryAudio");
            audioPanelButton.RegisterCallback<ClickEvent, SettingsManager.SettingsType>(ChangeSettingsCategory, SettingsManager.SettingsType.Audio);

            Button gamePanelButton = root.Q<Button>("SettingsCategoryGame");
            gamePanelButton.RegisterCallback<ClickEvent, SettingsManager.SettingsType>(ChangeSettingsCategory, SettingsManager.SettingsType.Game);

            Button displayPanelButton = root.Q<Button>("SettingsCategoryDisplay");
            displayPanelButton.RegisterCallback<ClickEvent, SettingsManager.SettingsType>(ChangeSettingsCategory, SettingsManager.SettingsType.Display);

            Button visualsPanelButton = root.Q<Button>("SettingsCategoryVisuals");
            visualsPanelButton.RegisterCallback<ClickEvent, SettingsManager.SettingsType>(ChangeSettingsCategory, SettingsManager.SettingsType.Visuals);

            Button accessibilityPanelButton = root.Q<Button>("SettingsCategoryAccessibility");
            accessibilityPanelButton.RegisterCallback<ClickEvent, SettingsManager.SettingsType>(ChangeSettingsCategory, SettingsManager.SettingsType.Accessibility);


            //Functional
            Button sidePanelApplyButton = root.Q<Button>("SettingsApply");
            sidePanelApplyButton.RegisterCallback<ClickEvent, ConfirmationModes>(OpenConfirmationWindow, ConfirmationModes.ApplySettings);

            Button sidePanelRevertToDefaultButton = root.Q<Button>("SettingsDefault");
            sidePanelRevertToDefaultButton.RegisterCallback<ClickEvent, ConfirmationModes>(OpenConfirmationWindow, ConfirmationModes.ResetToDefault);

            Button sidePanelExitMenuButton = root.Q<Button>("SettingsReturn");
            sidePanelExitMenuButton.RegisterCallback<ClickEvent>(CloseSettingsMenu);

            //Confirmation
            Button confirmationApplyButton = root.Q<Button>("ApplyButton");
            confirmationApplyButton.RegisterCallback<ClickEvent>(ConfirmationApply);

            _confirmationCancelButton = root.Q<Button>("CancelButton");
            _confirmationCancelButton.RegisterCallback<ClickEvent>(ConfirmationCancel);

            #endregion Buttons

            #region Label

            //Confirmation
            _confirmationWindowTitle = root.Q<Label>("ConfirmationTitle");
            _confirmationWindowResetCountdown = root.Q<Label>("ConfirmationResetTime");

            #endregion Label

            #region Settings GUI Controllers

            _settingsGUIControl = new SettingsGUIControl(settingsManager, this, root);
            _settingsGUIDisplay = new SettingsGUIDisplay(settingsManager, this, root);
            _settingsGUIGame = new SettingsGUIGame(settingsManager, this, root);

            #endregion Settings GUI Controllers

            #endregion Settings Main Panel
        }

        #region Variables

        private readonly SettingsManager _settingsManager;

        //Countdown
        private bool _updateCountdown;
        private float _currentCountdownTime;
        private const int CountdownTime = 10;

        private SettingsManager.SettingsType _currentSettingsType;
        private const SettingsManager.SettingsType DefaultSettingsType = SettingsManager.SettingsType.Controls;

        private ConfirmationModes _currentConfirmationMode;

        private enum ConfirmationModes
        {
            ApplySettings,
            ResetToDefault,
            UnsavedChanges
        }

        #region Settings GUI Controllers

        private readonly SettingsGUIControl _settingsGUIControl;
        private readonly SettingsGUIDisplay _settingsGUIDisplay;
        private readonly SettingsGUIGame _settingsGUIGame;

        #endregion Settings GUI Controllers

        #region Main Settings Panel

        private readonly VisualElement _mainSettingsWindow;

        #region Buttons

        //Confirmation
        private readonly Button _confirmationCancelButton;

        #endregion Buttons

        #region Visual Elements

        //Categories
        private readonly VisualElement _controlSettingsWindow;
        private readonly VisualElement _audioSettingsWindow;
        private readonly VisualElement _gameSettingsWindow;
        private readonly VisualElement _displaySettingsWindow;
        private readonly VisualElement _visualsSettingsWindow;
        private readonly VisualElement _accessibilitySettingsWindow;

        //Confirmation
        private readonly VisualElement _confirmationPanel;

        #endregion Visual Elements

        #region Labels

        //Confirmation
        private readonly Label _confirmationWindowTitle;
        private readonly Label _confirmationWindowResetCountdown;

        #endregion Labels

        #endregion Main Settings Panel

        #region Localization

        //Confirmation Localization
        private readonly LocalizedString _confirmationTextApplySettings = new("GUITable", "ConfirmationLabelSaveSettings");
        private readonly LocalizedString _confirmationTextResetToDefault = new("GUITable", "ConfirmationLabelResetToDefault");
        private readonly LocalizedString _confirmationTextUnsavedChanges = new("GUITable", "ConfirmationLabelUnsavedChanges");
        private readonly LocalizedString _confirmationButtonTextCancel = new("GUITable", "ConfirmationButtonCancel");
        private readonly LocalizedString _confirmationButtonTextRevert = new("GUITable", "ConfirmationButtonRevert");

        #endregion Localization

        #endregion Variables

        #region Methodes

        public void Update()
        {
            UpdateCountdown();
        }

        #region Settings Category Panels

        private void ChangeSettingsCategory(ClickEvent evt, SettingsManager.SettingsType category)
        {
            if (CategoryHasUnsavedChanges(_currentSettingsType))
            {
                OpenConfirmationWindow(null, ConfirmationModes.UnsavedChanges);
                return;
            }

            //Update current Category
            _currentSettingsType = category;

            CloseAllSettingsPanels();
            OpenSettingPanel();
        }

        private void OpenSettingPanel()
        {
            UpdateCurrentSettingPanelUI();

            switch (_currentSettingsType)
            {
                default:
                case SettingsManager.SettingsType.Controls:
                    _controlSettingsWindow.style.display = DisplayStyle.Flex;
                    break;
                case SettingsManager.SettingsType.Audio:
                    break;
                case SettingsManager.SettingsType.Game:
                    _gameSettingsWindow.style.display = DisplayStyle.Flex;
                    break;
                case SettingsManager.SettingsType.Display:
                    _displaySettingsWindow.style.display = DisplayStyle.Flex;
                    break;
                case SettingsManager.SettingsType.Visuals:
                    break;
                case SettingsManager.SettingsType.Accessibility:
                    break;
            }
        }

        private void CloseAllSettingsPanels()
        {
            _controlSettingsWindow.style.display = DisplayStyle.None;
            _displaySettingsWindow.style.display = DisplayStyle.None;
            _gameSettingsWindow.style.display = DisplayStyle.None;
        }

        private void UpdateCurrentSettingPanelUI()
        {
            switch (_currentSettingsType)
            {
                case SettingsManager.SettingsType.Audio:
                    break;
                default:
                case SettingsManager.SettingsType.Controls:
                    _settingsGUIControl.SetupAllSettingsUI();
                    break;
                case SettingsManager.SettingsType.Display:
                    _settingsGUIDisplay.SetupAllSettingsUI();
                    break;
                case SettingsManager.SettingsType.Game:
                    _settingsGUIGame.SetupAllSettingsUI();
                    break;
                case SettingsManager.SettingsType.Visuals:
                    break;
                case SettingsManager.SettingsType.Accessibility:
                    break;
            }
        }

        private void UpdateTemporarySettingsFromCurrentUIPanel()
        {
            switch (_currentSettingsType)
            {
                case SettingsManager.SettingsType.Audio:
                    break;
                default:
                case SettingsManager.SettingsType.Controls:
                    _settingsGUIControl.UpdateSettingsFromUIValues();
                    break;
                case SettingsManager.SettingsType.Display:
                    _settingsGUIDisplay.UpdateSettingsFromUIValues();
                    break;
                case SettingsManager.SettingsType.Game:
                    _settingsGUIGame.UpdateSettingsFromUIValues();
                    break;
                case SettingsManager.SettingsType.Visuals:
                    break;
                case SettingsManager.SettingsType.Accessibility:
                    break;
            }
        }

        #endregion Settings Category Panels

        #region Main Settings Window

        public void OpenSettingsMenu()
        {
            //On open set default settings//
            _currentSettingsType = DefaultSettingsType;

            //Update Temporary Settings
            MoveCurrentSettingsToTemporary();

            //Panels
            CloseAllSettingsPanels();
            OpenSettingPanel();
            _mainSettingsWindow.style.display = DisplayStyle.Flex;
        }

        public void CloseSettingsMenu(ClickEvent evt)
        {
            _mainSettingsWindow.style.display = DisplayStyle.None;
        }
        
        public bool IsSettingsMenuOpen()
        {
            return _mainSettingsWindow.style.display == DisplayStyle.Flex;
        }

        #endregion Main Settings Window

        #region Confirmation Window

        #region Confirmation Countdown

        private void UpdateCountdown()
        {
            if (!_updateCountdown) return;

            _currentCountdownTime -= Time.unscaledDeltaTime;

            _confirmationWindowResetCountdown.text = Mathf.RoundToInt(_currentCountdownTime).ToString();

            //Reset settings to previous
            if (_currentCountdownTime <= 0)
            {
                ConfirmationApplyRevert();
                CloseConfirmationWindow();
            }
        }

        private void ActivateCountdown()
        {
            _updateCountdown = true;
            _currentCountdownTime = CountdownTime;
            _confirmationWindowResetCountdown.style.display = DisplayStyle.Flex;
        }

        private void CancelCountdown()
        {
            _updateCountdown = false;
            _confirmationWindowResetCountdown.style.display = DisplayStyle.None;
        }

        #endregion Confirmation Countdown

        #region Confirmation Panel

        private void SetupConfirmationWindow()
        {
            CancelCountdown();

            switch (_currentConfirmationMode)
            {
                default:
                case ConfirmationModes.ApplySettings:
                    ActivateCountdown();
                    _confirmationWindowTitle.text = _confirmationTextApplySettings.GetLocalizedString();
                    _confirmationCancelButton.text = _confirmationButtonTextCancel.GetLocalizedString();
                    break;
                case ConfirmationModes.ResetToDefault:
                    _confirmationWindowTitle.text = _confirmationTextResetToDefault.GetLocalizedString();
                    _confirmationCancelButton.text = _confirmationButtonTextCancel.GetLocalizedString();
                    break;
                case ConfirmationModes.UnsavedChanges:
                    _confirmationWindowTitle.text = _confirmationTextUnsavedChanges.GetLocalizedString();
                    _confirmationCancelButton.text = _confirmationButtonTextRevert.GetLocalizedString();
                    break;
            }

            _confirmationPanel.style.display = DisplayStyle.Flex;
        }

        private void OpenConfirmationWindow(ClickEvent evt, ConfirmationModes confirmationMode)
        {
            //Update current Confirmation Mode
            _currentConfirmationMode = confirmationMode;

            UpdateTemporarySettingsFromCurrentUIPanel();

            //Only Apply needs action at this point//
            if (_currentConfirmationMode == ConfirmationModes.ApplySettings)
            {
                //Swap
                SwapCurrentSettingsWithTemporary();
                //Apply
                ApplyCurrentSettings();
                //Ask if fully apply & set timer
                ActivateCountdown();
            }

            SetupConfirmationWindow();

            _confirmationPanel.style.display = DisplayStyle.Flex;
        }

        private void CloseConfirmationWindow()
        {
            _confirmationPanel.style.display = DisplayStyle.None;
        }

        #endregion Confirmation Panel

        #region Confirmation Buttons

        private void ConfirmationApply(ClickEvent evt)
        {
            switch (_currentConfirmationMode)
            {
                default:
                case ConfirmationModes.ApplySettings:
                    MoveCurrentSettingsToTemporary();
                    UpdateCurrentSettingPanelUI();
                    SaveCurrentSettings();
                    CancelCountdown();
                    break;
                case ConfirmationModes.ResetToDefault:
                    ResetCurrentSettingsCategory();
                    MoveCurrentSettingsToTemporary();
                    ApplyCurrentSettings();
                    UpdateCurrentSettingPanelUI();
                    SaveCurrentSettings();
                    break;
                case ConfirmationModes.UnsavedChanges:
                    CloseConfirmationWindow();
                    OpenConfirmationWindow(null, ConfirmationModes.ApplySettings);
                    return;
            }

            CloseConfirmationWindow();
        }

        private void ConfirmationCancel(ClickEvent evt)
        {
            switch (_currentConfirmationMode)
            {
                default:
                case ConfirmationModes.ApplySettings:
                    ConfirmationApplyRevert();
                    break;
                case ConfirmationModes.ResetToDefault:
                    break;
                case ConfirmationModes.UnsavedChanges:
                    MoveCurrentSettingsToTemporary();
                    UpdateCurrentSettingPanelUI();
                    break;
            }

            CloseConfirmationWindow();
        }

        private void ConfirmationApplyRevert()
        {
            CancelCountdown();
            SwapCurrentSettingsWithTemporary();
            MoveCurrentSettingsToTemporary();
            ApplyCurrentSettings();
            UpdateCurrentSettingPanelUI();
        }

        #endregion Confirmation Buttons

        #endregion Confirmation Window

        private bool CategoryHasUnsavedChanges(SettingsManager.SettingsType category)
        {
            switch (category)
            {
                case SettingsManager.SettingsType.Audio:

                    break;
                case SettingsManager.SettingsType.Controls:
                    return _settingsGUIControl.HasUnsavedChanges();
                case SettingsManager.SettingsType.Display:
                    return _settingsGUIDisplay.HasUnsavedChanges();
                case SettingsManager.SettingsType.Game:
                    return _settingsGUIGame.HasUnsavedChanges();
                case SettingsManager.SettingsType.Visuals:

                    break;
                case SettingsManager.SettingsType.Accessibility:

                    break;
            }

            return false;
        }

        private void SwapCurrentSettingsWithTemporary()
        {
            _settingsManager.SwapTemporaryAndCurrentSettings();
        }

        private void MoveCurrentSettingsToTemporary()
        {
            _settingsManager.MoveSettingsCurrentToTemporary();
        }

        private void ResetCurrentSettingsCategory()
        {
            _settingsManager.ResetSettingsToDefaultOneCategory(_currentSettingsType);
        }


        private void ApplyCurrentSettings()
        {
            _settingsManager.UpdateOneSetting(_currentSettingsType);
        }

        private void SaveCurrentSettings()
        {
            _settingsManager.SaveCurrentSettings();
        }

        #region Helper Methodes

        public static void UpdateSliderFromFloatField(Slider slider, FloatField field)
        {
            if (!slider.hasFocusPseudoState) return;

            field.value = slider.value;
        }

        public static void UpdateFloatFieldFromSlider(Slider slider, FloatField field)
        {
            if (!field.hasFocusPseudoState) return;

            field.value = Mathf.Clamp(field.value, slider.lowValue, slider.highValue);
            slider.value = field.value;
        }

        #endregion Helper Methodes

        #endregion Methodes
    }
}