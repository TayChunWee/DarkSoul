using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
	[CustomEditor(typeof(CombatController))]
	public class CombatControllerEditor : Editor
	{
	    public override void OnInspectorGUI()
	    {
	        if (GUILayout.Button("Open CombatEditor", GUILayout.Height(35)))
	        {
                CombatEditor.Init();
	        }
	    }
	}
}
