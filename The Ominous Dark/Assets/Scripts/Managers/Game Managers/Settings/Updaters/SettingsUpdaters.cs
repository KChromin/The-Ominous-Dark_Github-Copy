using Cysharp.Threading.Tasks;

namespace NOS.GameManagers.Settings
{
    public class SettingsUpdaters
    {
        public SettingsUpdaters(SettingsContainers currentSettings)
        {
            _display = new SettingsUpdaterDisplay(currentSettings);
            _game = new SettingsUpdaterGame(currentSettings);
        }

        private readonly SettingsUpdaterAudio _audio = new();
        private readonly SettingsUpdaterControl _control = new();
        private readonly SettingsUpdaterDisplay _display;
        private readonly SettingsUpdaterGame _game;
        private readonly SettingsUpdaterVisual _visual = new();
        private readonly SettingsUpdaterAccessibility _accessibility = new();

        public void UpdateAllSettings()
        {
            _audio.UpdateSettings();
            _control.UpdateSettings();
            _display.UpdateSettings();
            _game.UpdateSettings();
            _visual.UpdateSettings();
            _accessibility.UpdateSettings();
        }

        public async UniTaskVoid UpdateAllSettingsNextFrame()
        {
            await UniTask.WaitForEndOfFrame();
            _audio.UpdateSettings();
            _control.UpdateSettings();
            _display.UpdateSettings();
            _game.UpdateSettings();
            _visual.UpdateSettings();
            _accessibility.UpdateSettings();
        }

        public void UpdateOneSettings(SettingsManager.SettingsType settingsType)
        {
            switch (settingsType)
            {
                case SettingsManager.SettingsType.Audio:
                    _audio.UpdateSettings();
                    break;
                case SettingsManager.SettingsType.Control:
                    _control.UpdateSettings();
                    break;
                case SettingsManager.SettingsType.Display:
                    _display.UpdateSettings();
                    break;
                case SettingsManager.SettingsType.Game:
                    _game.UpdateSettings();
                    break;
                case SettingsManager.SettingsType.Visual:
                    _visual.UpdateSettings();
                    break;
                case SettingsManager.SettingsType.Accessibility:
                    _accessibility.UpdateSettings();
                    break;
                default:
                    UpdateAllSettingsNextFrame().Forget();
                    break;
            }
        }
    }
}