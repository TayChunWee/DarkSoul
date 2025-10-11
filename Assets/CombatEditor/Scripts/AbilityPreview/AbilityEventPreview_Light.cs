using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{
    //If you need to create object with handle, you can just inherit the AbilityEventPreview_CreateObjWithHandle
#if UNITY_EDITOR
    public partial class AbilityEventPreview_Light : AbilityEventPreview_CreateObjWithHandle
	{
	    Light light;
	    public override void InitPreview()
	    {
	        base.InitPreview();
	        if (InstantiatedObj != null)
	        {
	            light = InstantiatedObj.GetComponent<Light>();
	        }
	    }
	    //Update Preview when drag the timeline or timeline is playing.
	    public override void PreviewRunning(float CurrentTimePercentage)
	    {
	        base.PreviewRunning(CurrentTimePercentage);
	        if(light!=null)
	        {
	            if (PreviewInRange(CurrentTimePercentage))
	            {
	                light.intensity = Obj.curve.GetCurveValue(StartTimePercentage, EndTimePercentage, CurrentTimePercentage);
	            }
	            else
	            {
	                light.intensity = 0;
	            }
	        }
	    }
	}
	
	public partial class AbilityEventPreview_Light : AbilityEventPreview_CreateObjWithHandle
	{
	    public AbilityEventObj_Light Obj => (AbilityEventObj_Light)_EventObj;
	    public AbilityEventPreview_Light(AbilityEventObj Obj) : base(Obj)
	    {
	        _EventObj = Obj;
	    }
	}
#endif
}
