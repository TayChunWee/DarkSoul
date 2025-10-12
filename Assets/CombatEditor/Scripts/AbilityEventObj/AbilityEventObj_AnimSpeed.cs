using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor {	
	
	[AbilityEvent]
	[CreateAssetMenu(menuName = "AbilityEvents/ AnimSpeed")]
	public class AbilityEventObj_AnimSpeed : AbilityEventObj
	{
	    public float Speed = 1;
	    public override EventTimeType GetEventTimeType()
	    {
	        return EventTimeType.EventRange;
	    }
	    public override AbilityEventEffect Initialize()
	    {
	        return new AbilityEventEffect_AnimSpeed(this);
	    }
	    public override AbilityEventPreview InitializePreview()
	    {
	        return new AbilityEventPreview_AnimSpeed(this);
	    }
	}
	
	public class AbilityEventEffect_AnimSpeed : AbilityEventEffect
	{
	
	    CharacterAnimSpeedModifier modifier;
	    public AbilityEventEffect_AnimSpeed(AbilityEventObj Obj) : base(Obj)
	    {
	        _EventObj = Obj;
	    }
	    public override void StartEffect()
	    {
	        base.StartEffect();
	        modifier = _combatController._animSpeedExecutor.AddAnimSpeedModifier( (_EventObj as AbilityEventObj_AnimSpeed).Speed );
	    }
	    public override void EndEffect()
	    {
	        base.EndEffect();
	        _combatController._animSpeedExecutor.RemoveAnimSpeedModifier(modifier);
	    }
	}
}
