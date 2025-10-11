using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
	public class AssetProcessor :  UnityEditor.AssetModificationProcessor
	{
			public static string[] OnWillSaveAssets(string[] paths)
	    {
	        bool isSavingScene = false;
	        bool isSavingPrefab = false;
	
	        foreach (string path in paths)
	        {
                if (path.EndsWith(".unity"))
                {
                    isSavingScene = true;
                    break;
                }
                if (path.EndsWith(".prefab"))
                {
                    isSavingPrefab = true;
                }
            }
	        if (isSavingScene || isSavingPrefab)
	        {
                if (CombatEditorUtility.EditorExist())
                {
                    var editor = CombatEditorUtility.GetCurrentEditor();
                    editor.OnStopPlayAnimation();
                    editor.OnEndPreview();
                }
	        }
	
	        return paths;
	    }
	}
}
