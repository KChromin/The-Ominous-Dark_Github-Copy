using System;
using UnityEngine;

namespace NOS.GameManagers.Settings
{
    [Serializable]
    public class SettingsContainers : ICloneable
    {
        public SettingsAudioContainer audio = new();
        [Space]
        public SettingsControlContainer control = new();
        [Space]
        public SettingsDisplayContainer display = new();
        [Space]
        public SettingsGameContainer game = new();
        [Space]
        public SettingsVisualContainer visual = new();
        [Space]
        public SettingsAccessibilityContainer accessibility = new();

        public object Clone()
        {
            SettingsContainers copiedContainer = new()
            {
                audio = (SettingsAudioContainer)audio.Clone(),
                control = (SettingsControlContainer)control.Clone(),
                display = (SettingsDisplayContainer)display.Clone(),
                game = (SettingsGameContainer)game.Clone(),
                visual = (SettingsVisualContainer)visual.Clone(),
                accessibility = (SettingsAccessibilityContainer)accessibility.Clone()
            };

            return copiedContainer;
        }
    }
}