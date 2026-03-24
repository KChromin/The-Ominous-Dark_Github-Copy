using UnityEngine.UIElements;

namespace NOS.GameManagers.Settings
{
    public interface ISettingsGUI
    {
        public SettingsManager SettingsManager { get; set; }
        public SettingsGUI SettingsGUI { get; set; }
        public VisualElement UiRoot { get; set; }

        //Set all ui elements to current setting states
        public void SetupAllSettingsUI();

        public void UpdateSettingsFromUIValues();

        public bool HasUnsavedChanges();

        public void UpdateHiddenPanelStates(ClickEvent evt);
    }
}