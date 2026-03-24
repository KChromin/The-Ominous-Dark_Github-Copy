using System;
using NOS.GameManagers;
using NOS.GameManagers.Input;
using NOS.GameManagers.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NOS.GUI.Test
{
    public class Test_GameplaySettingsGUIOpen : MonoBehaviour
    {
        private GameInputs _inputs;
        private SettingsManager _settings;

        private void Awake()
        {
            _inputs = InputManager.Instance.Test_GetInputs();
            _settings = SettingsManager.Instance;
            _inputs.Test.Escape.performed += MenuTrigger;
        }

        void MenuTrigger(InputAction.CallbackContext ctx)
        {
            if (_settings.IsSettingsMenuOpen())
            {
                _settings.GUICloseSettingsMenu();
                Time.timeScale = 1;
                CursorManager.Instance.SetCursorForGameplay();
            }
            else
            {
                _settings.GUIOpenSettingsMenu();
                Time.timeScale = 0;
                CursorManager.Instance.SetCursorForUI();
            }
        }

        private void OnDisable()
        {
            _inputs.Test.Escape.performed -= MenuTrigger;
        }
    }
}