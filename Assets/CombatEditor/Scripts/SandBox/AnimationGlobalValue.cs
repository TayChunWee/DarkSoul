using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor {	
	//Used to communicate between previews
	public class CombatGlobalEditorValue
	{
	    public static float Percentage;
	    public static Vector3 CurrentMotionTAtGround;
	    public static bool IsPlaying;
	    public static bool IsLooping;
	    public static string PreviewGroupName = "---PreviewGroup---";
	    public static Vector3 CharacterRootCenterAtCurrentFrame;
	
	    public static Vector3 CharacterTransPosBeforePreview;
	
	    public static Vector3 CurrentRootMotionOffset;
	
	}
}
