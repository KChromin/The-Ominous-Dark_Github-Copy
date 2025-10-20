namespace NOS.GameManagers.Settings
{
    public class SettingsAutoConfigurations
    {
        public SettingsAutoConfigurations(SettingsContainers currentSettings)
        {
            _display = new SettingsAutoConfigurationDisplay(currentSettings);
            _visual = new SettingsAutoConfigurationVisual(currentSettings);
        }

        private readonly SettingsAutoConfigurationDisplay _display;
        private readonly SettingsAutoConfigurationVisual _visual;

        public enum SettingsType
        {
            Display,
            Visual
        }

        public void AutoConfigureSettings(SettingsType settings)
        {
            switch (settings)
            {
                default:
                case SettingsType.Display:
                    _display.SetAutoConfigurationSettings();
                    break;
                case SettingsType.Visual:
                    _visual.SetAutoConfigurationSettings();
                    break;
            }
        }
    }
}