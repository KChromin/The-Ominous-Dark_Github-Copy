using System;
using UnityEngine;
using UnityEditor;

namespace NOS.Editor.Tools.Create
{
    public class VisualsPerObjectDirectoryCreator : EditorWindow
    {
        //Path to objects directory//
        private const string PathToObjects = "Assets/Visuals/_PerObject";

        private string _pathToNewDirectory = "Assets/Visuals/_PerObject";

        private string _objectName = "Name";

        private bool _supplementWithSubdirectories;

        #region Directories to create

        //Directories to create//
        private bool _createDirectoryMaterials;
        private bool _createDirectoryTextures;
        private bool _createDirectorySprites;
        private bool _createDirectoryAnimations;
        private bool _createDirectoryModels;
        private bool _createDirectoryVFX;
        private bool _createDirectoryShaders;

        #endregion Directories to create

        [MenuItem("No One's Sorrow/Create/New Visual Per Object Directories Creator")]
        private static void Init()
        {
            VisualsPerObjectDirectoryCreator window = (VisualsPerObjectDirectoryCreator)GetWindow(typeof(VisualsPerObjectDirectoryCreator));
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Visual Object Directory Creator", EditorStyles.largeLabel);

            //Choose name//
            GUILayout.Label("New Object Name", EditorStyles.label);
            _objectName = EditorGUILayout.TextField("Object Name", _objectName);

            GUILayout.Space(10);

            //If you want only to add subdirectory to existing parent directory//
            GUILayout.Label("Supplement object directory with subdirectories ( When directory already exists, and you need some more subdirectories )", EditorStyles.label);
            _supplementWithSubdirectories = EditorGUILayout.Toggle("Supplement subdirectories", _supplementWithSubdirectories);

            GUILayout.Space(10);

            //Choose path//
            GUILayout.Label("Current Location", EditorStyles.boldLabel);
            GUILayout.Label(_pathToNewDirectory, EditorStyles.label);
            if (GUILayout.Button("Choose Directory Location"))
            {
                _pathToNewDirectory = EditorUtility.OpenFolderPanel("Choose parent directory", _pathToNewDirectory, "");

                //Get only local project path
                _pathToNewDirectory = _pathToNewDirectory.Substring(_pathToNewDirectory.IndexOf("Assets", StringComparison.Ordinal));

                //If it misses then is clicked//
                if (_pathToNewDirectory == "")
                {
                    _pathToNewDirectory = PathToObjects;
                }
            }

            GUILayout.Space(10);

            //Choose directories to create//
            GUILayout.Label("Subdirectories To Create", EditorStyles.boldLabel);
            _createDirectoryMaterials = EditorGUILayout.Toggle("Materials", _createDirectoryMaterials);
            _createDirectoryTextures = EditorGUILayout.Toggle("Textures", _createDirectoryTextures);
            _createDirectorySprites = EditorGUILayout.Toggle("Sprites", _createDirectorySprites);
            _createDirectoryAnimations = EditorGUILayout.Toggle("Animations", _createDirectoryAnimations);
            _createDirectoryModels = EditorGUILayout.Toggle("Models", _createDirectoryModels);
            _createDirectoryVFX = EditorGUILayout.Toggle("VFX", _createDirectoryVFX);
            _createDirectoryShaders = EditorGUILayout.Toggle("Shaders", _createDirectoryShaders);

            GUILayout.Space(10);

            //Submit//
            if (GUILayout.Button("Create Directories"))
            {
                ExecuteDirectoriesCreation();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ExecuteDirectoriesCreation()
        {
            if (!CheckIfDirectoryAlreadyExists(_pathToNewDirectory + "/" + _objectName))
            {
                //Create root folder//
                CreateNewDirectory(_pathToNewDirectory, _objectName);
            }
            else if (!_supplementWithSubdirectories)
            {
                return;
            }

            string objectPath = _pathToNewDirectory + "/" + _objectName;

            CreateSubdirectories(objectPath);

            Debug.Log("Creation Executed!");
        }

        // ReSharper disable once CognitiveComplexity
        private void CreateSubdirectories(string path)
        {
            //Create all subdirectories//
            if (_createDirectoryMaterials)
            {
                if (!CheckIfDirectoryAlreadyExists(path + "/" + "Materials"))
                {
                    CreateNewDirectory(path, "Materials");
                }
            }

            if (_createDirectoryTextures)
            {
                if (!CheckIfDirectoryAlreadyExists(path + "/" + "Textures"))
                {
                    CreateNewDirectory(path, "Textures");
                }
            }

            if (_createDirectorySprites)
            {
                if (!CheckIfDirectoryAlreadyExists(path + "/" + "Sprites"))
                {
                    CreateNewDirectory(path, "Sprites");
                }
            }

            if (_createDirectoryAnimations)
            {
                if (!CheckIfDirectoryAlreadyExists(path + "/" + "Animations"))
                {
                    CreateNewDirectory(path, "Animations");
                }
            }

            if (_createDirectoryModels)
            {
                if (!CheckIfDirectoryAlreadyExists(path + "/" + "Models"))
                {
                    CreateNewDirectory(path, "Models");
                }
            }

            if (_createDirectoryVFX)
            {
                if (!CheckIfDirectoryAlreadyExists(path + "/" + "VFX"))
                {
                    CreateNewDirectory(path, "VFX");
                }
            }

            if (_createDirectoryShaders)
            {
                if (!CheckIfDirectoryAlreadyExists(path + "/" + "Shaders"))
                {
                    CreateNewDirectory(path, "Shaders");
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static bool CheckIfDirectoryAlreadyExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path)) return false;

            Debug.LogError("Directory already exists! - " + path);
            return true;
        }

        private static void CreateNewDirectory(string path, string newDirectoryName)
        {
            AssetDatabase.CreateFolder(path, newDirectoryName);
        }
    }
}