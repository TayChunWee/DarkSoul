using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyPropertyDrawer : PropertyDrawer
	{
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	    {
	        GUI.enabled = false;
	        EditorGUI.PropertyField(position, property, label);
	        GUI.enabled = true;
	    }
	}
}
