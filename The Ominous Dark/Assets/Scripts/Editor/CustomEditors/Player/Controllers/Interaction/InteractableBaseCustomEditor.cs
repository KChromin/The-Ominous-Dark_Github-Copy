using NOS.Controllers.Interactions;
using NOS.CustomEditors;
using UnityEditor;

namespace NOS.Controllers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableBase))]
    public class InteractableBaseCustomEditor : Editor
    {
        private InteractableBase script;
        private bool enableDebug;

        #region Serialized Properties

        private SerializedProperty _canBeUsed;
        private SerializedProperty _objectName;
        private SerializedProperty _interactionName;
        private SerializedProperty _interactionMode;
        private SerializedProperty _onHoldTimeToUseInSeconds;
        private SerializedProperty _onHoldProgressDegradesWhileNotHolding;
        private SerializedProperty _onHoldProgressDegradationEverySecond;

        #endregion Serialized Properties

        private void OnEnable()
        {
            script = (InteractableBase)target;

            _canBeUsed = serializedObject.FindAutoProperty("CanBeUsed");
            _objectName = serializedObject.FindAutoProperty("ObjectName");
            _interactionName = serializedObject.FindAutoProperty("InteractionName");
            _interactionMode = serializedObject.FindAutoProperty("InteractionMode");
            _onHoldTimeToUseInSeconds = serializedObject.FindProperty("onHoldTimeToUseInSeconds");
            _onHoldProgressDegradesWhileNotHolding = serializedObject.FindProperty("onHoldProgressDegradesWhileNotHolding");
            _onHoldProgressDegradationEverySecond = serializedObject.FindProperty("onHoldProgressDegradationEverySecond");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            _canBeUsed.boolValue = EditorGUILayout.Toggle("Can Be Used", _canBeUsed.boolValue);
            EditorGUILayout.Space(2);
            _objectName.stringValue = EditorGUILayout.TextField("Object Name", _objectName.stringValue);
            EditorGUILayout.Space(2);
            _interactionName.enumValueIndex = (int)(InteractionActionName)EditorGUILayout.EnumPopup("Interaction Name", (InteractionActionName)_interactionName.enumValueIndex);
            EditorGUILayout.Space(15);
            _interactionMode.enumValueIndex = (int)(InteractionModes)EditorGUILayout.EnumPopup("Interaction Mode", (InteractionModes)_interactionMode.enumValueIndex);
            EditorGUILayout.Space(2);

            //On Click Options
            if (_interactionMode.enumValueIndex == (int)InteractionModes.OnClick)
            {
            }

            //On Hold Options
            if (_interactionMode.enumValueIndex == (int)InteractionModes.OnHold)
            {
                _onHoldTimeToUseInSeconds.floatValue = EditorGUILayout.FloatField("Time To Use In Seconds", _onHoldTimeToUseInSeconds.floatValue);

                EditorGUILayout.Space(4);

                //Progress Degradation
                _onHoldProgressDegradesWhileNotHolding.boolValue = EditorGUILayout.Toggle("Progress Degradation Without Holding", _onHoldProgressDegradesWhileNotHolding.boolValue);
                if (script.onHoldProgressDegradesWhileNotHolding)
                {
                    _onHoldProgressDegradationEverySecond.floatValue = EditorGUILayout.FloatField("Degradation Value Every Second", _onHoldProgressDegradationEverySecond.floatValue);
                }

                #region Debug

                EditorGUILayout.Space(20);
                EditorGUILayout.LabelField("Debug");
                enableDebug = EditorGUILayout.Toggle("Enable Debug", enableDebug);

                if (enableDebug)
                {
                    EditorGUILayout.Toggle("Is Interacting", script.IsInteracting);
                    EditorGUILayout.FloatField("Interacting Time", script.InteractingTime);
                    Repaint();
                }

                #endregion Debug
            }

            EditorGUI.BeginChangeCheck();
            serializedObject.ApplyModifiedProperties();
        }
    }
}