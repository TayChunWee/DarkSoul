using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	public class AbilityEventPreview_AnimSpeed : AbilityEventPreview
	{
	    public AbilityEventObj_AnimSpeed Obj => (AbilityEventObj_AnimSpeed)_EventObj;
	    public PreviewObject_AnimSpeed _speed;
	    public AbilityEventPreview_AnimSpeed(AbilityEventObj Obj) : base(Obj)
	    {
	        _EventObj = Obj;
	    }
	
#if UNITY_EDITOR
	    public override void InitPreview()
	    {
	        base.InitPreview();
	
	        var SpeedObj = new GameObject("Preview_AnimSpeed");
	        SpeedObj.transform.SetParent(previewGroup.transform);
	        _speed = SpeedObj.AddComponent<PreviewObject_AnimSpeed>();
	        _speed._preview = this;
	        _speed.CurrentAnimSpeedModifier = Obj.Speed;
	        _speed.transform.SetParent(previewGroup.transform);
	        
	    }
	    public override void PreviewRunning(float CurrentTimePercentage)
	    {
	        base.PreviewRunning(CurrentTimePercentage);
	
	        if (PreviewInRange(CurrentTimePercentage))
	        {
	            _speed.CurrentAnimSpeedModifier = Obj.Speed;
	        }
	        else
	        {
	            _speed.CurrentAnimSpeedModifier = 1;
	        }
	
	    }
#endif
	}
}
