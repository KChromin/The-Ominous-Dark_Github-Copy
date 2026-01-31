using System;
using UnityEngine.UIElements;

namespace NOS.GameManagers.Settings
{
    public class SettingsGUI
    {
        public SettingsGUI(UIDocument uiDocument)
        {
            _document = uiDocument;
            _root = _document.rootVisualElement;

            #region Settings Main Panel

            _settingsWindow = _root.Q<VisualElement>("SettingsContainer");

            #region Visual Elements

            //Categories
            _controlSettings = _root.Q<VisualElement>("SettingsPanelControls");
            _displaySettings = _root.Q<VisualElement>("SettingsPanelDisplay");

            #endregion Visual Elements

            #region Buttons

            _controlsButton = _root.Q<Button>("SettingsCategoryControls");
            _controlsButton.RegisterCallback<ClickEvent>(evt => ChangeCategoryEvent(evt, SettingsCategories.Controls));
            _audioButton = _root.Q<Button>("SettingsCategoryAudio");
            _gameButton = _root.Q<Button>("SettingsCategoryGame");
            _displayButton = _root.Q<Button>("SettingsCategoryDisplay");
            _displayButton.RegisterCallback<ClickEvent>(evt => ChangeCategoryEvent(evt, SettingsCategories.Display));
            _visualsButton = _root.Q<Button>("SettingsCategoryVisuals");
            _accessibilityButton = _root.Q<Button>("SettingsCategoryAccessibility");

            //Functional
            _applyButton = _root.Q<Button>("SettingsApply");
            _defaultButton = _root.Q<Button>("SettingsDefault");
            _returnButton = _root.Q<Button>("SettingsReturn");
            _returnButton.RegisterCallback<ClickEvent>(CloseMenu);

            //Confirmation
            _confirmButtonTimer = _root.Q<Button>("ConfirmationButton");
            _cancelButtonTimer = _root.Q<Button>("CancelButton");
            _confirmButton = _root.Q<Button>("ResetToDefaultButton");
            _cancelButton = _root.Q<Button>("CancelResetToDefaultButton");

            #endregion Buttons

            #endregion Settings Main Panel
        }

        #region Variables

        private UIDocument _document;
        private VisualElement _root;

        #region Main Settings Panel

        private readonly VisualElement _settingsWindow;

        #region Buttons

        //Categories
        private Button _controlsButton;
        private Button _audioButton;
        private Button _gameButton;
        private Button _displayButton;
        private Button _visualsButton;
        private Button _accessibilityButton;

        //Functional
        private Button _applyButton;
        private Button _defaultButton;
        private Button _returnButton;

        //Confirmation
        private Button _confirmButtonTimer;
        private Button _cancelButtonTimer;
        private Button _confirmButton;
        private Button _cancelButton;

        #endregion Buttons

        #region Visual Elements

        //Categories
        private VisualElement _controlSettings;
        private VisualElement _audioSettings;
        private VisualElement _gameSettings;
        private VisualElement _displaySettings;
        private VisualElement _visualsSettings;
        private VisualElement _accessibilitySettings;

        #endregion Visual Elements

        private enum SettingsCategories
        {
            Controls,
            Audio,
            Game,
            Display,
            Visuals,
            Accessibility
        }

        #endregion Main Settings Panel

        #endregion Variables

        #region Settings Main Panel

        private void CloseAllSettingsPanels()
        {
            _controlSettings.style.display = DisplayStyle.None;
            _displaySettings.style.display = DisplayStyle.None;
        }

        private void ChangeCategoryEvent(ClickEvent evt, SettingsCategories category)
        {
            CloseAllSettingsPanels();

            switch (category)
            {
                default:
                case SettingsCategories.Controls:
                    _controlSettings.style.display = DisplayStyle.Flex;
                    break;
                case SettingsCategories.Audio:
                    break;
                case SettingsCategories.Game:
                    break;
                case SettingsCategories.Display:
                    _displaySettings.style.display = DisplayStyle.Flex;
                    break;
                case SettingsCategories.Visuals:
                    break;
                case SettingsCategories.Accessibility:
                    break;
            }
        }

        private void CloseMenu(ClickEvent evt)
        {
            CloseAllSettingsPanels();
            _controlSettings.style.display = DisplayStyle.Flex;
            _settingsWindow.style.display = DisplayStyle.None;
        }


        public void OpenMenu()
        {
            CloseAllSettingsPanels();
            _controlSettings.style.display = DisplayStyle.Flex;
            _settingsWindow.style.display = DisplayStyle.Flex;
        }

        #endregion Settings Main Panel
    }
}