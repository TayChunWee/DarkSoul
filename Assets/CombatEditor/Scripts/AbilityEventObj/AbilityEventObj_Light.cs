using UnityEngine;

 namespace CombatEditor {	
    //Replace the "Light" with the event you want to create
	//If you want to create a object with handle in preview, please inherit the "AbilityEventObj" with "AbilityEventObj_CreateObjWithHandle"
	[AbilityEvent]
	[CreateAssetMenu(menuName = "AbilityEvents / Light")]
	public class AbilityEventObj_Light : AbilityEventObj_CreateObjWithHandle
	{
	    public TweenCurve curve;
	    
	    //Write the data you need here.
	    public override EventTimeType GetEventTimeType()
	    {
	        return EventTimeType.EventRange;
	    }
	    public override AbilityEventEffect Initialize()
	    {
	        return new AbilityEventEffect_Light(this);
	    }
#if UNITY_EDITOR
	    public override AbilityEventPreview InitializePreview()
	    {
	        return new AbilityEventPreview_Light(this);
	    }
#endif
	}
	//Write you logic here
	public partial class AbilityEventEffect_Light : AbilityEventEffect_CreateObjWithHandle
	{
	    public Light Light;
	    public override void StartEffect()
	    {
	        base.StartEffect();
	        Light = InstantiatedObj.GetComponent<Light>();
	    }
	    public override void EffectRunning( float currentTime)
	    {
	        base.EffectRunning();
	        if (Light != null)
	        {
	            Light.intensity = EventObj.curve.GetCurveValue(eve.GetEventStartTime(), eve.GetEventEndTime(), currentTime);
	        }
	    }
	    public override void EndEffect()
	    {
	        base.EndEffect();
	        if (Light != null)
	        {
	            Object.Destroy(Light.gameObject);
	        }
	    }
	}
	
	public partial class AbilityEventEffect_Light : AbilityEventEffect_CreateObjWithHandle
	{
	    AbilityEventObj_Light EventObj => (AbilityEventObj_Light)_EventObj;
	    public AbilityEventEffect_Light(AbilityEventObj InitObj) : base(InitObj)
	    {
	        _EventObj = InitObj;
	    }
	}
}
