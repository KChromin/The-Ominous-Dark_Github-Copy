using UnityEngine;
using UnityEngine.UIElements;

namespace NOS.GameManagers.Settings
{
    public class SettingsGUIControl : ISettingsGUI
    {
        public SettingsGUIControl(SettingsManager settingsManager, SettingsGUI settingsGUI, VisualElement uiRoot)
        {
            SettingsManager = settingsManager;
            SettingsGUI = settingsGUI;
            UiRoot = uiRoot;

            #region UI Elements

            //General
            _optionGeneralSensitivity = UiRoot.Q<VisualElement>("GeneralSensitivity").Q<Slider>("SettingsSlider");
            _optionGeneralSensitivityField = UiRoot.Q<VisualElement>("GeneralSensitivity").Q<FloatField>("SettingsSliderValue");
            _optionGeneralSensitivity.RegisterValueChangedCallback(_ => SettingsGUI.UpdateSliderFromFloatField(_optionGeneralSensitivity, _optionGeneralSensitivityField));
            _optionGeneralSensitivityField.RegisterValueChangedCallback(_ => SettingsGUI.UpdateFloatFieldFromSlider(_optionGeneralSensitivity, _optionGeneralSensitivityField));
            
            //2 Axes
            _optionDifferentAxesSensitivity = UiRoot.Q<VisualElement>("DifferentAxesSensitivity").Q<Toggle>("SettingsBool");
            _hiddenDifferentAxes = UiRoot.Q<VisualElement>("DifferentAxesHiddenPanel");
            _optionDifferentAxesSensitivity.RegisterCallback<ClickEvent>(UpdateHiddenPanelStates);

            _optionAxisXSensitivity = UiRoot.Q<VisualElement>("SettingsSliderSensitivityX").Q<Slider>("SettingsSlider");
            _optionAxisXSensitivityField = UiRoot.Q<VisualElement>("SettingsSliderSensitivityX").Q<FloatField>("SettingsSliderValue");
            _optionAxisXSensitivity.RegisterValueChangedCallback(_ => SettingsGUI.UpdateSliderFromFloatField(_optionAxisXSensitivity, _optionAxisXSensitivityField));
            _optionAxisXSensitivityField.RegisterValueChangedCallback(_ => SettingsGUI.UpdateFloatFieldFromSlider(_optionAxisXSensitivity, _optionAxisXSensitivityField));

            _optionAxisYSensitivity = UiRoot.Q<VisualElement>("SettingsSliderSensitivityY").Q<Slider>("SettingsSlider");
            _optionAxisYSensitivityField = UiRoot.Q<VisualElement>("SettingsSliderSensitivityY").Q<FloatField>("SettingsSliderValue");
            _optionAxisYSensitivity.RegisterValueChangedCallback(_ => SettingsGUI.UpdateSliderFromFloatField(_optionAxisYSensitivity, _optionAxisYSensitivityField));
            _optionAxisYSensitivityField.RegisterValueChangedCallback(_ => SettingsGUI.UpdateFloatFieldFromSlider(_optionAxisYSensitivity, _optionAxisYSensitivityField));

            //Invert Y
            _optionInvertedYAxis = UiRoot.Q<VisualElement>("InvertedYAxis").Q<Toggle>("SettingsBool");
            _optionInvertedYAxis.RegisterCallback<ClickEvent>(UpdateHiddenPanelStates);

            //Look Smoothing
            _optionLookSmoothing = UiRoot.Q<VisualElement>("LookSmoothing").Q<Toggle>("SettingsBool");
            _optionLookSmoothing.RegisterCallback<ClickEvent>(UpdateHiddenPanelStates);

            _hiddenLookSmoothing = UiRoot.Q<VisualElement>("LookSmoothingHiddenPanel");
            _optionLookSmoothingValue = UiRoot.Q<VisualElement>("LookSmoothingValue").Q<Slider>("SettingsSlider");
            _optionLookSmoothingValueField = UiRoot.Q<VisualElement>("LookSmoothingValue").Q<FloatField>("SettingsSliderValue");
            _optionLookSmoothingValue.RegisterValueChangedCallback(_ => SettingsGUI.UpdateSliderFromFloatField(_optionLookSmoothingValue, _optionLookSmoothingValueField));
            _optionLookSmoothingValueField.RegisterValueChangedCallback(_ => SettingsGUI.UpdateFloatFieldFromSlider(_optionLookSmoothingValue, _optionLookSmoothingValueField));

            #endregion UI Elements
        }

        #region Variables

        public SettingsManager SettingsManager { get; set; }
        public SettingsGUI SettingsGUI { get; set; }
        public VisualElement UiRoot { get; set; }

        #region UI Elements

        private readonly Slider _optionGeneralSensitivity;
        private readonly FloatField _optionGeneralSensitivityField;
        
        private readonly Toggle _optionDifferentAxesSensitivity;

        private readonly VisualElement _hiddenDifferentAxes;
        private readonly Slider _optionAxisXSensitivity;
        private readonly FloatField _optionAxisXSensitivityField;

        private readonly Slider _optionAxisYSensitivity;
        private readonly FloatField _optionAxisYSensitivityField;

        private readonly Toggle _optionInvertedYAxis;

        private readonly Toggle _optionLookSmoothing;
        private readonly VisualElement _hiddenLookSmoothing;
        private readonly Slider _optionLookSmoothingValue;
        private readonly FloatField _optionLookSmoothingValueField;

        #endregion UI Elements

        #endregion Variables

        public void SetupAllSettingsUI()
        {
            _optionGeneralSensitivity.value = SettingsManager.temporarySettings.control.LookSensitivityGeneral;
            _optionGeneralSensitivityField.value = SettingsManager.temporarySettings.control.LookSensitivityGeneral;
            
            _optionDifferentAxesSensitivity.value = SettingsManager.temporarySettings.control.LookSeparateSensitivityAxes;
            _optionAxisXSensitivity.value = SettingsManager.temporarySettings.control.LookSensitivityXAxis;
            _optionAxisXSensitivityField.value = SettingsManager.temporarySettings.control.LookSensitivityXAxis;
            _optionAxisYSensitivity.value = SettingsManager.temporarySettings.control.LookSensitivityYAxis;
            _optionAxisYSensitivityField.value = SettingsManager.temporarySettings.control.LookSensitivityYAxis;
            _optionInvertedYAxis.value = SettingsManager.temporarySettings.control.LookInvertYAxis;
            _optionLookSmoothing.value = SettingsManager.temporarySettings.control.LookSmoothing;
            _optionLookSmoothingValue.value = SettingsManager.temporarySettings.control.LookSmoothingTime;
            _optionLookSmoothingValueField.value = SettingsManager.temporarySettings.control.LookSmoothingTime;

            UpdateHiddenPanelStates(null);
        }

        public void UpdateSettingsFromUIValues()
        {
            SettingsManager.temporarySettings.control.LookSensitivityGeneral = _optionGeneralSensitivity.value;
            SettingsManager.temporarySettings.control.LookSeparateSensitivityAxes = _optionDifferentAxesSensitivity.value;
            SettingsManager.temporarySettings.control.LookSensitivityXAxis = _optionAxisXSensitivity.value;
            SettingsManager.temporarySettings.control.LookSensitivityYAxis = _optionAxisYSensitivity.value;
            SettingsManager.temporarySettings.control.LookInvertYAxis = _optionInvertedYAxis.value;
            SettingsManager.temporarySettings.control.LookSmoothing = _optionLookSmoothing.value;
            SettingsManager.temporarySettings.control.LookSmoothingTime = _optionLookSmoothingValue.value;
        }

        public bool HasUnsavedChanges()
        {
            UpdateSettingsFromUIValues();

            if (!Mathf.Approximately(SettingsManager.temporarySettings.control.LookSensitivityGeneral, _optionGeneralSensitivity.value))
            {
                return true;
            }

            if (SettingsManager.temporarySettings.control.LookSeparateSensitivityAxes != _optionDifferentAxesSensitivity.value)
            {
                return true;
            }

            if (!Mathf.Approximately(SettingsManager.temporarySettings.control.LookSensitivityXAxis, _optionAxisXSensitivity.value))
            {
                return true;
            }

            if (!Mathf.Approximately(SettingsManager.temporarySettings.control.LookSensitivityYAxis, _optionAxisYSensitivity.value))
            {
                return true;
            }

            if (SettingsManager.temporarySettings.control.LookInvertYAxis != _optionInvertedYAxis.value)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.control.LookSmoothing != _optionLookSmoothing.value)
            {
                return true;
            }

            if (!Mathf.Approximately(SettingsManager.temporarySettings.control.LookSmoothingTime, _optionLookSmoothingValue.value))
            {
                return true;
            }

            return false;
        }

        public void UpdateHiddenPanelStates(ClickEvent evt)
        {
            _hiddenDifferentAxes.style.display = DisplayStyle.None;
            _hiddenLookSmoothing.style.display = DisplayStyle.None;

            if (_optionDifferentAxesSensitivity.value)
            {
                _hiddenDifferentAxes.style.display = DisplayStyle.Flex;
            }

            if (_optionLookSmoothing.value)
            {
                _hiddenLookSmoothing.style.display = DisplayStyle.Flex;
            }
        }
    }
}