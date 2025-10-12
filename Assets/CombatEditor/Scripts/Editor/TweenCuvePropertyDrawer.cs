using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

 namespace CombatEditor
{
    [CustomPropertyDrawer(typeof(TweenCurve))]
	public class TweenCuvePropertyDrawer :PropertyDrawer
	{
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	    {
	        var curveProperty = property.FindPropertyRelative("curve");
	        var startValueProperty = property.FindPropertyRelative("StartValue");
	        var endValueProperty = property.FindPropertyRelative("EndValue");
	
	        var ValueWidth = 50f;
	        Rect curveRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * 2 );
	        Rect startValueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2 + 5, ValueWidth, EditorGUIUtility.singleLineHeight);
	        Rect endValueRect = new Rect(position.x + position.width - ValueWidth, position.y + EditorGUIUtility.singleLineHeight * 2 + 5, ValueWidth, EditorGUIUtility.singleLineHeight);
	
	        //GUILayout.BeginVertical("Curve", "window");
	        EditorGUI.CurveField(curveRect, curveProperty , Color.green, new Rect(0,0,1,1),new GUIContent(""));
	
	        EditorGUI.PropertyField(startValueRect, startValueProperty , new GUIContent(""));
	        EditorGUI.PropertyField(endValueRect, endValueProperty, new GUIContent(""));
	
	        //GUILayout.EndVertical();
	
	
	    }
	    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	    {
	        return EditorGUIUtility.singleLineHeight * 3;
	    }
	
	}
}
