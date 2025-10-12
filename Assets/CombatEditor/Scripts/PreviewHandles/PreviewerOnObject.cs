using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif

 namespace CombatEditor
{
    public class PreviewerOnObject : MonoBehaviour
	{
#if UNITY_EDITOR
	    public CombatController _combatController;
	    public AbilityEventPreview _preview;
	 
	    public virtual void SelfDestroy()
	    {
	        SceneView.duringSceneGui -= OnSceneGUI;
	    }
	    public virtual void Init()
	    {
	        SceneView.duringSceneGui += OnSceneGUI;
	    }
	
	    /// <summary>
	    /// 1.UpdateHandle
	    
	    /// </summary>
	    /// <param name="sceneView"></param>
	    public virtual void OnSceneGUI(SceneView sceneView)
	    {
	        if (_preview == null || _combatController == null || this == null)
	        {
	            SceneView.duringSceneGui -= OnSceneGUI;
	            return;
	        }
	        if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
	        {
	            return;
	        }
	        if(!_preview.eve.Previewable)
	        {
	            UpdateHiddenHandle();
	            return;
	        }
	
	        PaintHandle();
	    }
	   
	    /// <summary>
	    /// Called Outside
	    /// </summary>
	    public virtual void UpdateTransformData()
	    {
	
	    }
	
	    public virtual void PaintHandle()
	    {
	
	    }
	    
	    public virtual void UpdateHiddenHandle()
	    {
	
	    }
	
	
#endif
	}
}
