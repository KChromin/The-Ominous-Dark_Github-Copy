using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings")]
    public class SettingsScriptableObject : ScriptableObject
    {
        public SettingsContainers settings;
    }
}