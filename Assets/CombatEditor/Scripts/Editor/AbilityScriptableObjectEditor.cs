using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{
    [CustomEditor(typeof(AbilityScriptableObject))]
	public class AbilityScriptableObjectEditor : Editor
	{
	    public float LabelWidth = 140;
	    public override void OnInspectorGUI()
	    {
	        base.OnInspectorGUI();
	    }
	}
	
	
}
