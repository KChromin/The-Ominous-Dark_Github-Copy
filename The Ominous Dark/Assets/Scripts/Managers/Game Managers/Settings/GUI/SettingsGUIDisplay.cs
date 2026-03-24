using UnityEngine.UIElements;

namespace NOS.GameManagers.Settings
{
    public class SettingsGUIDisplay : ISettingsGUI
    {
        public SettingsGUIDisplay(SettingsManager settingsManager, SettingsGUI settingsGUI, VisualElement uiRoot)
        {
            SettingsManager = settingsManager;
            SettingsGUI = settingsGUI;
            UiRoot = uiRoot;

            #region UI Elements

            _optionFullScreen = UiRoot.Q<VisualElement>("FullscreenMode").Q<DropdownField>("SettingsDropdown");
            _optionAspectRatio = UiRoot.Q<VisualElement>("AspectRatio").Q<DropdownField>("SettingsDropdown");
            _optionAspectRatio.RegisterValueChangedCallback(_ => UpdateHiddenPanelStates(null));

            _hidden16X9Ratio = UiRoot.Q<VisualElement>("16x9HiddenPanel");
            _option16X9Ratio = UiRoot.Q<VisualElement>("16x9Options").Q<DropdownField>("SettingsDropdown");
            _hidden16X10Ratio = UiRoot.Q<VisualElement>("16x10HiddenPanel");
            _option16X10Ratio = UiRoot.Q<VisualElement>("16x10Options").Q<DropdownField>("SettingsDropdown");
            _hidden21X9Ratio = UiRoot.Q<VisualElement>("21x9HiddenPanel");
            _option21X9Ratio = UiRoot.Q<VisualElement>("21x9Options").Q<DropdownField>("SettingsDropdown");

            _customResolutionToggle = UiRoot.Q<VisualElement>("CustomResolution").Q<Toggle>("SettingsBool");
            _customResolutionToggle.RegisterCallback<ClickEvent>(UpdateHiddenPanelStates);

            _hiddenCustomResolution = UiRoot.Q<VisualElement>("CustomResolutionHiddenPanel");
            _customHeight = UiRoot.Q<VisualElement>("CustomHeight").Q<IntegerField>("IntegerOption");

            _customWidth = UiRoot.Q<VisualElement>("CustomWidth").Q<IntegerField>("IntegerOption");

            _vSyncMode = UiRoot.Q<VisualElement>("VSyncMode").Q<DropdownField>("SettingsDropdown");

            _maxFramerateToggle = UiRoot.Q<VisualElement>("MaxFrameRate").Q<Toggle>("SettingsBool");
            _maxFramerateToggle.RegisterCallback<ClickEvent>(UpdateHiddenPanelStates);

            _hiddenMaxFramerate = UiRoot.Q<VisualElement>("CustomFrameRateHiddenPanel");
            _hiddenMaxFramerate.RegisterCallback<ClickEvent>(UpdateHiddenPanelStates);

            _maxFramerateValue = UiRoot.Q<VisualElement>("MaxFramerateValue").Q<IntegerField>("IntegerOption");

            _runInBackgroundToggle = UiRoot.Q<VisualElement>("RunInBackground").Q<Toggle>("SettingsBool");

            #endregion UI Elements
        }

        #region Variables

        public SettingsManager SettingsManager { get; set; }
        public SettingsGUI SettingsGUI { get; set; }
        public VisualElement UiRoot { get; set; }

        #region UI Elements

        private readonly DropdownField _optionFullScreen;
        private readonly DropdownField _optionAspectRatio;

        private readonly VisualElement _hidden16X9Ratio;
        private readonly DropdownField _option16X9Ratio;
        private readonly VisualElement _hidden16X10Ratio;
        private readonly DropdownField _option16X10Ratio;
        private readonly VisualElement _hidden21X9Ratio;
        private readonly DropdownField _option21X9Ratio;

        private readonly Toggle _customResolutionToggle;
        private readonly VisualElement _hiddenCustomResolution;
        private readonly IntegerField _customHeight;
        private readonly IntegerField _customWidth;

        private readonly DropdownField _vSyncMode;

        private readonly Toggle _maxFramerateToggle;
        private readonly VisualElement _hiddenMaxFramerate;
        private readonly IntegerField _maxFramerateValue;

        private readonly Toggle _runInBackgroundToggle;

        #endregion UI Elements

        #endregion Variables

        public void SetupAllSettingsUI()
        {
            _optionFullScreen.index = SettingsManager.temporarySettings.display.FullscreenMode;
            _optionAspectRatio.index = SettingsManager.temporarySettings.display.AspectRatio;

            _option16X9Ratio.index = SettingsManager.temporarySettings.display.ResolutionPreset16X9;
            _option16X10Ratio.index = SettingsManager.temporarySettings.display.ResolutionPreset16X10;
            _option21X9Ratio.index = SettingsManager.temporarySettings.display.ResolutionPreset21X9;

            _customResolutionToggle.value = SettingsManager.temporarySettings.display.ResolutionCustom;

            _customHeight.value = SettingsManager.temporarySettings.display.ResolutionCustomHeight;
            _customWidth.value = SettingsManager.temporarySettings.display.ResolutionCustomWidth;

            _vSyncMode.index = SettingsManager.temporarySettings.display.VSyncMode;

            _maxFramerateToggle.value = SettingsManager.temporarySettings.display.FramerateCustomMax;


            _maxFramerateValue.value = SettingsManager.temporarySettings.display.FramerateCustomMaxValue;

            _runInBackgroundToggle.value = SettingsManager.temporarySettings.display.RunInBackground;

            UpdateHiddenPanelStates(null);
        }

        public void UpdateSettingsFromUIValues()
        {
            SettingsManager.temporarySettings.display.FullscreenMode = (short)_optionFullScreen.index;

            SettingsManager.temporarySettings.display.AspectRatio = (short)_optionAspectRatio.index;
            SettingsManager.temporarySettings.display.ResolutionPreset16X9 = (short)_option16X9Ratio.index;
            SettingsManager.temporarySettings.display.ResolutionPreset16X10 = (short)_option16X10Ratio.index;
            SettingsManager.temporarySettings.display.ResolutionPreset21X9 = (short)_option21X9Ratio.index;

            SettingsManager.temporarySettings.display.ResolutionCustom = _customResolutionToggle.value;

            SettingsManager.temporarySettings.display.ResolutionCustomHeight = (short)_customHeight.value;
            SettingsManager.temporarySettings.display.ResolutionCustomWidth = (short)_customWidth.value;

            SettingsManager.temporarySettings.display.VSyncMode = (short)_vSyncMode.index;

            SettingsManager.temporarySettings.display.FramerateCustomMax = _maxFramerateToggle.value;

            SettingsManager.temporarySettings.display.FramerateCustomMaxValue = (short)_maxFramerateValue.value;

            SettingsManager.temporarySettings.display.RunInBackground = _runInBackgroundToggle.value;
        }

        // ReSharper disable once CognitiveComplexity
        public bool HasUnsavedChanges()
        {
            UpdateSettingsFromUIValues();

            if (SettingsManager.temporarySettings.display.FullscreenMode != (short)_optionFullScreen.index)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.AspectRatio != (short)_optionAspectRatio.index)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.ResolutionPreset16X9 != (short)_option16X9Ratio.index)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.ResolutionPreset16X10 != (short)_option16X10Ratio.index)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.ResolutionPreset21X9 != (short)_option21X9Ratio.index)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.ResolutionCustom != _customResolutionToggle.value)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.ResolutionCustomHeight != (short)_customHeight.value)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.ResolutionCustomWidth != (short)_customWidth.value)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.VSyncMode != (short)_vSyncMode.index)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.FramerateCustomMax != _maxFramerateToggle.value)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.FramerateCustomMaxValue != (short)_maxFramerateValue.value)
            {
                return true;
            }

            if (SettingsManager.temporarySettings.display.RunInBackground != _runInBackgroundToggle.value)
            {
                return true;
            }

            return false;
        }

        public void UpdateHiddenPanelStates(ClickEvent evt)
        {
            _hidden16X9Ratio.style.display = DisplayStyle.None;
            _hidden16X10Ratio.style.display = DisplayStyle.None;
            _hidden21X9Ratio.style.display = DisplayStyle.None;
            switch (_optionAspectRatio.index)
            {
                case 0:
                    _hidden16X9Ratio.style.display = DisplayStyle.Flex;
                    break;
                case 1:
                    _hidden16X10Ratio.style.display = DisplayStyle.Flex;
                    break;
                case 2:
                    _hidden21X9Ratio.style.display = DisplayStyle.Flex;
                    break;
            }

            _hiddenCustomResolution.style.display = DisplayStyle.None;
            if (_customResolutionToggle.value)
            {
                _hiddenCustomResolution.style.display = DisplayStyle.Flex;
            }

            _hiddenMaxFramerate.style.display = DisplayStyle.None;
            if (_maxFramerateToggle.value)
            {
                _hiddenMaxFramerate.style.display = DisplayStyle.Flex;
            }
        }
    }
}