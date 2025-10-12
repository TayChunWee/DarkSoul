using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CombatEditor
{
    [CustomEditor(typeof(AbilityEventObj_States))]
    public class AbilityEventObj_StatesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();

            var CountSp = serializedObject.FindProperty("DivideCount");
            var StatesSp = serializedObject.FindProperty("States");
            EditorGUI.BeginChangeCheck();
            if(EditorGUILayout.PropertyField(CountSp))
            {
             
            }
            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
                var obj = target as AbilityEventObj_States;
                CombatEditorUtility.GetCurrentEditor().OnResetMultiStatesCount(obj);
            }


            EditorGUI.BeginChangeCheck();
            for (int i = 0;i<CountSp.intValue; i++)
            {
                var StateNameSp = StatesSp.GetArrayElementAtIndex(i);
                StateNameSp.stringValue = EditorGUILayout.TextField(new GUIContent("State" + i), StateNameSp.stringValue);
            }
            if(serializedObject.hasModifiedProperties)
            {
                CombatEditorUtility.GetCurrentEditor().Repaint();
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.HelpBox("Please use CombatController.IsInState(string StateName) to check the current state.",MessageType.Info);
        }
    }
}
