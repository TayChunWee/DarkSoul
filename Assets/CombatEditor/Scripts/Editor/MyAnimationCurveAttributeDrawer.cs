using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace CombatEditor
{	
	//public class AnimationCurveAttributeDrawer : OdinAttributeDrawer<AnimationCurveAttribute, AnimationCurve>
	public class MyAnimationCurveDrawer:PropertyDrawer
	{
	    SerializedProperty Curve;
	    SerializedProperty Scale;
	    public float CurveHeight = 30;
	    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	    {
	        //return base.GetPropertyHeight(property, label);
	        Rect ControlRect = EditorGUILayout.GetControlRect();
	        return CurveHeight - 18;
	    }
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	    {
	        //base.OnGUI(position, property, label);
	        EditorGUI.BeginProperty(position, label, property);
	        //����ʼ
	        Curve = property.FindPropertyRelative("curve");
	        Scale = property.FindPropertyRelative("Scale");
	
	        Rect newPos = new Rect(position.x, position.y - 18, position.width, position.height + 18);
	     
	        Curve.animationCurveValue = EditorGUI.CurveField( newPos, GUIContent.none, Curve.animationCurveValue, Color.green, new Rect(0, -1, 1, 2));
	        //Scale.floatValue = EditorGUI.FloatField(position, Scale.floatValue);
	
	            EditorGUI.DrawRect(new Rect(newPos.position + new Vector2(newPos.width * CombatGlobalEditorValue.Percentage, 0), new Vector2(1, newPos.height)), Color.white);
	        //����־�㡣
	        EditorGUI.EndProperty();
	    }
	    
	}
}
