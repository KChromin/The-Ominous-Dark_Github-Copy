using NOS.GameManagers.Settings;
using NOS.Patterns.Singleton;
using UnityEngine;

namespace NOS.GameManagers
{
    [DefaultExecutionOrder(-88)]
    public class CursorManager : SingletonPersistent<CursorManager>
    {
        public void SetCursorForUI()
        {
            SetVisibility(true);
            SetLockState(false);
        }

        public void SetCursorForGameplay()
        {
            SetVisibility(false);
            SetLockState(true);
        }

        #region Private Methodes

        private void SetVisibility(bool newState)
        {
            Cursor.visible = newState;
        }

        private void SetLockState(bool newState)
        {
            if (newState) //Locked
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else //Unlocked
            {
                Cursor.lockState = SettingsManager.Instance.CurrentSettings.accessibility.cursorLockToWindow ? CursorLockMode.Confined : CursorLockMode.None;
            }
        }

        #endregion Private Methodes
    }
}