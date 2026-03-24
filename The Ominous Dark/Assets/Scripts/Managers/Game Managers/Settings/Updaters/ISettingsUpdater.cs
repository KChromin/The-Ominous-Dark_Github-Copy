namespace NOS.GameManagers.Settings
{
    public interface ISettingsUpdater
    {
        public SettingsManager Settings { get; set; }
        public void UpdateSettings();
    }
}