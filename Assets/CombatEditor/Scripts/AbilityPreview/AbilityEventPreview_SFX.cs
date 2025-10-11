using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
	public class AbilityEventPreview_SFX : AbilityEventPreview
	{
	    public AbilityEventObj_SFX Obj => (AbilityEventObj_SFX)_EventObj;
	    public AbilityEventPreview_SFX(AbilityEventObj Obj):base(Obj)
	    {
	        _EventObj = Obj;
	    }
	
#if UNITY_EDITOR
	    public override void PassStartFrame()
	    {
	        if (CombatGlobalEditorValue.IsPlaying || CombatGlobalEditorValue.IsLooping)
	        {
	            if (Obj.clips.Count > 0)
	            {
	                var randomClip = Obj.clips[UnityEngine.Random.Range(0, Obj.clips.Count)];
	                if (randomClip != null)
	                {
	                    EditorSFX.PlayClip(randomClip);
	                }
	            }
	        }
	    }
#endif
	}
	
#if UNITY_EDITOR
	public static class EditorSFX
	{
	
	    public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
	    {
	        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
	
	        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
	        MethodInfo method = audioUtilClass.GetMethod(
	            "PlayPreviewClip",
	            BindingFlags.Static | BindingFlags.Public,
	            null,
	            new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
	            null
	        );
	
	        method.Invoke(
	            null,
	            new object[] { clip, startSample, loop }
	        );
	    }
	
	    public static void StopAllClips()
	    {
	        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
	
	        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
	        MethodInfo method = audioUtilClass.GetMethod(
	            "StopAllPreviewClips",
	            BindingFlags.Static | BindingFlags.Public,
	            null,
	            new Type[] { },
	            null
	        );
	
	        Debug.Log(method);
	        method.Invoke(
	            null,
	            new object[] { }
	        );
	    }
	}
#endif
}
