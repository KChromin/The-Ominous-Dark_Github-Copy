using UnityEngine;
using UnityEngine.UIElements;

namespace NOS.GameManagers.Settings
{
    public class SettingsGUIGame : ISettingsGUI
    {
        public SettingsGUIGame(SettingsManager settingsManager, SettingsGUI settingsGUI, VisualElement uiRoot)
        {
            SettingsManager = settingsManager;
            SettingsGUI = settingsGUI;
            UiRoot = uiRoot;

            #region UI Elements

            _optionLanguage = UiRoot.Q<VisualElement>("Language").Q<DropdownField>("SettingsDropdown");

            _optionFOV = UiRoot.Q<VisualElement>("FieldOfView").Q<Slider>("SettingsSlider");
            _optionFOVField = UiRoot.Q<VisualElement>("FieldOfView").Q<FloatField>("SettingsSliderValue");
            _optionFOV.RegisterValueChangedCallback(_ => SettingsGUI.UpdateSliderFromFloatField(_optionFOV, _optionFOVField));
            _optionFOVField.RegisterValueChangedCallback(_ => SettingsGUI.UpdateFloatFieldFromSlider(_optionFOV, _optionFOVField));
            
            _optionHeadBobbingIntensity = UiRoot.Q<VisualElement>("HeadBobbingIntensity").Q<Slider>("SettingsSlider");
            _optionHeadBobbingIntensityField = UiRoot.Q<VisualElement>("HeadBobbingIntensity").Q<FloatField>("SettingsSliderValue");
            _optionHeadBobbingIntensity.RegisterValueChangedCallback(_ => SettingsGUI.UpdateSliderFromFloatField(_optionHeadBobbingIntensity, _optionHeadBobbingIntensityField));
            _optionHeadBobbingIntensityField.RegisterValueChangedCallback(_ => SettingsGUI.UpdateFloatFieldFromSlider(_optionHeadBobbingIntensity, _optionHeadBobbingIntensityField));
            
            _optionInventoryLooping = UiRoot.Q<VisualElement>("InventoryItemScrollLoopsOnEnds").Q<Toggle>("SettingsBool");

            #endregion UI Elements
        }

        #region Variables

        public SettingsManager SettingsManager { get; set; }
        public SettingsGUI SettingsGUI { get; set; }
        public VisualElement UiRoot { get; set; }

        #region UI Elements

        private readonly DropdownField _optionLanguage;

        private readonly Slider _optionFOV;
        private readonly FloatField _optionFOVField;

        private readonly Slider _optionHeadBobbingIntensity;
        private readonly FloatField _optionHeadBobbingIntensityField;

        private readonly Toggle _optionInventoryLooping;

        #endregion UI Elements

        #endregion Variables
        
        public void SetupAllSettingsUI()
        {
            _optionLanguage.index = SettingsManager.temporarySettings.game.Language;

            _optionFOV.value = SettingsManager.temporarySettings.game.FieldOfView;
            _optionFOVField.value = SettingsManager.temporarySettings.game.FieldOfView;

            _optionHeadBobbingIntensity.value = SettingsManager.temporarySettings.game.HeadBobbingIntensity;
            _optionHeadBobbingIntensityField.value = SettingsManager.temporarySettings.game.HeadBobbingIntensity;

            _optionInventoryLooping.value = SettingsManager.temporarySettings.game.InventoryItemScrollLoopsOnEnds;
        }

        public void UpdateSettingsFromUIValues()
        {
            SettingsManager.temporarySettings.game.Language = (short)_optionLanguage.index;

            SettingsManager.temporarySettings.game.FieldOfView = _optionFOV.value;
            SettingsManager.temporarySettings.game.FieldOfView = _optionFOVField.value;

            SettingsManager.temporarySettings.game.HeadBobbingIntensity = _optionHeadBobbingIntensity.value;
            SettingsManager.temporarySettings.game.HeadBobbingIntensity = _optionHeadBobbingIntensityField.value;

            SettingsManager.temporarySettings.game.InventoryItemScrollLoopsOnEnds = _optionInventoryLooping.value;
        }

        public bool HasUnsavedChanges()
        {
            if (SettingsManager.temporarySettings.game.Language != (short)_optionLanguage.index)
            {
                return true;
            }

            if (!Mathf.Approximately(SettingsManager.temporarySettings.game.FieldOfView, _optionFOV.value))
            {
                return true;
            }

            if (!Mathf.Approximately(SettingsManager.temporarySettings.game.FieldOfView, _optionFOVField.value))
            {
                return true;
            }

            if (!Mathf.Approximately(SettingsManager.temporarySettings.game.HeadBobbingIntensity, _optionHeadBobbingIntensity.value))
            {
                return true;
            }

            if (!Mathf.Approximately(SettingsManager.temporarySettings.game.HeadBobbingIntensity, _optionHeadBobbingIntensityField.value))
            {
                return true;
            }

            if (SettingsManager.temporarySettings.game.InventoryItemScrollLoopsOnEnds != _optionInventoryLooping.value)
            {
                return true;
            }

            return false;
        }

        public void UpdateHiddenPanelStates(ClickEvent evt)
        {
        }
    }
}