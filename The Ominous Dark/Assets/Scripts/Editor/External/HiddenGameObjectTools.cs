using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

//https://discussions.unity.com/t/how-do-i-remove-a-game-object-that-is-not-visible-in-the-hierarchy/92281/3

namespace NOS.Editor.Tools.External
{
    public class HiddenGameObjectTools : EditorWindow
    {
        #region Menu Command

        [MenuItem("No One's Sorrow/External/Hidden GameObject Tools")]
        public static void ShowWindow()
        {
            var window = GetWindow<HiddenGameObjectTools>();
            window.titleContent = new GUIContent("Hidden GOs");
            window.GatherHiddenObjects();
        }

        #endregion

        #region GUI

        private static readonly GUILayoutOption ButtonWidth = GUILayout.Width(80);
        private static readonly GUILayoutOption BigButtonHeight = GUILayout.Height(35);

        private void OnGUI()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Refresh", BigButtonHeight))
                {
                    GatherHiddenObjects();
                }

                if (GUILayout.Button("Test", BigButtonHeight, ButtonWidth))
                {
                    var go = new GameObject("HiddenTestObject");
                    go.hideFlags = HideFlags.HideInHierarchy;
                    GatherHiddenObjects();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);

            EditorGUILayout.LabelField("Hidden Objects (" + HiddenObjects.Count + ")", EditorStyles.boldLabel);
            for (int i = 0; i < HiddenObjects.Count; i++)
            {
                var hiddenObject = HiddenObjects;
                GUILayout.BeginHorizontal();
                {
                    var gone = hiddenObject[i] == null;
                    GUILayout.Label(gone ? "null" : hiddenObject[i].name);
                    GUILayout.FlexibleSpace();
                    if (gone)
                    {
                        GUILayout.Box("Select", ButtonWidth);
                        GUILayout.Box("Reveal", ButtonWidth);
                        GUILayout.Box("Delete", ButtonWidth);
                    }
                    else
                    {
                        if (GUILayout.Button("Select", ButtonWidth))
                        {
                            Selection.activeGameObject = hiddenObject[i];
                        }

                        if (GUILayout.Button(IsHidden(hiddenObject[i]) ? "Reveal" : "Hide", ButtonWidth))
                        {
                            hiddenObject[i].hideFlags ^= HideFlags.HideInHierarchy;
                            EditorSceneManager.MarkSceneDirty(hiddenObject[i].scene);
                        }

                        if (GUILayout.Button("Delete", ButtonWidth))
                        {
                            var scene = hiddenObject[i].scene;
                            DestroyImmediate(hiddenObject[i]);
                            EditorSceneManager.MarkSceneDirty(scene);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        #endregion

        #region Hidden Objects

        private List<GameObject> HiddenObjects = new List<GameObject>();

        private void GatherHiddenObjects()

        {
            HiddenObjects.Clear();
            var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var go in allObjects)
            {
                if ((go.hideFlags & HideFlags.HideInHierarchy) != 0)
                {
                    HiddenObjects.Add(go);
                }
            }

            Repaint();
        }

        private static bool IsHidden(GameObject go)

        {
            return (go.hideFlags & HideFlags.HideInHierarchy) != 0;
        }

        #endregion
    }
}