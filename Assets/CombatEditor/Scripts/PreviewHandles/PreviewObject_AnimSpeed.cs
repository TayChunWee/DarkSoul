using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{
    
	public class PreviewObject_AnimSpeed : PreviewerOnObject
	{
#if UNITY_EDITOR
	    public float CurrentAnimSpeedModifier;
#endif
	    //public override void UpdateHandle()
	    //{
	    //    CurrentAnimSpeedModifier = ((AbilityEventPreview_AnimSpeed)_preview).Obj.Speed;
	    //}
	}
}
