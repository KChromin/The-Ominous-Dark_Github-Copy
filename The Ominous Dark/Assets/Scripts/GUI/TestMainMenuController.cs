using NOS.GameManagers.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace NOS.GUI.MainMenu
{
    public class TestMainMenuController : MonoBehaviour
    {
        private SettingsManager _settingsManager;
        
        private UIDocument _document;
        private VisualElement _root;

        #region UI Buttons

        private Button _startButton;
        private Button _settingsButton;
        private Button _exitButton;

        #endregion UI Buttons

        private void Awake()
        {
            _settingsManager = SettingsManager.Instance;
            
            _document = GetComponent<UIDocument>();
            _root = _document.rootVisualElement;
            _startButton = _root.Q<Button>("StartButton");
            _settingsButton = _root.Q<Button>("SettingsButton");
            _exitButton = _root.Q<Button>("ExitButton");

            _startButton.RegisterCallback<ClickEvent>(StartEvent);
            _settingsButton.RegisterCallback<ClickEvent>(SettingsEvent);
            _exitButton.RegisterCallback<ClickEvent>(ExitEvent);
        }

        private void StartEvent(ClickEvent evt)
        {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        }

        private void SettingsEvent(ClickEvent evt)
        {
            _settingsManager.GUIOpenSettingsMenu();
        }


        private void ExitEvent(ClickEvent evt)
        {
            Application.Quit();
        }
    }
}