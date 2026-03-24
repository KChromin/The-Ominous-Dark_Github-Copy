using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

namespace NOS.GameManagers.Settings
{
    public class SettingsManagerFileHandler
    {
        public SettingsManagerFileHandler(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;

            _directoryPath = Application.persistentDataPath + "/" + "Settings";
            _filePath = _directoryPath + "/Settings.txt";
        }

        #region Variables

        //File path//
        private readonly string _directoryPath;
        private readonly string _filePath;

        //Current settings
        private SettingsManager _settingsManager;

        //Comments
        private const int LenghtBeforeComment = 48;

        //Which settings need restoring//
        public Action<bool[]> OnCorruptedSettingsCheck;

        //Array for settings loading//
        private readonly bool[] _loadFileCheckThatSettingsAreLoaded = new bool[6];

        //Array for resetting all settings//
        private static readonly bool[] ResetAllSettingsArray = { false, false, false, false, false, false };

        #endregion Variables

        private void AssureExistenceOfSettingsDirectory()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        #region Load Methodes

        public void LoadFile()
        {
            AssureExistenceOfSettingsDirectory();

            string loadedSettings = GetLoadedSettingsFile();
            List<string> settingsTypes = new();

            //Split between every type of settings//
            foreach (Match match in Regex.Matches(loadedSettings, @"\{[^}]*\}"))
            {
                settingsTypes.Add(match.Value);
            }

            //When cannot load all, reset to default and save //
            if (settingsTypes.Count != 6)
            {
                Debug.LogError("Cannot load All 6 Settings | Regeneration!");

                //Regenerate All Settings
                OnCorruptedSettingsCheck?.Invoke(ResetAllSettingsArray);
                return;
            }

            //Reset array//
            for (int i = 0; i < _loadFileCheckThatSettingsAreLoaded.Length; i++)
            {
                _loadFileCheckThatSettingsAreLoaded[i] = false;
            }

            //Try to assign loaded values

            //Audio
            _settingsManager.CurrentSettings.audio = (SettingsAudioContainer)TryToLoadSetting(settingsTypes[0], SettingsManager.SettingsType.Audio, out _loadFileCheckThatSettingsAreLoaded[0]);

            //Control
            _settingsManager.CurrentSettings.control = (SettingsControlContainer)TryToLoadSetting(settingsTypes[1], SettingsManager.SettingsType.Controls, out _loadFileCheckThatSettingsAreLoaded[1]);

            //Display
            _settingsManager.CurrentSettings.display = (SettingsDisplayContainer)TryToLoadSetting(settingsTypes[2], SettingsManager.SettingsType.Display, out _loadFileCheckThatSettingsAreLoaded[2]);

            //Game
            _settingsManager.CurrentSettings.game = (SettingsGameContainer)TryToLoadSetting(settingsTypes[3], SettingsManager.SettingsType.Game, out _loadFileCheckThatSettingsAreLoaded[3]);

            //Visual
            _settingsManager.CurrentSettings.visual = (SettingsVisualContainer)TryToLoadSetting(settingsTypes[4], SettingsManager.SettingsType.Visuals, out _loadFileCheckThatSettingsAreLoaded[4]);

            //Accessibility
            _settingsManager.CurrentSettings.accessibility = (SettingsAccessibilityContainer)TryToLoadSetting(settingsTypes[5], SettingsManager.SettingsType.Accessibility, out _loadFileCheckThatSettingsAreLoaded[5]);

            //Regenerate settings if needed
            OnCorruptedSettingsCheck?.Invoke(_loadFileCheckThatSettingsAreLoaded);
        }

        private static object TryToLoadSetting(string data, SettingsManager.SettingsType settingsType, out bool isSettingLoaded)
        {
            try
            {
                isSettingLoaded = true;
                return LoadedDataAsObject(data, settingsType);
            }
            catch
            {
                switch (settingsType)
                {
                    case SettingsManager.SettingsType.Audio:
                        Debug.LogError("Cannot load Audio Settings | Regeneration!");
                        break;
                    case SettingsManager.SettingsType.Controls:
                        Debug.LogError("Cannot load Control Settings | Regeneration!");
                        break;
                    case SettingsManager.SettingsType.Display:
                        Debug.LogError("Cannot load Display Settings | Regeneration!");
                        break;
                    case SettingsManager.SettingsType.Game:
                        Debug.LogError("Cannot load Game Settings | Regeneration!");
                        break;
                    case SettingsManager.SettingsType.Visuals:
                        Debug.LogError("Cannot load Visual Settings | Regeneration!");
                        break;
                    case SettingsManager.SettingsType.Accessibility:
                        Debug.LogError("Cannot load Accessibility Settings | Regeneration!");
                        break;
                    default:
                        Debug.LogError("Something Weird Happened!");
                        break;
                }

                isSettingLoaded = false;
                return null;
            }
        }

        private string GetLoadedSettingsFile()
        {
            string loadedData = "";

            try
            {
                loadedData = File.ReadAllText(_filePath);
            }
            catch
            {
                Debug.LogError("File doesn't exist! | Regenerating Settings File!");
                OnCorruptedSettingsCheck.Invoke(ResetAllSettingsArray);
            }

            return loadedData;
        }

        #endregion Load Methodes

        #region Save Methodes

        public void SaveFile()
        {
            AssureExistenceOfSettingsDirectory();
            File.WriteAllText(_filePath, GetAllDataToSave());
        }
        
        private string GetAllDataToSave()
        {
            string dataToSave = "[Settings Configuration] \n\n";

            dataToSave += DataToSave(_settingsManager.CurrentSettings.audio, SettingsManager.SettingsType.Audio) + "\n";
            dataToSave += DataToSave(_settingsManager.CurrentSettings.control, SettingsManager.SettingsType.Controls) + "\n";
            dataToSave += DataToSave(_settingsManager.CurrentSettings.display, SettingsManager.SettingsType.Display) + "\n";
            dataToSave += DataToSave(_settingsManager.CurrentSettings.game, SettingsManager.SettingsType.Game) + "\n";
            dataToSave += DataToSave(_settingsManager.CurrentSettings.visual, SettingsManager.SettingsType.Visuals) + "\n";
            dataToSave += DataToSave(_settingsManager.CurrentSettings.accessibility, SettingsManager.SettingsType.Accessibility) + "\n";

            return dataToSave;
        }

        #endregion Save Methodes

        #region Data Parser

        // ReSharper disable once CognitiveComplexity
        private static string DataToSave(object classToSave, SettingsManager.SettingsType settingsType)
        {
            #region Header &  Description

            string header;
            string[] description;

            switch (settingsType)
            {
                default:
                case SettingsManager.SettingsType.Audio:
                    header = SettingsDescriptions.SettingsHeaders[0];
                    description = SettingsDescriptions.AudioSettingsDescriptions;
                    break;
                case SettingsManager.SettingsType.Controls:
                    header = SettingsDescriptions.SettingsHeaders[1];
                    description = SettingsDescriptions.ControlSettingsDescriptions;
                    break;
                case SettingsManager.SettingsType.Display:
                    header = SettingsDescriptions.SettingsHeaders[2];
                    description = SettingsDescriptions.DisplaySettingsDescriptions;
                    break;
                case SettingsManager.SettingsType.Game:
                    header = SettingsDescriptions.SettingsHeaders[3];
                    description = SettingsDescriptions.GameSettingsDescriptions;
                    break;
                case SettingsManager.SettingsType.Visuals:
                    header = SettingsDescriptions.SettingsHeaders[4];
                    description = SettingsDescriptions.VisualSettingsDescriptions;
                    break;
                case SettingsManager.SettingsType.Accessibility:
                    header = SettingsDescriptions.SettingsHeaders[5];
                    description = SettingsDescriptions.AccessibilitySettingsDescriptions;
                    break;
            }

            #endregion Header &  Description

            //Create string, and add Header//
            string dataToSave = header + "\n";

            //Convert Class to Json, and slice it by line//
            string classData = JsonConvert.SerializeObject(classToSave, Formatting.Indented);

            string[] classDataInLines = classData.Split(Environment.NewLine);

            for (int i = 0; i < classDataInLines.Length; i++)
            {
                dataToSave += classDataInLines[i];

                //Check if it should add comment - Should not add anything if its { or }//
                if (i == 0 || i == classDataInLines.Length - 1)
                {
                    dataToSave += "\n";
                    continue;
                }

                //Add comment//
                string spacesForComments = "";

                for (int j = 0; j < LenghtBeforeComment - classDataInLines[i].Length; j++)
                {
                    spacesForComments += " ";
                }

                dataToSave += spacesForComments + "/// " + description[i - 1] + "\n";
            }

            return dataToSave;
        }

        private static object LoadedDataAsObject(string loadedData, SettingsManager.SettingsType settingsType)
        {
            string finalJson = "";

            #region Delete comments

            string[] settingsLines = loadedData.Split("\n");

            //Delete comments//
            foreach (string line in settingsLines)
            {
                if (line.Contains("///"))
                {
                    int index = line.IndexOf("///", StringComparison.Ordinal);

                    if (index != -1)
                    {
                        finalJson += line.Substring(0, index - 1);
                    }
                }
                else
                {
                    finalJson += line;
                }
            }

            #endregion Delete comments

            return ConvertJsonToClass(finalJson, settingsType);
        }

        private static object ConvertJsonToClass(string json, SettingsManager.SettingsType settingsType)
        {
            object classFromJson;

            switch (settingsType)
            {
                default:
                case SettingsManager.SettingsType.Audio:
                    classFromJson = JsonConvert.DeserializeObject<SettingsAudioContainer>(json);
                    break;
                case SettingsManager.SettingsType.Controls:
                    classFromJson = JsonConvert.DeserializeObject<SettingsControlContainer>(json);
                    break;
                case SettingsManager.SettingsType.Display:
                    classFromJson = JsonConvert.DeserializeObject<SettingsDisplayContainer>(json);
                    break;
                case SettingsManager.SettingsType.Game:
                    classFromJson = JsonConvert.DeserializeObject<SettingsGameContainer>(json);
                    break;
                case SettingsManager.SettingsType.Visuals:
                    classFromJson = JsonConvert.DeserializeObject<SettingsVisualContainer>(json);
                    break;
                case SettingsManager.SettingsType.Accessibility:
                    classFromJson = JsonConvert.DeserializeObject<SettingsAccessibilityContainer>(json);
                    break;
            }

            return classFromJson;
        }

        #endregion Data Parser
    }
}